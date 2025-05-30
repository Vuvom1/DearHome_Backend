using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class GoodReceivedItemRepository : BaseRepository<GoodReceivedItem>, IGoodReceivedItemRepository
{
    private new readonly DearHomeContext _context;

    public GoodReceivedItemRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }
}
