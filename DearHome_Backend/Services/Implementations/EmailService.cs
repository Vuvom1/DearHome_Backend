using System;
using Microsoft.AspNetCore.Identity;
using Azure.Communication.Email;
using DearHome_Backend.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace DearHome_Backend.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string? _smtpHost;
    private readonly int _smtpPort;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;
    private readonly string? _fromEmail;
    private readonly string? _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        _smtpHost = smtpSettings["Host"];
        _smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
        _smtpUsername = smtpSettings["Username"];
        _smtpPassword = smtpSettings["Password"];
        _fromEmail = smtpSettings["FromEmail"];
        _fromName = smtpSettings["FromName"];
        _enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");
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
        if (string.IsNullOrEmpty(_smtpHost))
        {
            throw new InvalidOperationException("SMTP host is not configured.");
        }
        if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
        {
            throw new InvalidOperationException("SMTP credentials are not configured.");
        }
        if (string.IsNullOrEmpty(_fromEmail))
        {
            throw new InvalidOperationException("Sender email is not configured.");
        }

        try
        {
            var smtpClient = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                EnableSsl = _enableSsl,
                UseDefaultCredentials = false, 
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName ?? string.Empty),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    
}
