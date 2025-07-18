using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<int> GetTotalOrdersCountAsync();
    Task<decimal> GetTotalSalesAsync();
    Task<IEnumerable<OrderStatusPercentage>> GetOrderStatusPercentageAsync();
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int offSet, int limit);
    Task<IEnumerable<Order>> GetOrdersByUserIdAndStatusAsync(Guid userId, OrderStatus status, int offSet, int limit);
    Task UpdateOrderStatusByPaymentOrderCodeAsync(long paymentOrderCode, OrderStatus status); 
    Task UpdateOrderStatusByIdAsync(Guid orderId, OrderStatus status);  
    Task<bool> IsPromotionUsedByUserId(Guid userId, Guid promotionId);
    Task<IEnumerable<Guid>> GetUsedPromotionIdsByUserId(Guid userId);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetDailySalesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetMonthlySalesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetYearlySalesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetDailyOrderCountsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetMonthlyOrderCountsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetYearlyOrderCountsAsync(DateTime startDate, DateTime endDate);
}
