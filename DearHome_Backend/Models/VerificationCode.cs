using System;

namespace DearHome_Backend.Models;

public class VerificationCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
}
