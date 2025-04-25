using System;

namespace DearHome_Backend.Models;

public class GoodReceivedItem : BaseEntity
{
    public required Guid VariantId { get; set; }
    public required Guid GoodReceivedNoteId { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitCost { get; set; }
    public required decimal TotalCost { get; set; }
    public virtual Variant? Variant { get; set; }
    public virtual GoodReceivedNote? GoodReceivedNote { get; set; }
}
