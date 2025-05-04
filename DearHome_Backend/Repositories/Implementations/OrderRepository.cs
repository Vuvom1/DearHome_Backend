using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private new readonly DearHomeContext _context;
    public OrderRepository(DearHomeContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task UpdateOrderStatusByPaymentOrderCodeAsync(long paymentOrderCode, OrderStatus status)
    {
        return _context.Orders
            .Where(o => o.PaymentOrderCode == paymentOrderCode)
            .ExecuteUpdateAsync(o => o.SetProperty(x => x.Status, status));
    }

    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync(int offSet, int limit)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .Include(o => o.User)
            .Include(o => o.Address)
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpdateOrderStatusByIdAsync(Guid orderId, OrderStatus status)
    {
        await _context.Orders
            .Where(o => o.Id == orderId)
            .ExecuteUpdateAsync(o => o.SetProperty(x => x.Status, status));
    }

    public async Task<bool> IsPromotionUsedByUserId(Guid userId, Guid promotionId)
    {
        var isPromotionUsed = await _context.Orders
            .AnyAsync(o => o.UserId == userId && o.PromotionId == promotionId);

        return isPromotionUsed;
    }

    public async Task<IEnumerable<Guid>> GetUsedPromotionIdsByUserId(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId && o.PromotionId.HasValue)
            .Select(o => o.PromotionId!.Value)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int offSet, int limit)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .Include(o => o.User)
            .Include(o => o.Address)
            .Where(o => o.UserId == userId)
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAndStatusAsync(Guid userId, OrderStatus status, int offSet, int limit)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .Include(o => o.User)
            .Include(o => o.Address)
            .Include(o => o.Promotion)
            .Where(o => o.UserId == userId && o.Status == status)
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<IEnumerable<Order>>> GetAllAsync(int pageNumber, int pageSize, string? searchString = null)
    {

        IQueryable<Order> query = _context.Orders;

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(o => (o.User != null && o.User.Name != null && o.User.Name.Contains(searchString)) || 
                                     (o.User != null && o.User.Email != null && o.User.Email.Contains(searchString)));
        }

        query = query.Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .Include(o => o.User)
            .Include(o => o.Address);

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var orders = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<IEnumerable<Order>>(orders, pageNumber, pageSize, totalRecords);
    }
}
