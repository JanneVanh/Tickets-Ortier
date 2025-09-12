namespace Core.Interfaces;

public interface IEmailService
{
    Task SendEmail(string subject, string body, string toEmail);
}
