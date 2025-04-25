using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class PlacementRepository : BaseRepository<Placement>, IPlacementRepository
{
    private readonly DearHomeContext _context;
    public PlacementRepository(DearHomeContext context) : base(context)
    {   
        _context = context;
    }
}
