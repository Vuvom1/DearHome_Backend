using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class AttributeValueRepository : BaseRepository<AttributeValue>, IAttributeValueRepository
{
    private new readonly DearHomeContext _context;
    public AttributeValueRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }
}
