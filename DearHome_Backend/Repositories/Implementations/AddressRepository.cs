using System;
using DearHome_Backend.Data;
using DearHome_Backend.Modals;
using DearHome_Backend.Repositories.Interfaces;

namespace DearHome_Backend.Repositories.Implementations;

public class AddressRepository : BaseRepository<Address>, IAddressRepository
{
    private new readonly DearHomeContext _context;
    public AddressRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }
}
