using System;

namespace DearHome_Backend.Models;

public class Review : BaseEntity
{
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public Guid VariantId { get; set; }
    public virtual Variant? Variant { get; set; }
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public Guid OrderId { get; set; }
    public virtual Order? Order { get; set; }
}
