using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.PaginationDtos;
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
        CreatePaymentResult paymentResult;
        Guid orderId = Guid.NewGuid();

        var calculatedOrder = await CalculatedOrderAsync(order);
        calculatedOrder.Id = orderId;

        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(calculatedOrder.UserId);    

        // Create payment link
        paymentResult = await _paymentService.CreatePaymentLinkAsync(calculatedOrder, user!, returnUrl);

        // Update the order with payment link information
        calculatedOrder.PaymentLinkId = paymentResult.paymentLinkId;
        calculatedOrder.PaymentLinkUrl = paymentResult.checkoutUrl;
        calculatedOrder.PaymentOrderCode = paymentResult.orderCode;

        var createdOrder = await base.CreateAsync(calculatedOrder);

        return paymentResult;
    }

    public override async Task<Order> CreateAsync(Order order)
    {
        var calculatedOrder = await CalculatedOrderAsync(order);

        var createdOrder = await base.CreateAsync(calculatedOrder);

        var user = await _userRepository.GetByIdAsync(order.UserId);    

        if (order.PaymentMethod == PaymentMethods.BankTransfer) {
            CreatePaymentResult paymentResult = await _paymentService.CreatePaymentLinkAsync(order, user!, "https://example.com");
        }

        return createdOrder;
    }

    public async Task<PaginatedResponse<IEnumerable<Order>>> GetAllAsync(int pageNumber, int pageSize, string? searchString = null)
    {
        var paginatedResponse = await _orderRepository.GetAllAsync(pageNumber, pageSize, searchString);

        return paginatedResponse;
    }

    public Task<IEnumerable<KeyValuePair<string, decimal>>> GetOrderCountsAsync(StatsPeriod period, DateTime? startDate, DateTime? endDate)
    {
        if (period == StatsPeriod.Daily)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddDays(-30);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetDailyOrderCountsAsync(startDate.Value, endDate.Value);
        }
        else if (period == StatsPeriod.Monthly)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddMonths(-12);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetMonthlyOrderCountsAsync(startDate.Value, endDate.Value);
        }
        else if (period == StatsPeriod.Yearly)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddYears(-5);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetYearlyOrderCountsAsync(startDate.Value, endDate.Value);
        }
        else
        {
            throw new ArgumentException("Invalid period specified.");
        }
    }

    public Task<IEnumerable<Order>> GetOrdersByUserIdAndStatusAsync(Guid userId, OrderStatus status, int offSet, int limit)
    {
        return _orderRepository.GetOrdersByUserIdAndStatusAsync(userId, status, offSet, limit);
    }

    public Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int offSet, int limit)
    {
        return _orderRepository.GetOrdersByUserIdAsync(userId, offSet, limit);
    }

    public Task<IEnumerable<KeyValuePair<string, decimal>>> GetSalesStatsByPeriodAsync(StatsPeriod period, DateTime? startDate, DateTime? endDate)
    {
        if (period == StatsPeriod.Daily)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddDays(-30);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetDailySalesAsync(startDate.Value, endDate.Value);
        }
        else if (period == StatsPeriod.Monthly)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddMonths(-12);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetMonthlySalesAsync(startDate.Value, endDate.Value);
        }
        else if (period == StatsPeriod.Yearly)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddYears(-5);    
                endDate = DateTime.Now.Date;
            }
            return _orderRepository.GetYearlySalesAsync(startDate.Value, endDate.Value);
        }
        else
        {
            throw new ArgumentException("Invalid period specified.");
        }
    }

    public async Task<int> GetTotalOrdersCountAsync()
    {
        return await _orderRepository.GetTotalOrdersCountAsync();
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        await _orderRepository.UpdateOrderStatusByIdAsync(orderId, status);
    }

    private async Task<Order> CalculatedOrderAsync(Order order)
    {
        decimal FinalPrice = 0;

        foreach (var orderDetail in order.OrderDetails!)
        {
            var orderItem = await _variantRepository.GetWithProductByIdAsync(orderDetail.VariantId);
            
            orderDetail.UnitPrice = orderItem!.PriceAdjustment + orderItem.Product!.Price;
            orderDetail.TotalPrice = orderDetail.UnitPrice * orderDetail.Quantity;

            FinalPrice += orderDetail.TotalPrice;
        }

        // Calculate the total price of the order
        order.TotalPrice = FinalPrice;

        // Check if the order has a promotion ID and set it to null if it's empty
        if (order.PromotionId.HasValue && order.PromotionId == Guid.Empty)
        {
            order.PromotionId = null;
        } else {
            var promotion = await _orderRepository.GetByIdAsync(order.PromotionId!);
            if (promotion != null)
            {
                order.Discount = FinalPrice * promotion.Discount / 100;
                FinalPrice -= order.Discount;
            }
            
        }

        if (order.ShippingRate != null) {
            var shipping = await _shippingService.CreateShipmentAsync(order.ShippingRate, order.AddressId, order.AddressId, order.PaymentMethod == PaymentMethods.Cash ? ((int) order.TotalPrice).ToString() : "0", "0", "0", "0", "0");

            order.ShippingCode = shipping.Id;
        
            FinalPrice += shipping.Fee;
        }

        order.FinalPrice = FinalPrice;

        return order;
    }
}
