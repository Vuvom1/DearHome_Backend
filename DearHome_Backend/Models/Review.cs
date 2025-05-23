using System;

namespace DearHome_Backend.Models;

public class Review : BaseEntity
{
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public Guid OrderDetailId { get; set; }
    public virtual OrderDetail? OrderDetail { get; set; }
}
