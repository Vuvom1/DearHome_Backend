using System;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    private new readonly DearHomeContext _context;

    public CategoryRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public Task<Category> GetCategoryByName(string name)
    {
        var categoryTask = _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        return categoryTask.ContinueWith(task =>
        {
            var category = task.Result;
            if (category == null)
            {
                throw new InvalidOperationException($"Category with name '{name}' not found.");
            }
            return category;
        });
    }

    public Task<List<Category>> GetCategoriesByParentId(Guid parentId)
    {
        return _context.Categories.Where(c => c.ParentCategoryId == parentId).ToListAsync();
    }

    public Task<List<Category>> GetAllWithParentAndAttributes()
    {
        return _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.CategoryAttributes!)
                .ThenInclude(ca => ca.Attribute!)
            .ToListAsync();
    }

    public Task<List<Category>> GetAllWithAttributesAndAttributeValues()
    {
        return _context.Categories
            .Include(c => c.CategoryAttributes!)
                .ThenInclude(ca => ca.Attribute!)
                .ThenInclude(a => a.AttributeValues!)
            .Include(c => c.CategoryAttributes!)
            .ToListAsync();
    }

    public async Task<Category> GetCategoryBySlug(string slug)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.CategoryAttributes!)
                .ThenInclude(ca => ca.Attribute!)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category == null)
        {
            throw new InvalidOperationException($"Category with slug '{slug}' not found.");
        }

        return category;
    }

    public async Task<IEnumerable<Category>> GetAllParentCategories()
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .Include(c => c.CategoryAttributes!)
                .ThenInclude(ca => ca.Attribute!)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoryWithStockAndPercentage>> GetCategoriesWithTotalStockAndPercentageAsync()
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories!)
                .ThenInclude(sc => sc.Products!)
                .ThenInclude(p => p.Variants!)
            .Select(c => new CategoryWithStockAndPercentage
            {
                Id = c.Id,
                CategoryName = c.Name,
                // Parent stock amount is the sum of all subcategories' stock amounts
                StockAmount = c.SubCategories!.Sum(sc => sc.Products!.Sum(p => p.Variants!.Sum(v => v.Stock))),
                // Parent percentage is calculated from the total of subcategories
                Percentage = Math.Round(c.SubCategories!.Sum(sc => sc.Products!.Sum(p => p.Variants!.Sum(v => v.Stock))) / (decimal)_context.Products.Sum(p => p.Variants!.Sum(v => v.Stock)) * 100, 2),
                SubCategories = c.SubCategories!.Select(sc => new SubCategoryStock
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    StockAmount = sc.Products!.Sum(p => p.Variants!.Sum(v => v.Stock)),
                    Percentage = Math.Round(sc.Products!.Sum(p => p.Variants!.Sum(v => v.Stock)) / (decimal)_context.Products.Sum(p => p.Variants!.Sum(v => v.Stock)) * 100, 2)
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<string> GetCategoryNameByIdAsync(Guid id)
    {
    return await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => c.Name)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Category with ID '{id}' not found.");
    }
}
