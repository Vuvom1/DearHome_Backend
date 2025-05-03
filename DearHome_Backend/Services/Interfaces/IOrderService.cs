using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;
using Net.payOS.Types;

namespace DearHome_Backend.Services.Interfaces;

public interface IOrderService : IBaseService<Order>
{
    Task<IEnumerable<Order>> GetAllAsync(int offSet, int limit);
    Task<CreatePaymentResult> AddOrderWithOnlinePaymentAsync(Order order, string returnUrl);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
}
