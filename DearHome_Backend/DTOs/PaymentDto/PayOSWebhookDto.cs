using System;

namespace DearHome_Backend.DTOs.PaymentDto;

public class PayOSWebhookDto
{
    public string? Code { get; set; }
    public string? Id { get; set; }
    public bool Cancel { get; set; }
    public string? Status { get; set; }
    public long OrderCode { get; set; }
}
