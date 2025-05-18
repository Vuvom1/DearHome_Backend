using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;
using Net.payOS.Types;


namespace DearHome_Backend.Services.Interfaces;

public interface IOrderService : IBaseService<Order>
{
    Task<PaginatedResponse<IEnumerable<Order>>> GetAllAsync(int pageNumber, int pageSize, string? searchString = null);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int offSet, int limit);
    Task<IEnumerable<Order>> GetOrdersByUserIdAndStatusAsync(Guid userId, OrderStatus status, int offSet, int limit);
    Task<CreatePaymentResult> AddOrderWithOnlinePaymentAsync(Order order, string returnUrl);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetSalesStatsByPeriodAsync(StatsPeriod period, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<KeyValuePair<string, decimal>>> GetOrderCountsAsync(StatsPeriod period, DateTime? startDate, DateTime? endDate);

}
