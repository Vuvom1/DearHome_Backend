using System;

namespace DearHome_Backend.Models;

public class ProductCombo : BaseEntity
{
    public required Guid ProductId { get; set; }
    public required Guid ComboId { get; set; }
    public virtual Product? Product { get; set; }
    public virtual Combo? Combo { get; set; }
}
