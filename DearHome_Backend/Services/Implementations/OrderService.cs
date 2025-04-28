using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class OrderService : BaseService<Order>, IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    public OrderService(IOrderRepository orderRepository, IPaymentService paymentService, IShippingService shippingService) : base(orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
    }

    public override async Task<Order> CreateAsync(Order order)
    {
        decimal FinalPrice = 0;

        foreach (var orderDetail in order.OrderDetails!)
        {
            decimal totalAmount = orderDetail.UnitPrice * orderDetail.Quantity;
            FinalPrice += totalAmount;

            orderDetail.TotalPrice = totalAmount;
        }

        if (order.PromotionId.HasValue && order.PromotionId == Guid.Empty)
        {
            order.PromotionId = null;
        }

        var createdOrder = await base.CreateAsync(order);

        if (order.PaymentMethod == PaymentMethods.BankTransfer) {
            await _paymentService.CreatePaymentLinkAsync(order, order.User!, "https://example.com");
        }

        if (order.ShippingRate != null) {
            var shipping = await _shippingService.CreateShipmentAsync(order.ShippingRate, order.AddressId, order.AddressId, order.PaymentMethod == PaymentMethods.Cash ? order.TotalPrice.ToString() : "0", "0", "0", "0", "0");

            createdOrder.ShippingCode = shipping.Id;
        }

        return createdOrder;
    }
}
