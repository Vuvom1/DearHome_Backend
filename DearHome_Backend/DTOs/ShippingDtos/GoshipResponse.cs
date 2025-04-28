using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class GoshipResponse
{
    public int code { get; set; }
    public string status { get; set; }
    public object data { get; set; }
}
