using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class VariantAttributeRepository : BaseRepository<VariantAttribute>, IVariantAttributeRepository
{
    private new readonly DearHomeContext _context;
    public VariantAttributeRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }
}
