using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.AddressDtos;
using DearHome_Backend.DTOs.OrderDetailDtos;
using DearHome_Backend.DTOs.UserDtos;

namespace DearHome_Backend.DTOs.OrderDtos;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public virtual UserDto? User { get; set; }
    public Guid AddressId { get; set; }
    public virtual AddressDto? Address { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalPrice { get; set; }
    public Guid? PromotionId { get; set; }
    // public virtual Promotion? Promotion { get; set; }
    public string? Note { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string? ShippingRate { get; set; }
    public string? ShippingCode { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public long? PaymentOrderCode { get; set; }
    public string? PaymentLinkId { get; set; }
    public string? PaymentLinkUrl { get; set; }
    public virtual ICollection<OrderDetailDto>? OrderDetails { get; set; }
}
