using System;
using DearHome_Backend.DTOs.ProductDtos;

namespace DearHome_Backend.DTOs.VariantAttributeDtos;

public class VariantDto
{
    public Guid Id { get; set; }
    public required List<string> ImageUrls { get; set; } = new List<string>();
    public required decimal PriceAdjustment { get; set; } = 0;
    public int Stock { get; set; } = 0;
    public bool IsActive { get; set; } = true; 
    public required string Sku { get; set; }
    public Guid ProductId { get; set; }
    public virtual ICollection<VariantAttributeDto>? VariantAttributes { get; set; } = new List<VariantAttributeDto>();
}
