using System;

namespace DearHome_Backend.Models;

public class Combo : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}
