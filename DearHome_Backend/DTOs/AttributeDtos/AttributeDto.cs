using System;
using DearHome_Backend.DTOs.AttributeValueDtos;
using DearHome_Backend.DTOs.CategoryAttributeDtos;

namespace DearHome_Backend.DTOs.AttributeDtos;

public class AttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public virtual ICollection<CategoryAttributeDto> CategoryAttributes { get; set; } = new List<CategoryAttributeDto>();
    public virtual ICollection<AttributeValueDto> AttributeValues { get; set; } = new List<AttributeValueDto>();
}
