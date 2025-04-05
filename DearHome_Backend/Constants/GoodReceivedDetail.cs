using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Constants;

public class GoodReceivedDetail : BaseEntity
{
    public Guid GoodReceivedNoteId { get; set; }
    public virtual GoodReceivedNote? GoodReceivedNote { get; set; }
    public Guid VariantId { get; set; }
    public virtual Variant? Variant { get; set; }
    public int Quantity { get; set; } = 0;
    public decimal UnitCost { get; set; } = 0;
    public decimal TotalCost { get; set; } = 0;
}
