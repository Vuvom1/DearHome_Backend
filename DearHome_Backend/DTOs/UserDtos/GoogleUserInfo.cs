using System;

namespace DearHome_Backend.DTOs.UserDtos;

public class GoogleUserInfo
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Picture { get; set; }
    public bool VerifiedEmail { get; set; }
    public string? Hd { get; set; }
}
