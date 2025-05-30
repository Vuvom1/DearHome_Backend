using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
{
    private new readonly DearHomeContext _context;

    public PromotionRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync(int offSet, int limit, string? search)
    {
        if (!string.IsNullOrEmpty(search))
        {
            return await _context.Promotions
                .Where(p => p.Name.Contains(search) || p.Description!.Contains(search))
                .Include(p => p.Products)
                .Skip(offSet)
                .Take(limit)
                .ToListAsync();
        }

        // If no search term is provided, return all promotions
        // with pagination
        // and include related products
        return await _context.Promotions
            .Include(p => p.Products)
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();
    }

    public override async Task<Promotion?> GetByIdAsync(object id)
    {
        var promotion = await _context.Promotions
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.Id == (Guid)id);

        return promotion;     
    }

    public override async Task AddAsync(Promotion entity)
    {
        // Add the promotion to the context
        await _context.Promotions.AddAsync(entity);

        // Add the related products to the context
        foreach (var product in entity.ProductIds!)
        {
            var productEntity = await _context.Products.FindAsync(product);
            if (productEntity != null)
            {
                // Initialize Products collection if it's null
                entity.Products ??= new List<Product>();
                entity.Products.Add(productEntity);
            }
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Promotion>> GetUsablePromotionByCustomterLeverl(CustomerLevels customerLevel)
    {
        //Retrieve the promotions usable by the user and current date
        var usablePromotions = await _context.Promotions
            .Where(p => p.CustomerLevel == customerLevel && p.Type == PromotionType.Order).ToListAsync();

        return usablePromotions;
    }

    public async Task<decimal> GetTotalDiscountAmountByOrdersAsync(DateTime startDate, DateTime endDate)
    {
        var totalDiscount = await _context.Promotions
            .Where(p => p.StartDate >= startDate && p.EndDate <= endDate)
            .Include(p => p.Orders)
            .SelectMany(p => p.Orders!)
            .Where(o => o.PromotionId != null)
            .SumAsync(o => o.Discount);

        return totalDiscount;
    }

    public async Task<int> GetTotalDiscountedOrdersCountAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.PromotionId != null && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .CountAsync();
    }

    public async Task<IEnumerable<PromotionInfo>> GetPromotionsWithDiscountedAmountsAndOrdersCountAsync(DateTime startDate, DateTime endDate)
    {
    return await _context.Promotions
            .Where(p => p.StartDate >= startDate && p.EndDate <= endDate)
            .Select(p => new PromotionInfo  
            {
                PromotionId = p.Id,
                PromotionName = p.Name,
                DiscountedAmount = p.Orders!.Sum(o => o.Discount),
                DiscountedOrdersCount = p.Orders!.Count(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                
            })
            .ToListAsync();
    }
}
