using System;

namespace DearHome_Backend.DTOs.PaymentDto;

public class PaymentLinkResponse
{
    public string? Bin { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int OrderCode { get; set; }
    public string? Curency { get; set; }
    public string? PaymentLinkId { get; set; }
    public string? Status { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
}
