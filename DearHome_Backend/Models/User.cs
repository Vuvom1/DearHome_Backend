using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Modals;
using Microsoft.AspNetCore.Identity;

namespace DearHome_Backend.Models;

public class User : IdentityUser<Guid>
{
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public int CustomerPoints { get; set; } = 0;
    public CustomerLevels CustomerLevel { get; set; } = CustomerLevels.Bronze;
    public virtual ICollection<Address>? Addresses { get; set; }
    public virtual ICollection<Payment>? Payments { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<Review>? Reviews { get; set; }
}
