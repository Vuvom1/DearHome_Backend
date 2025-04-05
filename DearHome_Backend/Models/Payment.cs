using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Constants;

public class Payment  : BaseEntity
{
    public required string CardHolderName { get; set; }
    public required string CardNumber { get; set; }
    public required string CardType { get; set; }
    public required string ExpirationDate { get; set; }
    public required string Method { get; set; }
    public bool IsDefault { get; set; } = false;
    public required Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
