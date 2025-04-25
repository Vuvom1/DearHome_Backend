using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.GoodReceivedItemDtos;

namespace DearHome_Backend.DTOs.GoodReceivedNoteDtos;

public class AddGoodReceivedNoteDto
{
    public DateTime ReceivedDate { get; set; }
    public string? Note { get; set; }
    public decimal ShippingCost { get; set; } = 0;
    public GoodReceivedNoteStatus Status { get; set; } = GoodReceivedNoteStatus.Received;
    public virtual ICollection<AddGoodReceivedItemDto>? GoodReceivedItems { get; set; } = [];
}
