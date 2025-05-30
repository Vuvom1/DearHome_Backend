using System;
using DearHome_Backend.Constants;

namespace DearHome_Backend.DTOs.StatsDtos;

public class OrderStatusPercentage
{
    public required OrderStatus Status { get; set; }
    public required decimal Percentage { get; set; }

}
