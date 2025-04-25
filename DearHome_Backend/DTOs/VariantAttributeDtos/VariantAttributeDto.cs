using System;
using DearHome_Backend.DTOs.AttributeValueDtos;

namespace DearHome_Backend.DTOs.VariantAttributeDtos;

public class VariantAttributeDto
{
    public Guid Id { get; set; }
    public Guid VariantId { get; set; }
    public Guid AttributeValueId { get; set; }
    public virtual AttributeValueDto? AttributeValue { get; set; }
    public virtual VariantDto? Variant { get; set;}
}
