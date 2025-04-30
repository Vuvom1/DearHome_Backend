using System;
using DearHome_Backend.Models;
using Net.payOS.Types;

namespace DearHome_Backend.Services.Interfaces;

public interface IOrderService : IBaseService<Order>
{
    Task<CreatePaymentResult> AddOrderWithOnlinePaymentAsync(Order order, string returnUrl);
}
