using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class GoodReceivedNoteRepository : BaseRepository<GoodReceivedNote>, IGoodReceivedNoteRepository
{
    private new readonly DearHomeContext _context;
    public GoodReceivedNoteRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<GoodReceivedNote>> GetAllAsync()
    {
        return await _context.GoodReceivedNotes
            .Include(x => x.GoodReceivedItems!)
            .ThenInclude(x => x.Variant)
            .ToListAsync();
    }

    public override async Task<GoodReceivedNote?> GetByIdAsync(object id)
    {
        return await _context.GoodReceivedNotes
            .Include(x => x.GoodReceivedItems!)
            .ThenInclude(x => x.Variant)
            .FirstOrDefaultAsync(x => x.Id == (Guid)id);
    }
}
