using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private readonly DearHomeContext _context;
    public OrderRepository(DearHomeContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
}
