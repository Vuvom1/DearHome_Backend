using System;

namespace DearHome_Backend.DTOs;

public class UpdatePlacementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
