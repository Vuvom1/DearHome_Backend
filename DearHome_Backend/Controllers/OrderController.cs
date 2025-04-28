using AutoMapper;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] AddOrderDto addOrderDto)
        {
            if (addOrderDto == null)
            {
                return BadRequest("Order cannot be null.");
            }

            var order = _mapper.Map<Order>(addOrderDto);
            var createdOrder = await _orderService.CreateAsync(order);
            return Ok(createdOrder);
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
    }
}
