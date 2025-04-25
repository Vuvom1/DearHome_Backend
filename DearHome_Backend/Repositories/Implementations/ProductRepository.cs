using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private new readonly DearHomeContext _context;
    public ProductRepository(DearHomeContext context) : base(context)
    {   
        _context = context;
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithVariantsAsync()
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public Task<Product?> GetByIdWithAttributeValuesAndVariantsAsync(Guid id)
    {
        return _context.Products
            .Include(p => p.AttributeValues)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product?>> GetByCategoryIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.AttributeValues)
            .Where(p => p.CategoryId == id)
            .ToListAsync();
    }
}
