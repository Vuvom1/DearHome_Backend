using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Models;

public class Placement : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
