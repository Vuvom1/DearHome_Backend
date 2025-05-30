using DearHome_Backend.Constants;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IStatisticService _statisticService;

        public StatsController(IOrderService orderService, IUserService userService, IStatisticService statisticService)
        {
            _orderService = orderService;
            _userService = userService;
            _statisticService = statisticService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesStats([FromQuery] StatsPeriod period, DateTime? startDate, DateTime? endDate)
        {
            var salesStats = await _orderService.GetSalesStatsByPeriodAsync(period, startDate, endDate);
            return Ok(salesStats);
        }

        [HttpGet("order-counts")]
        public async Task<IActionResult> GetOrderCounts([FromQuery] StatsPeriod period, DateTime? startDate, DateTime? endDate)
        {
            var orderCounts = await _orderService.GetOrderCountsAsync(period, startDate, endDate);
            return Ok(orderCounts);
        }

        [HttpGet("overall")]
        public async Task<IActionResult> GetOverallStats()
        {
            var overallStats = await _statisticService.GetOverallStatsAsync();
            return Ok(overallStats);
        }

        [HttpGet("top-sales-products")]
        public async Task<IActionResult> GetTopSalesProducts([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int count)
        {
            var topSalesProducts = await _statisticService.GetTopSalesProductsAsync(startDate, endDate, count);
            return Ok(topSalesProducts);
        }

        [HttpGet("order-status-percentage")]
        public async Task<IActionResult> GetOrderStatusPercentage()
        {
            var orderStatusPercentage = await _statisticService.GetOrderStatusPercentageAsync();
            return Ok(orderStatusPercentage);
        }

        [HttpGet("categories-stock")]
        public async Task<IActionResult> GetCategoriesWithProductStockAndPercentage()
        {
            var categoriesWithStock = await _statisticService.GetCategoriesAndStockAmountsWithPercentagesAsync();
            return Ok(categoriesWithStock);
        }

        [HttpGet("promotion-statistics")]
        public async Task<IActionResult> GetPromotionStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var promotionStats = await _statisticService.GetPromotionStatisticsAsync(startDate, endDate);
            return Ok(promotionStats);
        }
    }
}
