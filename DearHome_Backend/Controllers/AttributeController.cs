using AutoMapper;
using DearHome_Backend.DTOs.AttributeDtos;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : ControllerBase
    {   
        private readonly IAttributeService _attributeService;
        private readonly IMapper _mapper;

        public AttributeController(IAttributeService attributeService, IMapper mapper)
        {
            _mapper = mapper;
            _attributeService = attributeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttributes()
        {
            var attributes = await _attributeService.GetAllAsync();
            var attributeDtos = _mapper.Map<List<AttributeDto>>(attributes);

            return Ok(attributeDtos);
        }

        [HttpGet("with-category-attributes")]
        public async Task<IActionResult> GetAllAttributesWithCategoryAttributes()
        {
            var attributes = await _attributeService.GetAllWithCategoryAttributeAsync();
            var attributeDtos = _mapper.Map<List<AttributeDto>>(attributes);

            return Ok(attributeDtos);
        }

        [HttpGet("with-attribute-values")]
        public async Task<IActionResult> GetAllAttributesWithAttributeValues()
        {
            var attributes = await _attributeService.GetAllWithAttributeValuesAsync();
            var attributeDtos = _mapper.Map<List<AttributeDto>>(attributes);

            return Ok(attributeDtos);
        }

        [HttpGet("with-attribute-values-by-category/{categoryId}")]
        public async Task<IActionResult> GetAttributesWithAttributeValuesByCategoryId(Guid categoryId)
        {
            var attributes = await _attributeService.GetWithAttributeValuesByCategoryIdAsync(categoryId);
            var attributeDtos = _mapper.Map<List<AttributeDto>>(attributes);

            return Ok(attributeDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttribute([FromBody] AddAttributeDto addAttributeDto)
        {
            if (addAttributeDto == null)
            {
                return BadRequest("Attribute data is required.");
            }

            var attribute = _mapper.Map<Models.Attribute>(addAttributeDto);
            await _attributeService.CreateAsync(attribute);
            return Ok("Attribute created successfully.");
        }       

        [HttpPut]
        public async Task<IActionResult> UpdateAttribute([FromBody] UpdateAttributeDto updateAttributeDto)
        {
            if (updateAttributeDto == null)
            {
                return BadRequest("Attribute data is required.");
            }

            var attribute = _mapper.Map<Models.Attribute>(updateAttributeDto);
            await _attributeService.UpdateAsync(attribute);
            return Ok("Attribute updated successfully.");
        } 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute(Guid id)
        {
            var attribute = await _attributeService.GetByIdAsync(id);
            if (attribute == null)
            {
                return NotFound("Attribute not found.");
            }

            await _attributeService.DeleteAsync(id);
            return Ok("Attribute deleted successfully.");
        }
    }

    
}
