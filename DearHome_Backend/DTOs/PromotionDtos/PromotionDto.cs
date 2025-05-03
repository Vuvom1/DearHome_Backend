using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.OrderDtos;
using DearHome_Backend.DTOs.ProductDtos;

namespace DearHome_Backend.DTOs.PromotionDtos;

public class PromotionDto
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
    public CustomerLevels CustomerLevel { get; set; } // 0: Bronze, 1: Silver, 2: Gold, 3: Platinum
    public int Ussage { get; set; } // Usage limit for the promotion
    public virtual ICollection<ProductDto>? Products { get; set; } // List of products that this promotion applies to
    public virtual ICollection<OrderDto>? Orders { get; set; } // Orders that used t
}
