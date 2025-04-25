using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IVerificationCodeRepository : IBaseRepository<VerificationCode>
{
    public Task<VerificationCode?> GetVerificationCodeByEmailAsync(string email);
}
