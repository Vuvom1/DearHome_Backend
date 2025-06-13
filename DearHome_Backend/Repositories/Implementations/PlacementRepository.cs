using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class PlacementRepository : BaseRepository<Placement>, IPlacementRepository
{
    private new readonly DearHomeContext _context;
    public PlacementRepository(DearHomeContext context) : base(context)
    {   
        _context = context;
    }

    public async Task<string> GetPlacementNameByIdAsync(Guid id)
    {
        return await _context.Placements
            .Where(p => p.Id == id)
            .Select(p => p.Name)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Placement with ID {id} not found.");
    }
}
