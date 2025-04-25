using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.AddressDtos;

namespace DearHome_Backend.DTOs.UserDtos;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public int CustomerPoints { get; set; } = 0;
    public CustomerLevels CustomerLevel { get; set; } = CustomerLevels.Bronze;
    public virtual ICollection<UpdateAddressDto>? Addresses { get; set; }
    // public virtual ICollection<Payment>? Payments { get; set; }
}
