using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.AttributeValueDtos;

namespace DearHome_Backend.DTOs.AttributeDtos;

public class AddAttributeDto
{
    public required string Name { get; set; }
    public required AttributeType Type { get; set; }
    public virtual ICollection<AddAttributeValueDto>? AttributeValues { get; set; }
}
