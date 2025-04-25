using System;
using DearHome_Backend.DTOs.AttributeDtos;

namespace DearHome_Backend.Models;

public class CategoryAttribute : BaseEntity
{
    public Guid CategoryId { get; set; }
    public Guid AttributeId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Attribute? Attribute { get; set; }
    
}
