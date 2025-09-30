using Microsoft.Extensions.Options;
using RentACar.Application.Shared.Settings;
using System.Net.Mail;
using System.Net;
using RentACar.Application.Abstracts.Services;

namespace RentACar.Infrastructure.Services;

public class EmailService : IEmailService
{
    private  EmailSettings _emailSettings {  get;  }

    public EmailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body)
    {
        using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var email in toEmails.Distinct()) 
        {

            message.To.Add(email.Trim());
        }

        await smtp.SendMailAsync(message);
    }
}
