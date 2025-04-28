using System;

namespace DearHome_Backend.Models.OrderDetailDtos;

public class AddOrderDetailDto
{
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
}
