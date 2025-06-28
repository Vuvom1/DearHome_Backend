using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Data;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.StatsDtos;
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

    public override async Task<PaginatedResult<Order>> GetAllAsync(int offSet, int limit, string? search = null)
    {
        IQueryable<Order> query = _context.Orders;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(o => (o.User != null && o.User.Name != null && o.User.Name.Contains(search)) || 
                                     (o.User != null && o.User.Email != null && o.User.Email.Contains(search)));
        }

        query = query.Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
            .Include(o => o.User);

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

        var orders = await query
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();

        return new PaginatedResult<Order>(orders, offSet / limit + 1, limit, totalRecords);
    }
    

    public override async Task<PaginatedResult<Order>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false)
    {
        IQueryable<Order> query = _context.Orders;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(o => (o.User != null && o.User.Name != null && o.User.Name.Contains(search)) || 
                                     (o.User != null && o.User.Email != null && o.User.Email.Contains(search)));
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = filter switch
            {
                "placed" => query.Where(o => o.Status == OrderStatus.Placed),
                "waitForPayment" => query.Where(o => o.Status == OrderStatus.WaitForPayment),
                "processing" => query.Where(o => o.Status == OrderStatus.Processing),
                "shipping" => query.Where(o => o.Status == OrderStatus.Shipping),
                "completed" => query.Where(o => o.Status == OrderStatus.Completed),
                "delivered" => query.Where(o => o.Status == OrderStatus.Delivered),
                "cancelled" => query.Where(o => o.Status == OrderStatus.Cancelled),
                _ => query
            };
        }

        // Apply sorting before includes to avoid EF translation issues
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "id" => isDescending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),
                "createdat" => isDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
                "updatedat" => isDescending ? query.OrderByDescending(o => o.UpdatedAt) : query.OrderBy(o => o.UpdatedAt),
                "orderdate" => isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                "status" => isDescending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                "totalprice" => isDescending ? query.OrderByDescending(o => o.TotalPrice) : query.OrderBy(o => o.TotalPrice),
                "finalprice" => isDescending ? query.OrderByDescending(o => o.FinalPrice) : query.OrderBy(o => o.FinalPrice),
                "discount" => isDescending ? query.OrderByDescending(o => o.Discount) : query.OrderBy(o => o.Discount),
                "paymentordercode" => isDescending ? query.OrderByDescending(o => o.PaymentOrderCode) : query.OrderBy(o => o.PaymentOrderCode),
                _ => query.OrderByDescending(o => o.CreatedAt) // Default sorting
            };
        }
        else
        {
            // Default sorting if no sortBy is provided
            query = query.OrderByDescending(o => o.CreatedAt);
        }

        query = query.Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
            .Include(o => o.User);

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / limit);

        var orders = await query
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();

        return new PaginatedResult<Order>(orders, offSet / limit + 1, limit, totalRecords);
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

    public override async Task<Order?> GetByIdAsync(object id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Variant)
                .ThenInclude(v => v!.Product)
                .ThenInclude(p => p!.Category)
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Reviews)
            .Include(o => o.User)
            .Include(o => o.Address)
            .Include(o => o.Promotion)
            .FirstOrDefaultAsync(o => o.Id == (Guid)id);

        return order;
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetDailySalesAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == OrderStatus.Completed)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new KeyValuePair<string, decimal>(g.Key.ToString(), g.Sum(o => o.TotalPrice - o.Discount)))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetMonthlySalesAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == OrderStatus.Completed)
            .GroupBy(o =>  new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new KeyValuePair<string, decimal>($"{g.Key.Year}-{g.Key.Month}", g.Sum(o => o.TotalPrice - o.Discount)))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetYearlySalesAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == OrderStatus.Completed)
            .GroupBy(o => o.CreatedAt.Year)
            .Select(g => new KeyValuePair<string, decimal>(g.Key.ToString(), g.Sum(o => o.TotalPrice - o.Discount)))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetDailyOrderCountsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new KeyValuePair<string, decimal>(g.Key.ToString(), g.Count()))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetMonthlyOrderCountsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new KeyValuePair<string, decimal>($"{g.Key.Year}-{g.Key.Month}", g.Count()))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetYearlyOrderCountsAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .GroupBy(o => o.CreatedAt.Year)
            .Select(g => new KeyValuePair<string, decimal>(g.Key.ToString(), g.Count()))
            .ToListAsync();
    }

    public Task<int> GetTotalOrdersCountAsync()
    {
        return _context.Orders.CountAsync();
    }

    public Task<decimal> GetTotalSalesAsync()
    {
        return _context.Orders
            .Where(o => o.Status == OrderStatus.Completed)
            .SumAsync(o => o.FinalPrice);
    }

    public Task<KeyValuePair<OrderStatus, decimal>> GetOrderStatusPercentageAsync(OrderStatus status, DateTime startDate, DateTime endDate)
    {
        return _context.Orders
            .Where(o => o.Status == status && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .GroupBy(o => o.Status)
            .Select(g => new KeyValuePair<OrderStatus, decimal>(g.Key, (decimal)g.Count() / _context.Orders.Count(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate) * 100))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<OrderStatusPercentage>> GetOrderStatusPercentageAsync()
    {
        return await _context.Orders
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusPercentage
            {
                Status = g.Key,
                Percentage = (decimal)g.Count() / _context.Orders.Count() * 100
            })
            .ToListAsync();
    }
}
