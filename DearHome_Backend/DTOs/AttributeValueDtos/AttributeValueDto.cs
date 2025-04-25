using System;

namespace DearHome_Backend.DTOs.AttributeValueDtos;

public class AttributeValueDto
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public Guid AttributeId { get; set; }
}
