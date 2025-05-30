using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IStatisticService
{
    Task<object> GetOverallStatsAsync();
    Task<object> GetTopSalesProductsAsync(DateTime startDate, DateTime endDate, int count);
    Task<IEnumerable<PromotionStatisticDto>> GetPromotionStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<OrderStatusPercentage>> GetOrderStatusPercentageAsync();
    Task<IEnumerable<CategoryWithStockAndPercentage>> GetCategoriesAndStockAmountsWithPercentagesAsync();

}
