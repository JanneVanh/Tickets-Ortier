namespace Core.Interfaces;

public interface IEmailService
{
    Task SendEmail(string subject, string body, string toEmail);
    Task SendEmailWithAttachments(string subject, string body, string toEmail, List<(byte[] content, string fileName)>? attachments = null);
}
