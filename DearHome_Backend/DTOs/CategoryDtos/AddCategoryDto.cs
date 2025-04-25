using System;
using DearHome_Backend.DTOs.CategoryAttributeDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.DTOs.CategoryDtos;

public class AddCategoryDto
{
     public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public virtual ICollection<AddCategoryAttributeDto>? CategoryAttributes { get; set; } = new List<AddCategoryAttributeDto>();
}
