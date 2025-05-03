using System;

namespace DearHome_Backend.DTOs.AddressDtos;

public class AddressDto
{
    public Guid? Id { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; } = false;
}
