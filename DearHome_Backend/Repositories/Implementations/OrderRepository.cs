using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Data;
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
}
