using System;
using DearHome_Backend.DTOs.OrderDetailDtos;

namespace DearHome_Backend.DTOs.PaymentDto;

public class CreatePaymentLinkRequest
{
    public string? OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? BuyerName { get; set; }
    public string? BuyerEmail { get; set; }
    public string? BuyerPhone { get; set; }
    public string? BuyerAddress { get; set; }
    public List<OrderDetailDto> Items { get; set; } = new List<OrderDetailDto>();
    public string? CancelUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public long ExpiredAt { get; set; }
    public string? Signature { get; set; }

}
