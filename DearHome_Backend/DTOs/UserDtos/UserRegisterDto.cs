using System;

namespace DearHome_Backend.DTOs.UserDtos;

public class UserRegisterDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string VerificationCode { get; set; }
}
