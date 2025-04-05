using System;

namespace DearHome_Backend.Models;

public class AttributeValue : BaseEntity
{
    public required string Value { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid AttributeId { get; set; }
    public virtual Attribute? Attribute { get; set; }
}
