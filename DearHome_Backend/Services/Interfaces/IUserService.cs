using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IUserService 
{
    Task<User?> GetUserAsync(Guid id);
    Task<IEnumerable<User>> GetAllCustomersAsync(int offSet, int limit);
    Task<string> LoginAsync(string userName, string password);
    Task<string> LoginWithGoogleAsync(string accessToken);
    Task<User> RegisterAsync(User user, string password, string verificationCode);
    Task SendVerificationCodeAsync(string email);
    Task UpdateAsync(User user);

}
