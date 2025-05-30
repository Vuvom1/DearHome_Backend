using System;
using DearHome_Backend.DTOs.AttributeValueDtos;

namespace DearHome_Backend.DTOs.ProductDtos;

public class AddProductDto
{
    public string? ImageUrl { get; set; }
    public string? ModelUrl { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Guid PlacementId { get; set; }
    public virtual string Status { get; set; } = "Available";
    public virtual ICollection<AddAttributeValueDto>? AttributeValues { get; set; } = new List<AddAttributeValueDto>();
}
