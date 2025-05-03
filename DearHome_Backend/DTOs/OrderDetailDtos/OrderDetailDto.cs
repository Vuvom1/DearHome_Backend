using System;
using DearHome_Backend.DTOs.VariantAttributeDtos;

namespace DearHome_Backend.DTOs.OrderDetailDtos;

public class OrderDetailDto
{
     public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0;
    public decimal TotalPrice { get; set; } = 0;
    public virtual VariantDto? Variant { get; set; }
}
