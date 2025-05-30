using System;

namespace DearHome_Backend.DTOs.StatsDtos;

public class PromotionStatisticDto
{
    public decimal TotalDiscountedAmount { get; set; }
    public int TotalDiscountedOrders { get; set; }
    public IEnumerable<PromotionInfo>? Promotions { get; set; } 
}

public class PromotionInfo
{
    public Guid PromotionId { get; set; }
    public required string PromotionName { get; set; }
    public decimal DiscountedAmount { get; set; }
    public int DiscountedOrdersCount { get; set; }
}