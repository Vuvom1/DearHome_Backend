using System;
using DearHome_Backend.Constants;

namespace DearHome_Backend.Models;

public class GoodReceivedNote : BaseEntity
{
    public DateTime ReceivedDate { get; set; }
    public string? Note { get; set; }
    public int Quantity { get; set; }
    public decimal TotalCost { get; set; } = 0;
    public decimal ShippingCost { get; set; } = 0;
    public GoodReceivedNoteStatus Status { get; set; } = GoodReceivedNoteStatus.Received;
    public virtual ICollection<GoodReceivedItem>? GoodReceivedItems { get; set; } = [];
}
