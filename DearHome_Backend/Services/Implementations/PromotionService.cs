using System;
using DearHome_Backend.Constants;
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
    public PromotionService(
        IPromotionRepository promotionRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository
    ) : base(promotionRepository)
    {
        _promotionRepository = promotionRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    public Task<IEnumerable<Promotion>> GetAllAsync(int offSet, int limit, string? search)
    {
        return _promotionRepository.GetAllAsync(offSet, limit, search);
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
}
