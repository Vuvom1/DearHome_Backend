using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class ShippingCostDto
{
    public Guid AddressToId { get; set; }
    public List<ShippingItemDto> Items { get; set; } = new List<ShippingItemDto>();
}

