using System;

namespace DearHome_Backend.DTOs.GoodReceivedItemDtos;

public class UpdateGoodReceivedItemDto
{
    public required Guid VariantId { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitCost { get; set; }
}
