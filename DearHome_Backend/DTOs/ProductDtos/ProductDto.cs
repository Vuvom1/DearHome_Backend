using System;
using DearHome_Backend.DTOs.AttributeValueDtos;
using DearHome_Backend.DTOs.CategoryDtos;
using DearHome_Backend.DTOs.PlacementDtos;
using DearHome_Backend.DTOs.VariantAttributeDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.DTOs.ProductDtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Guid PlacementId { get; set; }
    public virtual string Status { get; set; } = "Available";
    public virtual CategoryDto? Category { get; set; }
    public virtual PlacementDto? Placement { get; set; }
    public virtual ICollection<AttributeValueDto>? AttributeValues { get; set; }
    public virtual ICollection<VariantDto>? Variants { get; set; }
}
