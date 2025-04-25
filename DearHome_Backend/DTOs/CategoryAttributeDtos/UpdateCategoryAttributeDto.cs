using System;

namespace DearHome_Backend.DTOs.CategoryAttributeDtos;

public class UpdateCategoryAttributeDto
{
    public Guid Id { get; set; }
    public Guid AttributeId { get; set; }
}
