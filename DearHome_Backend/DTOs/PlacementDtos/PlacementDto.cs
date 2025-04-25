using System;

namespace DearHome_Backend.DTOs.PlacementDtos;

public class PlacementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
