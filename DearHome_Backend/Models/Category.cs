using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Models;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<CategoryAttribute>? CategoryAttributes { get; set; } = new List<CategoryAttribute>();
    public virtual ICollection<Category>? SubCategories { get; set; }
    public virtual ICollection<Product>? Products { get; set; }
}
