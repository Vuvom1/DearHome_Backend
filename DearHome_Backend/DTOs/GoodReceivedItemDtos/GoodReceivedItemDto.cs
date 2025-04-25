using System;
using DearHome_Backend.DTOs.VariantAttributeDtos;

namespace DearHome_Backend.DTOs.GoodReceivedItemDtos;

public class GoodReceivedItemDto
{
    public Guid Id { get; set; }
    public required Guid VariantId { get; set; }
    public required Guid GoodReceivedNoteId { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitCost { get; set; }
    public required decimal TotalCost { get; set; }
    public virtual VariantDto? Variant { get; set; }
}
