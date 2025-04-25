using AutoMapper;
using DearHome_Backend.DTOs.CategoryAttributeDtos;
using DearHome_Backend.DTOs.CategoryDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("get-by-slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            var category = await _categoryService.GetCategoryBySlug(slug);
            if (category == null)
            {
                return NotFound($"Category with slug '{slug}' not found.");
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        } 


        [HttpGet("with-parent-and-attributes")]
        public async Task<IActionResult> GetAllCategoriesWithParentAndAttributes()
        {
            var categories = await _categoryService.GetAllWithParentAndAttributes();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("with-attributes-and-attribute-values")]
        public async Task<IActionResult> GetAllCategoriesWithAttributesAndAttributeValues()
        {
            var categories = await _categoryService.GetAllWithAttributesAndAttributeValues();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryByName(name);
            if (category == null)
            {
                return NotFound($"Category with name '{name}' not found.");
            }
            return Ok(category);
        }

        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetCategoriesByParentId(Guid parentId)
        {
            var categories = await _categoryService.GetCategoriesByParentId(parentId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryDto addCategoryDto)
        {
            if (addCategoryDto == null)
            {
                return BadRequest("Category data is required.");
            }

            var category = _mapper.Map<Category>(addCategoryDto);
            await _categoryService.CreateAsync(category);
            return Ok("Category created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto == null)
            {
                return BadRequest("Category data is required.");
            }

            var category = _mapper.Map<Category>(updateCategoryDto);
            await _categoryService.UpdateAsync(category);
            return Ok("Category updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok("Category deleted successfully.");
        }
    }
}
