using System;
using DearHome_Backend.Constants;

namespace DearHome_Backend.Models;

public class Promotion : BaseEntity
{
    public required string Name {get; set;}
    public required string Code {get; set;}
    public required decimal DiscountPercentage {get; set;}
    public DateTime StartDate {get; set;}
    public DateTime EndDate {get; set;}
    public bool IsActive {get; set;} = true;
    public string? Description {get; set;}
    public PromotionType Type {get; set;}
    public CustomerLevels CustomerLevel {get; set;} // 0: Bronze, 1: Silver, 2: Gold, 3: Platinum
    public int Ussage {get; set;} // Usage limit for the promotion
    public ICollection<Guid>? ProductIds {get; set;} // List of product IDs that this promotion applies to
    public virtual ICollection<Product>? Products {get; set;} // List of products that this promotion applies to
    public virtual ICollection<Order>? Orders {get; set;} // Orders that used this promotion

}
