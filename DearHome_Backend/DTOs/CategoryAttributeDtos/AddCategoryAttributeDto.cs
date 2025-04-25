using System;

namespace DearHome_Backend.DTOs.CategoryAttributeDtos;

public class AddCategoryAttributeDto
{
    public Guid CategoryId { get; set; }
    public Guid AttributeId { get; set; }
}
