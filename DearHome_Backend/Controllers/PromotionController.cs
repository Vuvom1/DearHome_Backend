using AutoMapper;
using DearHome_Backend.DTOs.PromotionDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IMapper _mapper;

        public PromotionController(IPromotionService promotionService, IMapper mapper)
        {
            _promotionService = promotionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPromotions([FromQuery] int offSet=0, [FromQuery] int limit=10, [FromQuery] string? search=null, [FromQuery] string? filter=null, [FromQuery] string? sortBy=null, [FromQuery] bool isDescending=false)
        {
            var promotions = await _promotionService.GetAllAsync(offSet, limit, search, filter, sortBy, isDescending);
            return Ok(promotions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotion(Guid id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null)
            {
                return NotFound("Promotion not found.");
            }
            var promotionDto = _mapper.Map<PromotionDto>(promotion);
            return Ok(promotionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] AddPromotionDto promotionDto)
        {
            if (promotionDto == null)
            {
                return BadRequest("Promotion data is null.");
            }

            var promotion = _mapper.Map<Promotion>(promotionDto);
            await _promotionService.CreateAsync(promotion);
            return CreatedAtAction(nameof(GetPromotion), new { id = promotion.Id }, promotionDto);
        }

        [HttpGet("usable/{userId}")]
        public async Task<IActionResult> GetUsablePromotionsByUserId(Guid userId)
        {
            var promotions = await _promotionService.GetUsablePromotionByUserId(userId);
            var promotionDtos = _mapper.Map<IEnumerable<PromotionDto>>(promotions);
            return Ok(promotionDtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(Guid id, [FromBody] UpdatePromotionDto promotionDto)
        {

            
            var promotion = _mapper.Map<Promotion>(promotionDto);
            promotion.Id = id;
            await _promotionService.UpdateAsync(promotion);
            return NoContent();
        }
    }
}
