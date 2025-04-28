using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Modals;

namespace DearHome_Backend.Models;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public Guid AddressId { get; set; }
    public virtual Address? Address { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalPrice { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal FinalPrice { get; set; } = 0;
    public Guid? PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }
    public string? Note { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string? ShippingRate { get; set; }
    public string? ShippingCode { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    
}
