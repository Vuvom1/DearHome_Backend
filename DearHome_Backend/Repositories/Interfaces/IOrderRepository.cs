using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task UpdateOrderStatusByPaymentOrderCodeAsync(long paymentOrderCode, OrderStatus status);   
}
