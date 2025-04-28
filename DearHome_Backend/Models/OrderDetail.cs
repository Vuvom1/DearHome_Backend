using System;

namespace DearHome_Backend.Models;

public class OrderDetail : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0;
    public decimal TotalPrice { get; set; } = 0;
    public virtual Order? Order { get; set; }
    public virtual Variant? Variant { get; set; }
}
