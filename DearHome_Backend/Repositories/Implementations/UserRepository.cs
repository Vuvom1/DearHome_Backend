using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private new readonly DearHomeContext _context;
    public UserRepository(DearHomeContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<User?> GetByIdAsync(object id)
    {
        return await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Payments)
            .FirstOrDefaultAsync(u => u.Id == (Guid)id);
    }

    public Task<bool> IsUserNameExistsAsync(string userName)
    {
        var userExists = _context.Users.Any(u => u.UserName == userName);
        return Task.FromResult(userExists);
    }

    public Task<bool> IsEmailExistsAsync(string email)
    {
        var emailExists = _context.Users.Any(u => u.Email == email);
        return Task.FromResult(emailExists);
    }

    public Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
    {
        var phoneNumberExists = _context.Users.Any(u => u.PhoneNumber == phoneNumber);
        return Task.FromResult(phoneNumberExists);
    }

    public override async Task UpdateAsync(User entity)
    {
        var existingUser = await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Payments)
            .FirstOrDefaultAsync(u => u.Id == entity.Id);

        if (existingUser != null)
        {
            _context.Entry(existingUser).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }

    public Task<User?> GetByIdWithAddressesAsync(Guid id)
    {
        return _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == (Guid)id);
    }
}
