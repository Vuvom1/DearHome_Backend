using System;
using System.Text.Json;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class PromotionService : BaseService<Promotion>, IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly NatsService _natsService;
    public PromotionService(
        IPromotionRepository promotionRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        NatsService natsService
    ) : base(promotionRepository)
    {
        _promotionRepository = promotionRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _natsService = natsService;
    }

    public override Task<PaginatedResult<Promotion>> GetAllAsync(int offSet, int limit, string? search)
    {
        return _promotionRepository.GetAllAsync(offSet, limit, search);
    }

    public override Task<PaginatedResult<Promotion>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false)
    {
        return _promotionRepository.GetAllAsync(offSet, limit, search, filter, sortBy, isDescending);
    }

    public override async Task<Promotion> CreateAsync(Promotion entity)
    {
        var createdPromotion = await base.CreateAsync(entity).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                //Publish the promotion created event
                PublishPromotionCreatedEventAsync(task.Result);
            }
            return task.Result;
        });

        return createdPromotion;
    }

    public override async Task<Promotion> UpdateAsync(Promotion entity)
    {
        var updatedPromotion = await base.UpdateAsync(entity).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                //Publish the promotion updated event
                PublishPromotionUpdatedEventAsync(task.Result);
            }
            return task.Result;
        });

        return updatedPromotion;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id).ContinueWith(_ => PublishPromotionDeletedEventAsync(id));

        return true;
    }

    public async Task<IEnumerable<Promotion>> GetUsablePromotionByUserId(Guid userId)
    {
        //Retrieve the user from the repository
        var user = await _userRepository.GetByIdAsync(userId);

        //Ensure that one user only uses one promotion
        var usedPromotionIds = await _orderRepository.GetUsedPromotionIdsByUserId(userId);

        var usablePromotions = await _promotionRepository.GetUsablePromotionByCustomterLeverl(user!.CustomerLevel);

        var unusedPromotionIds = usablePromotions
            .Where(p => !usedPromotionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToList();

        //Retieve the promotions usable by the user and current date
        usablePromotions = usablePromotions
            .Where(p => p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
            .Where(p => unusedPromotionIds.Contains(p.Id))
            .ToList();

        return usablePromotions;
    }

    // Create a DTO for promotion events
    private static object CreatePromotionEventData(Promotion promotion)
    {
        return new
        {
            id = promotion.Id,
            name = promotion.Name,
            code = promotion.Code,
            start_date = promotion.StartDate,
            end_date = promotion.EndDate,
            discount_percentage = promotion.DiscountPercentage,
            customer_level = promotion.CustomerLevel,
            created_at = promotion.CreatedAt,
            is_active = promotion.IsActive,
            description = promotion.Description
        };
    }

    // Refactored publish methods
    private void PublishPromotionCreatedEventAsync(Promotion promotion)
    {
        _natsService.Publish(NatsConstants.PromotionCreated, CreatePromotionEventData(promotion));
    }

    private void PublishPromotionUpdatedEventAsync(Promotion promotion)
    {
        _natsService.Publish(NatsConstants.PromotionUpdated, CreatePromotionEventData(promotion));
    }

    private void PublishPromotionDeletedEventAsync(Guid promotionId)
    {
        _natsService.Publish(NatsConstants.PromotionDeleted, new { id = promotionId });
    }
}
