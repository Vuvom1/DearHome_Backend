using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;
using Net.payOS.Types;

namespace DearHome_Backend.Services.Implementations;

public class OrderService : BaseService<Order>, IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    private readonly IUserRepository _userRepository;
    private readonly IVariantRepository _variantRepository;
    public OrderService(
        IOrderRepository orderRepository, 
    IPaymentService paymentService, 
    IShippingService shippingService,
    IUserRepository userRepository,
    IVariantRepository variantRepository) : base(orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
    }

    public async Task<CreatePaymentResult> AddOrderWithOnlinePaymentAsync(Order order, string returnUrl)
    {
        decimal FinalPrice = 0;
        CreatePaymentResult paymentResult;
        Guid orderId = Guid.NewGuid();

        foreach (var orderDetail in order.OrderDetails!)
        {
            var orderItem = await _variantRepository.GetWithProductByIdAsync(orderDetail.VariantId);
            
            orderDetail.UnitPrice = orderItem!.PriceAdjustment + orderItem.Product!.Price;
            orderDetail.TotalPrice = orderDetail.UnitPrice * orderDetail.Quantity;

            FinalPrice += orderDetail.TotalPrice;
        }

        if (order.PromotionId.HasValue && order.PromotionId == Guid.Empty)
        {
            order.PromotionId = null;
        }

        order.TotalPrice = FinalPrice;

        // Check if the order has a shipping rate
        // If the order has a shipping rate, create a shipment
        if (order.ShippingRate != null) {
            var shipping = await _shippingService.CreateShipmentAsync(order.ShippingRate, order.AddressId, order.AddressId, order.PaymentMethod == PaymentMethods.Cash ? order.TotalPrice.ToString() : "0", "0", "0", "0", "0");

            order.ShippingCode = shipping.Id;
        
            FinalPrice += shipping.Fee;
        }

        order.FinalPrice = FinalPrice;
        order.Id = orderId;

        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(order.UserId);    

        // Create payment link
        paymentResult = await _paymentService.CreatePaymentLinkAsync(order, user!, returnUrl);

        // Update the order with payment link information
        order.PaymentLinkId = paymentResult.paymentLinkId;
        order.PaymentLinkUrl = paymentResult.checkoutUrl;
        order.PaymentOrderCode = paymentResult.orderCode;

        var createdOrder = await base.CreateAsync(order);

        return paymentResult;
    }

    public override async Task<Order> CreateAsync(Order order)
    {
        decimal FinalPrice = 0;

        foreach (var orderDetail in order.OrderDetails!)
        {
            var orderItem = await _variantRepository.GetWithProductByIdAsync(orderDetail.VariantId);
            
            orderDetail.UnitPrice = orderItem!.PriceAdjustment + orderItem.Product!.Price;
            orderDetail.TotalPrice = orderDetail.UnitPrice * orderDetail.Quantity;

            FinalPrice += orderDetail.TotalPrice;
        }

        if (order.PromotionId.HasValue && order.PromotionId == Guid.Empty)
        {
            order.PromotionId = null;
        }

        if (order.ShippingRate != null) {
            var shipping = await _shippingService.CreateShipmentAsync(order.ShippingRate, order.AddressId, order.AddressId, order.PaymentMethod == PaymentMethods.Cash ? order.TotalPrice.ToString() : "0", "0", "0", "0", "0");

            order.ShippingCode = shipping.Id;
        
            FinalPrice += shipping.Fee;
        }

        order.FinalPrice = FinalPrice;

        var createdOrder = await base.CreateAsync(order);

        var user = await _userRepository.GetByIdAsync(order.UserId);    

        if (order.PaymentMethod == PaymentMethods.BankTransfer) {
            CreatePaymentResult paymentResult = await _paymentService.CreatePaymentLinkAsync(order, user!, "https://example.com");
        }

        return createdOrder;
    }
}
