using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class ShippingItemDto
{
    public Guid VariantId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0;
}
