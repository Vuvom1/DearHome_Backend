using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class GoshipAuthResponse
{
    public string? access_token { get; set; }
    public int expires_in { get; set; }
    public string? token_type { get; set; }
}
