using System;
using DearHome_Backend.Constants;

namespace DearHome_Backend.DTOs.PromotionDtos;

public class UpdatePromotionDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public PromotionType Type { get; set; }
    public CustomerLevels CustomerLevel { get; set; } 
    public int Ussage { get; set; } 
    public ICollection<Guid>? ProductIds { get; set; } 
}
