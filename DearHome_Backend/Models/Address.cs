using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Modals;

public class Address : BaseEntity
{
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; } = false;
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
