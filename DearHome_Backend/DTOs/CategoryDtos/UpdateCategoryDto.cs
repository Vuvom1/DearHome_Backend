using System;

namespace DearHome_Backend.DTOs.CategoryAttributeDtos;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public virtual ICollection<UpdateCategoryAttributeDto>? CategoryAttributes { get; set; } = new List<UpdateCategoryAttributeDto>(); 
}
