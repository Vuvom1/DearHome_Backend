using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Constants;

public class Attribute : BaseEntity
{
    public required string Name { get; set; }
    public required Type Type { get; set; }
    public bool IsRequired { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}
