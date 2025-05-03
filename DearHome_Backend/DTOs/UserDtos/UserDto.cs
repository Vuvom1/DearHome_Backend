using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.AddressDtos;

namespace DearHome_Backend.DTOs.UserDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public int CustomerPoints { get; set; } = 0;
    public CustomerLevels CustomerLevel { get; set; } = CustomerLevels.Bronze;
    public virtual ICollection<AddressDto>? Addresses { get; set; }
}
