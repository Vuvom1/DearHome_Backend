using System;
using Microsoft.AspNetCore.Identity;

namespace DearHome_Backend.Services.Interfaces;

public interface IEmailService : IEmailSender<IdentityUser<Guid>>
{
    Task SendVerificationCodeAsync(string email, string verificationCode);
}
