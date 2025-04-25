using System;
using DearHome_Backend.DTOs.AttributeDtos;
using DearHome_Backend.DTOs.AttributeValueDtos;

namespace DearHome_Backend.DTOs.CategoryAttributeDtos;

public class CategoryAttributeDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid AttributeId { get; set; }
    public virtual AttributeDto? Attribute { get; set; }
    public virtual ICollection<AttributeValueDto>? AttributeValues { get; set; } = new List<AttributeValueDto>();
}
