using System;

namespace DearHome_Backend.DTOs.UserDtos;

public class UserLoginDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
