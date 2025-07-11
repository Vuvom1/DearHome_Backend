using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private new readonly DearHomeContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(
        DearHomeContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<UserRepository> logger
    ) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
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

    public async Task<IEnumerable<User>> GetAllCustomersAsync(int offSet, int limit)
    {
        var roleExists = await _roleManager.RoleExistsAsync(UserRole.User.ToString());
        if (!roleExists)
        {
            _logger.LogWarning("Role {Role} does not exist.", UserRole.User.ToString());
            return Enumerable.Empty<User>();
        }
        var users = await _userManager.GetUsersInRoleAsync(UserRole.User.ToString());
        var userIds = users.Select(u => u.Id).ToList();

        return await _context.Users
            .Include(u => u.Addresses)
            .Where(u => userIds.Contains(u.Id))
            .Skip(offSet)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetTotalCustomersCountAsync()
    {
        var roleExists = await _roleManager.RoleExistsAsync(UserRole.User.ToString());
        if (!roleExists)
        {
            _logger.LogWarning("Role {Role} does not exist.", UserRole.User.ToString());
            return 0;
        }
        var users = await _userManager.GetUsersInRoleAsync(UserRole.User.ToString());
        return users.Count;
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found.", nameof(userId));
        }
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
        await _userManager.UpdateAsync(user);
    }

    public async Task<string> GetUserHashPasswordAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.PasswordHash ?? throw new InvalidOperationException($"User with ID '{userId}' does not have a password hash.");
    }
}
