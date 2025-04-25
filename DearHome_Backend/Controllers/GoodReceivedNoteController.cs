using AutoMapper;
using DearHome_Backend.DTOs.GoodReceivedNoteDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodReceivedNoteController : ControllerBase
    {
        private readonly IGoodReceivedNoteService _goodReceivedNoteService;
        private readonly IMapper _mapper;

        public GoodReceivedNoteController(IGoodReceivedNoteService goodReceivedNoteService, IMapper mapper)
        {
            _goodReceivedNoteService = goodReceivedNoteService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var goodReceivedNotes = await _goodReceivedNoteService.GetAllAsync();
            return Ok(goodReceivedNotes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var goodReceivedNote = await _goodReceivedNoteService.GetByIdAsync(id);
            if (goodReceivedNote == null)
            {
                return NotFound();
            }
            return Ok(goodReceivedNote);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddGoodReceivedNoteDto addGoodReceivedNoteDto)
        {
            var goodReceivedNote = _mapper.Map<GoodReceivedNote>(addGoodReceivedNoteDto);
            await _goodReceivedNoteService.CreateAsync(goodReceivedNote);
            return Ok("Good Received Note created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateGoodReceivedNoteDto updateGoodReceivedNoteDto)
        {
            var goodReceivedNote = _mapper.Map<GoodReceivedNote>(updateGoodReceivedNoteDto);
            await _goodReceivedNoteService.UpdateAsync(goodReceivedNote);
            return Ok("Good Received Note updated successfully.");
        }
    }
}
