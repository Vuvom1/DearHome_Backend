using System;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.PaginationDtos;
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
            .Include(p => p.Placement)
            .ToListAsync();
    }

    public override async Task<PaginatedResult<Product>> GetAllAsync(int offSet, int limit, string? search = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        return new PaginatedResult<Product>
        {
            Data = await query
                .Include(p => p.Category)
                .Include(p => p.Placement)
                .Skip(offSet)
                .Take(limit)
                .ToListAsync(),
            PageNumber = offSet / limit + 1,
            PageSize = limit,
            TotalRecords = await query.CountAsync()
        };
    }

    public override async Task<PaginatedResult<Product>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false)
    {
        var query = _context.Products.AsQueryable();

        query = query
            .Include(p => p.Category)
            .Include(p => p.Placement);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = filter switch
            {
                "active" => query.Where(p => p.IsActive),
                "inactive" => query.Where(p => !p.IsActive),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy switch
            {
                "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "createdAt" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "updatedAt" => isDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
                "category" => isDescending ? query.OrderByDescending(p => p.Category.Name) : query.OrderBy(p => p.Category.Name),
                _ => query
            };
        }

        return new PaginatedResult<Product>
        {
            Data = await query
                .Include(p => p.Category)
                .Include(p => p.Placement)
                .Skip(offSet)
                .Take(limit)
                .ToListAsync(),
            PageNumber = offSet / limit + 1,
            PageSize = limit,
            TotalRecords = await query.CountAsync()
        };
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
            .Include(p => p.Category)
             .ThenInclude(c => c!.CategoryAttributes!)
             .ThenInclude(ca => ca.Attribute)
            .Include(p => p.Placement)
            .Include(p => p.Variants!)
                .ThenInclude(v => v.VariantAttributes!)
                .ThenInclude(v => v.AttributeValue)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product?>> GetByCategoryIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.AttributeValues)
            .Where(p => p.CategoryId == id)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetTopSalesProductsAsync(int count)
    {
        var topSaleProductIds = await _context.OrderDetails
            .Where(od => od.Variant != null && od.Variant.Product != null)
            .GroupBy(od => od.Variant!.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSales = g.Sum(od => od.Quantity)
            })
            .Where(g => g.TotalSales > 0)
            .OrderByDescending(g => g.TotalSales)
            .Take(count)
            .Select(g => g.ProductId)  // Extract just the IDs
            .ToListAsync();

        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Placement)
            .Where(p => topSaleProductIds.Contains(p.Id))  // Use Contains instead of Any
            .ToListAsync();
    }

    public async Task<int> GetTotalProductsCountAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<IEnumerable<KeyValuePair<Product, decimal>>> GetTopSalesProductsWithAmountsAsync(DateTime startDate, DateTime endDate, int count)
    {
        var results = await _context.OrderDetails
            .Where(od => od.Variant != null && od.Order != null && od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
            .GroupBy(od => od.Variant!.Product)
            .Select(g => new
            {
                Product = g.Key!,
                TotalAmount = g.Sum(od => od.TotalPrice)
            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(count)
            .ToListAsync();

        return results.Select(r => new KeyValuePair<Product, decimal>(r.Product, r.TotalAmount));
    }

    public async Task<IEnumerable<KeyValuePair<Product, int>>> GetTopSalesProductsWithCountsAsync(DateTime startDate, DateTime endDate, int count)
    {
        var results = await _context.OrderDetails
            .Where(od => od.Variant != null && od.Order != null && od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
            .GroupBy(od => od.Variant!.Product)
            .Select(g => new
            {
                Product = g.Key!,
                TotalCount = g.Sum(od => od.Quantity)
            })
            .OrderByDescending(x => x.TotalCount)
            .Take(count)
            .ToListAsync();

        return results.Select(r => new KeyValuePair<Product, int>(r.Product, r.TotalCount));
    }
}
