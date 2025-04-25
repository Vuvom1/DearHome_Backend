using System;

namespace DearHome_Backend.Models;

public class AttributeValue : BaseEntity
{
    public required string Value { get; set; }
    public Guid AttributeId { get; set; }
    public virtual Attribute? Attribute { get; set; }
    public virtual List<VariantAttribute>?  VariantAttributes { get; set; } = new List<VariantAttribute>();
}
