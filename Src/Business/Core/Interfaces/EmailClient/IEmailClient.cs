namespace ShareMarket.Core.Interfaces.Emails.EmailClient;
public interface IEmailClient
{
    Task<bool> Send(string message, string subject);
    Task<bool> Send(string message, string subject, string[] toEmails, string[]? attachments = null);
    Task<bool> Send(string message, string subject, string[] toEmails, string headerText, string[]? attachments = null);
    Task<bool> Send(string message, string subject, string[] toEmails, string[] cCEmail, string[]? attachments = null);
    Task<bool> Send(string message, string subject, string[] toEmails, string[] cCEmail, string headerText, string[]? attachments = null);
}
