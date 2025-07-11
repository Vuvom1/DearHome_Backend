using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByIdWithAddressesAsync(Guid id);
    Task<IEnumerable<User>> GetAllCustomersAsync(int offSet, int limit);
    Task<int> GetTotalCustomersCountAsync();
    Task<bool> IsUserNameExistsAsync(string userName);
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
    Task ChangePasswordAsync(Guid userId, string newPassword);
    Task<string> GetUserHashPasswordAsync(Guid userId);
}
