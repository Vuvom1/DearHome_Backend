using AutoMapper;
using DearHome_Backend.DTOs;
using DearHome_Backend.DTOs.PlacementDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacementController : ControllerBase
    {
        private readonly IPlacementService _placementService;
        private readonly IMapper _mapper;
        public PlacementController(IPlacementService placementService, IMapper mapper)
        {
            _placementService = placementService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var placements = await _placementService.GetAllAsync();
            return Ok(placements);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var placement = await _placementService.GetByIdAsync(id);
            if (placement == null)
            {
                return NotFound();
            }
            return Ok(placement);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddPlacementDto addPlacementDto)
        {
            var placement = _mapper.Map<Placement>(addPlacementDto);
            await _placementService.CreateAsync(placement);
            return Ok("Placement created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePlacementDto placementDto)
        {
            var placement = _mapper.Map<Placement>(placementDto);
            await _placementService.UpdateAsync(placement);
            return Ok("Placement updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var placement = await _placementService.GetByIdAsync(id);
            if (placement == null)
            {
                return NotFound();
            }
            await _placementService.DeleteAsync(id);
            return Ok("Placement deleted successfully.");
        }
    }
}
