using AutoMapper;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.ReviewDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewService.GetAllAsync();
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            return Ok(reviewDtos);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var review = await _reviewService.GetByOrderIdAsync(orderId);
            if (review == null)
            {
                return NotFound();
            }
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(review);
            return Ok(reviewDtos);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null, [FromQuery] bool isDescending = false)
        {
            var paginatedReviews = await _reviewService.GetProductReviewsAsync(productId, page, pageSize, sortBy, isDescending);
            
            return Ok(paginatedReviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            var reviewDto = _mapper.Map<ReviewDto>(review);
            return Ok(reviewDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddReviewDto reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto);
            await _reviewService.CreateAsync(review);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, reviewDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto);
            await _reviewService.UpdateAsync(review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _reviewService.DeleteAsync(id);
            return NoContent();
        }
    }
}
