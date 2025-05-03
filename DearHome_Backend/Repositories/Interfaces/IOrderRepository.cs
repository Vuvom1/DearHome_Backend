using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetAllAsync(int offSet, int limit);
    Task UpdateOrderStatusByPaymentOrderCodeAsync(long paymentOrderCode, OrderStatus status); 
    Task UpdateOrderStatusByIdAsync(Guid orderId, OrderStatus status);  
    Task<bool> IsPromotionUsedByUserId(Guid userId, Guid promotionId);
    Task<IEnumerable<Guid>> GetUsedPromotionIdsByUserId(Guid userId);
}
