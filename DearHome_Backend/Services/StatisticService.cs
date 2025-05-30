using System;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services;

public class StatisticService : IStatisticService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPromotionRepository _promotionRepository;

    public StatisticService(IUserRepository userRepository, IOrderRepository orderRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IPromotionRepository promotionRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
    }

    public async Task<object> GetOverallStatsAsync()
    {
        var TotalSales = await _orderRepository.GetTotalSalesAsync();
        var TotalCustomers = await _userRepository.GetTotalCustomersCountAsync();
        var TotalOrders = await _orderRepository.GetTotalOrdersCountAsync();
        var TotalProducts = await _productRepository.GetTotalProductsCountAsync();

        return new
        {
            TotalSales,
            TotalCustomers,
            TotalOrders,
            TotalProducts
        };
    }

    public async Task<object> GetTopSalesProductsAsync(DateTime startDate, DateTime endDate, int count)
    {
        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be earlier than end date.");
        }

        var topSaleProductsByAmount = await _productRepository.GetTopSalesProductsWithAmountsAsync(startDate, endDate, count);
        var topSaleProductsByCount = await _productRepository.GetTopSalesProductsWithCountsAsync(startDate, endDate, count);

        return new
        {
            TopSaleProductsByAmount = topSaleProductsByAmount,
            TopSaleProductsByCount = topSaleProductsByCount
        };

    }

    public async Task<IEnumerable<CategoryWithStockAndPercentage>> GetCategoriesAndStockAmountsWithPercentagesAsync()
    {
        return await _categoryRepository.GetCategoriesWithTotalStockAndPercentageAsync();
    }

    public async Task<IEnumerable<OrderStatusPercentage>> GetOrderStatusPercentageAsync()
    {
        return await _orderRepository.GetOrderStatusPercentageAsync();
    }

    public async Task<IEnumerable<PromotionStatisticDto>> GetPromotionStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be earlier than end date.");
        }

        var totalDiscountAmount = await _promotionRepository.GetTotalDiscountedOrdersCountAsync(startDate, endDate);
        var totalDiscountedOrders = await _promotionRepository.GetTotalDiscountedOrdersCountAsync(startDate, endDate);
        var promotions = await _promotionRepository.GetPromotionsWithDiscountedAmountsAndOrdersCountAsync(startDate, endDate);

        return new List<PromotionStatisticDto>
        {
            new PromotionStatisticDto
            {
                TotalDiscountedAmount = totalDiscountAmount,
                TotalDiscountedOrders = totalDiscountedOrders,
                Promotions = promotions
            }
        };
    }
}
