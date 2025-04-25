using System;
using DearHome_Backend.DTOs.CategoryAttributeDtos;

namespace DearHome_Backend.DTOs.CategoryDtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public virtual ICollection<CategoryDto>? SubCategories { get; set; } = new List<CategoryDto>();
    public virtual ICollection<CategoryAttributeDto>? CategoryAttributes { get; set; } = new List<CategoryAttributeDto>();
}
