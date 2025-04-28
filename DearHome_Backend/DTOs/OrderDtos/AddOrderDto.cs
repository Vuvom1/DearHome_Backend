using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models.OrderDetailDtos;

namespace DearHome_Backend.DTOs.OrderDtos;

public class AddOrderDto
{
    public Guid AddressId { get; set; }
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.WaitForPayment;
    public Guid PromotionId { get; set; } = Guid.Empty;
    public string? Note { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public string? ShippingRate { get; set; }
    public virtual ICollection<AddOrderDetailDto>? OrderDetails { get; set; }
}
