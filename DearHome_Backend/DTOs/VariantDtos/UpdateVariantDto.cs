using System;
using DearHome_Backend.DTOs.VariantAttributeDtos;

namespace DearHome_Backend.DTOs.VariantDtos;

public class UpdateVariantDto
{
    public Guid Id { get; set; }
    public required List<string> ImageUrls { get; set; } = new List<string>();
    public string? ModelUrl { get; set; }
    public required decimal PriceAdjustment { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public required string Sku { get; set; }
    public Guid ProductId { get; set; }
    public virtual ICollection<UpdateVariantAttributeDto>? VariantAttributes { get; set; } = new List<UpdateVariantAttributeDto>();
}
