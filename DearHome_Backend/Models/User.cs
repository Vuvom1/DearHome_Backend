using System;
using DearHome_Backend.Constants;

namespace DearHome_Backend.Models;

public class User : BaseEntity
{
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int CustomerPoints { get; set; } = 0;
    public CustomerLevels CustomerLevel { get; set; } = CustomerLevels.Bronze;
}
