using System.Text;
using System.Text.Json;
using AutoMapper;
using DearHome_Backend.DTOs.ProductDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Implementations;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int offSet = 0, [FromQuery] int limit = 10, [FromQuery] string? searchString = null, [FromQuery] string? filter = null, [FromQuery] string? sortBy = null, [FromQuery] bool isDescending = false)
        {
            var products = await _productService.GetAllAsync(offSet, limit, searchString, filter, sortBy, isDescending);
            return Ok(products);
        }

        [HttpGet("top-sales")]
        public async Task<IActionResult> GetTopSalesProducts([FromQuery] int count)
        {
            var products = await _productService.GetTopSalesProductsAsync(count);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet("get-by-category/{id}")]
        public async Task<IActionResult> GetByCategoryId(Guid id)
        {
            var products = await _productService.GetByCategoryIdAsync(id);
            return Ok(products);
        }

        [HttpGet("with-variants")]
        public async Task<IActionResult> GetAllWithVariants()
        {
            var products = await _productService.GetAllWithVariantsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdWithAttributeValuesAndVariantsAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto addProductDto)
        {
            var product = _mapper.Map<Product>(addProductDto);
            await _productService.CreateAsync(product);
            return Ok("Product created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productService.UpdateAsync(product);
            return Ok("Product updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await _productService.DeleteAsync(id);
            return Ok("Product deleted successfully.");
        }
    }
}
