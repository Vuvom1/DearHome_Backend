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
    public int PromotionType {get; set;} // 0: CustomerLevel, 5: Order
    public CustomerLevels CustomerLevel {get; set;} // 0: Bronze, 1: Silver, 2: Gold, 3: Platinum
    public int MaximumUsage {get; set;} = 1; // Maximum usage for each user

}
