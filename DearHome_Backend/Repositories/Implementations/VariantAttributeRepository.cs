using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class VariantAttributeRepository : BaseRepository<VariantAttribute>, IVariantAttributeRepository
{
    private new readonly DearHomeContext _context;
    public VariantAttributeRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VariantAttribute>> GetWithAttributeValuesByIdAsync(IEnumerable<Guid> ids)
    {
        return await _context.VariantAttributes
            .Include(va => va.AttributeValue)
            .Where(va => ids.Contains(va.Id))
            .ToListAsync();
    }
}
