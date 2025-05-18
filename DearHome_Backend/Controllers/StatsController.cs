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

        public StatsController(IOrderService orderService)
        {
            _orderService = orderService;
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
    }
}
