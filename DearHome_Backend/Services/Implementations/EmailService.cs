using System;
using Microsoft.AspNetCore.Identity;
using Azure.Communication.Email;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    public Task SendConfirmationLinkAsync(IdentityUser<Guid> user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetCodeAsync(IdentityUser<Guid> user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(IdentityUser<Guid> user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    public Task SendVerificationCodeAsync(string email, string verificationCode)
    {
        var subject = "Verification Code";
        var message = $"Your verification code is: {verificationCode}";
        return SendEmailAsync(email, subject, message);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var azureCommSettings = _configuration.GetSection("AzureCommunication");
        var connectionString = azureCommSettings["ConnectionString"];
        var fromEmail = azureCommSettings["FromEmail"];
        var fromName = azureCommSettings["FromName"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Communication Service connection string is not configured.");
        }

        if (string.IsNullOrEmpty(fromEmail))
        {
            throw new InvalidOperationException("FromEmail is not configured.");
        }

        if (string.IsNullOrEmpty(fromName))
        {
            throw new InvalidOperationException("FromName is not configured.");
        }

        try
        {
            var emailClient = new EmailClient(connectionString);

            var emailContent = new EmailContent(subject)
            {
                PlainText = message,
                Html = message
            };

            var emailMessage = new EmailMessage(fromEmail, toEmail, emailContent);

            var sendResult = await emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);

            if (!sendResult.HasCompleted)
            {
                throw new InvalidOperationException("Failed to send email.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    
}
