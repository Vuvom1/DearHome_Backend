using System;
using DearHome_Backend.Data;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class AttributeRepository : BaseRepository<Models.Attribute>, IAttributeRepository
{
    private readonly DearHomeContext _context;
    public AttributeRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Attribute>> GetAllWithAttributeValuesAsync()
    {
        return await _context.Attributes
            .Include(a => a.AttributeValues)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Attribute>> GetAllWithCategoryAttributeAsync()
    {
        return await _context.Attributes
            .Include(a => a.CategoryAttributes)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Attribute>> GetWithAttributeValuesByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Attributes
            .Include(a => a.CategoryAttributes!)
                .ThenInclude(ca => ca.Category)
            .Include(a => a.AttributeValues)
            .Where(a => a.CategoryAttributes != null && a.CategoryAttributes.Any(ca => ca.CategoryId == categoryId))
            .ToListAsync();
    }

    public Task<Models.Attribute?> GetByIdWithAttributeValuesAsync(Guid id)
    {
        return _context.Attributes
            .Include(a => a.AttributeValues)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
