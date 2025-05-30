using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class GoshipShippingCostResponse
{
    public int code { get; set; }
    public string? status { get; set; }
    public List<object>? data { get; set; }
}
