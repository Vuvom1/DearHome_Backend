using System;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.ReviewDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    private new readonly DearHomeContext _context;

    public ReviewRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<IEnumerable<Review>>> GetByProductIdAsync(Guid productId, int pageNumber, int pageSize, string sortBy, bool isDescending)
    {
        var query = _context.Reviews
            .Where(r => r.OrderDetail!.Variant!.ProductId == productId)
            .AsQueryable();

        if (sortBy == "createdAt")
        {
            query = isDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt);
        }
        else if (sortBy == "rating")
        {
            query = isDescending ? query.OrderByDescending(r => r.Rating) : query.OrderBy(r => r.Rating);
        }

        var totalCount = await query.CountAsync();

        var reviews = await query
            .Include(r => r.OrderDetail)
                .ThenInclude(od => od!.Variant)
                .ThenInclude(v => v!.Product)
            .Include(r => r.OrderDetail)
                .ThenInclude(od => od!.Order)
                .ThenInclude(o => o!.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedResponse = new PaginatedResponse<IEnumerable<Review>>(
            reviews,
            pageNumber,
            pageSize,
            totalCount
        );

        return paginatedResponse;
    }

    public async Task<IEnumerable<Review?>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Reviews
            .Include(r => r.OrderDetail)
                .ThenInclude(od => od!.Variant)
                .ThenInclude(v => v!.Product)
            .Where(r => r.OrderDetailId == orderId)
            .ToListAsync();
    }
}
