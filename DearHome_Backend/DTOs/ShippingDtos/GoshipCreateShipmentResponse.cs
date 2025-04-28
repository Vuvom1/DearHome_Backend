using System;

namespace DearHome_Backend.DTOs.ShippingDtos;

public class GoshipCreateShipmentResponse
{
    public int Code { get; set; }
    public string? Status { get; set; }
    public object? Data { get; set; }
    public string?Id { get; set; }
    public int ShipmentStatus { get; set; }
    public string? ShipmentStatusTxt { get; set; }
    public int Cod { get; set; }
    public int Fee { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public string? CarrierShortName { get; set; }
    public string? SortingCode { get; set; }
    public string? CreatedAt { get; set; }
}
