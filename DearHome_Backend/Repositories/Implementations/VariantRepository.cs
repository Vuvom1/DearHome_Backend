using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class VariantRepository : BaseRepository<Variant>, IVariantRepository
{
    private new readonly DearHomeContext _context;
    public VariantRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public Task<Variant?> GetByIdWithVariantAttributesAsync(Guid id)
    {
        return _context.Variants
            .Include(v => v.VariantAttributes!)
                .ThenInclude(va => va.AttributeValue)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Variant>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Variants
            .Where(v => v.ProductId == productId) 
            .Include(v => v.VariantAttributes!)
                .ThenInclude(va => va.AttributeValue)
            .ToListAsync();
    }

    public Task IncreaseStockAsync(Guid variantId, int quantity)
    {
        var variant = _context.Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant != null)
        {
            variant.Stock += quantity;
            return _context.SaveChangesAsync();
        }
        return Task.CompletedTask;
    }

    public Task UpdateStockAsync(Guid variantId, int quantity)
    {
        var variant = _context.Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant != null)
        {
            variant.Stock = quantity;
            return _context.SaveChangesAsync();
        }
        return Task.CompletedTask;
    }
}
