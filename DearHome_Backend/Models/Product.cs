using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Models;

public class Product : BaseEntity
{
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Guid PlacementId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Placement? Placement { get; set; }
    public virtual string Status { get; set; } = "Available";
    public virtual List<Variant>? Variants { get; set; } = new List<Variant>();
    public virtual ICollection<AttributeValue>? AttributeValues { get; set; } = new List<AttributeValue>();
}
