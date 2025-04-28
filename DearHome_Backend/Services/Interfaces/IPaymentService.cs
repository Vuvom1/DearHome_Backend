using System;
using DearHome_Backend.Models;
using Net.payOS.Types;

namespace DearHome_Backend.Services.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentResult> CreatePaymentLinkAsync(Order order, User user, string url);
    Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId);
    Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderId);

}
