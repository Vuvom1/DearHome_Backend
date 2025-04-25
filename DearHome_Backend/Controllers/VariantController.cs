using AutoMapper;
using DearHome_Backend.DTOs.VariantDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantController : ControllerBase
    {
        private readonly IVariantService _variantService;
        private readonly IMapper _mapper;

        public VariantController(IVariantService variantService, IMapper mapper)
        {
            _variantService = variantService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var variants = await _variantService.GetAllAsync();
            return Ok(variants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var variant = await _variantService.GetByIdAsync(id);
            if (variant == null)
            {
                return NotFound();
            }
            return Ok(variant);
        }

        [HttpGet("get-by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            var variants = await _variantService.GetByProductIdAsync(productId);
            if (variants == null || !variants.Any())
            {
                return NotFound($"No variants found for product with ID {productId}.");
            }
            return Ok(variants);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddVariantDto addVariantDto)
        {
            var variant = _mapper.Map<Variant>(addVariantDto);
            await _variantService.CreateAsync(variant);
            return Ok("Variant created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateVariantDto updateVariantDto)
        {
            var variant = _mapper.Map<Variant>(updateVariantDto);
            await _variantService.UpdateAsync(variant);
            return Ok("Variant updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var variant = await _variantService.GetByIdAsync(id);
            if (variant == null)
            {
                return NotFound();
            }
            await _variantService.DeleteAsync(id);
            return Ok("Variant deleted successfully.");
        }
    }
}
