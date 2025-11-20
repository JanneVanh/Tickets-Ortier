using System.Net;
using System.Net.Mail;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _fromEmail;
    private readonly string _password;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _fromEmail = _configuration["Email:fromEmail"] ?? throw new ArgumentException(nameof(_fromEmail));
        _password = _configuration["Email:password"] ?? throw new ArgumentNullException(nameof(_password));
    }

    public async Task SendEmail(string subject, string body, string toEmail)
    {
        await SendEmailWithAttachments(subject, body, toEmail, null);
    }

    public async Task SendEmailWithAttachments(string subject, string body, string toEmail, List<(byte[] content, string fileName)>? attachments = null)
    {

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(_fromEmail, _password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body)
        {
            IsBodyHtml = true
        };

        // Add PDF attachments
        if (attachments != null)
        {
            foreach (var (content, fileName) in attachments)
            {
                var attachment = new Attachment(new MemoryStream(content), fileName, "application/pdf");
                mailMessage.Attachments.Add(attachment);
            }
        }

        await client.SendMailAsync(mailMessage);
    }
}

