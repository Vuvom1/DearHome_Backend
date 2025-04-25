using System;

namespace DearHome_Backend.Models;

public class VariantAttribute : BaseEntity
{
    public Guid VariantId { get; set; }
    public virtual Variant? Variant { get; set; }
    public Guid AttributeValueId { get; set; }
    public virtual AttributeValue? AttributeValue { get; set; }
    
}
