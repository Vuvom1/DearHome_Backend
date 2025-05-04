using System;

namespace DearHome_Backend.Models;

public class Variant : BaseEntity
{
    public required List<string> ImageUrls { get; set; } = new List<string>();
    public required decimal PriceAdjustment { get; set; } = 0;
    public int Stock { get; set; } = 0;
    public bool IsActive { get; set; } = true; 
    public required string Sku { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ICollection<VariantAttribute>? VariantAttributes { get; set; } = new List<VariantAttribute>();
    public virtual ICollection<GoodReceivedNote>? GoodReceivedNotes { get; set; } = new List<GoodReceivedNote>();
    public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
}