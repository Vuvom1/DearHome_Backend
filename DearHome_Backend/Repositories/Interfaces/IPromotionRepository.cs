using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IPromotionRepository : IBaseRepository<Promotion>
{
    Task<IEnumerable<Promotion>> GetUsablePromotionByCustomterLeverl(CustomerLevels customerLevel);
    Task<decimal> GetTotalDiscountAmountByOrdersAsync(DateTime startDate, DateTime endDate);
    Task<int> GetTotalDiscountedOrdersCountAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<PromotionInfo>> GetPromotionsWithDiscountedAmountsAndOrdersCountAsync(DateTime startDate, DateTime endDate);
}
