using System;
using DearHome_Backend.DTOs.AttributeValueDtos;

namespace DearHome_Backend.DTOs.AttributeDtos;

public class UpdateAttributeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public virtual ICollection<UpdateAttributeValueDto>? AttributeValues { get; set; }
}
