using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;

namespace DearHome_Backend.Models;

public class Attribute : BaseEntity
{
    public required string Name { get; set; }
    public required AttributeType Type { get; set; }
    public virtual ICollection<CategoryAttribute>? CategoryAttributes { get; set; } = new List<CategoryAttribute>();
    public virtual ICollection<AttributeValue>? AttributeValues { get; set; } = new List<AttributeValue>();
}
