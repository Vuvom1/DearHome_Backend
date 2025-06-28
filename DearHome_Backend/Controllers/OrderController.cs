using AutoMapper;
using DearHome_Backend.Constants;
using DearHome_Backend.DTOs.OrderDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IPaymentService paymentService, IMapper mapper)
            
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet] 
        public async Task<IActionResult> GetAllOrders([FromQuery] int offSet = 0, [FromQuery] int limit = 10, [FromQuery] string? searchString = null, [FromQuery] string? filter = null, [FromQuery] string? sortBy = null, [FromQuery] bool isDescending = false)
        {
            var orders = await _orderService.GetAllAsync(offSet, limit, searchString, filter, sortBy, isDescending);

            return Ok(orders);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] AddOrderDto addOrderDto, [FromQuery] string returnUrl)
        {
            if (addOrderDto == null)
            {
                return BadRequest("Order cannot be null.");
            }

            var order = _mapper.Map<Order>(addOrderDto);

            if (order.PaymentMethod == PaymentMethods.BankTransfer)
            {
                var paymentResult = await _orderService.AddOrderWithOnlinePaymentAsync(order, returnUrl);
                return Ok(paymentResult);
            }

            var createdOrder = await _orderService.CreateAsync(order);
            return Ok("Order created successfully.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatus status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return Ok("Order status updated successfully.");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId, [FromQuery] int offSet = 1, [FromQuery] int limit = 10)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId, offSet, limit);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet("user-and-status/{userId}")]
        public async Task<IActionResult> GetOrdersByUserIdAndStatus(Guid userId, [FromQuery]OrderStatus status, [FromQuery] int offSet = 1, [FromQuery] int limit = 10)
        {
            var orders = await _orderService.GetOrdersByUserIdAndStatusAsync(userId, status, offSet, limit);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(orderDtos);
        }
    }
}
