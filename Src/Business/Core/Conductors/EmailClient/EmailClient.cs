using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ShareMarket.Core.Entities.Audits;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Conductors;
using ShareMarket.Core.Interfaces.Emails.EmailClient;
using ShareMarket.Core.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ShareMarket.Core.Conductors.EmailClient;

public class EmailClient(
    IRepositoryConductor<EmailAuditLog> EmailAuditRepo,
    EmailConfiguration                  Configuration,
    ILogger<EmailClient> Logger) : IEmailClient
{
    readonly string pattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";

    public async Task<bool> Send(string message, string subject)
    {
        ThrowErrorIfNull(message, subject);
        MailMessage mailMessage = GetMailMessage(message, subject, Configuration.Header);

        mailMessage.To.Add(Configuration.ReplyTo);
        await SendMailMessage(mailMessage);

        return true;
    }

    public async Task<bool> Send(string message, string subject, string[] toEmails, string[]? attachments = null)
    {
        ThrowErrorIfNull(message, subject, toEmails);
        MailMessage mailMessage = GetMailMessage(message, subject, Configuration.Header, attachments);

        bool ValiEmail = false;
        for (int i = 0; i < toEmails.Length; i++)
        {
            if (Regex.IsMatch(toEmails[i], pattern))
            {
                ValiEmail = true;
                mailMessage.To.Add(toEmails[i]);
            }
        }
        if (ValiEmail)
        {
            await SendMailMessage(mailMessage);
        }

        return ValiEmail;
    }
    public async Task<bool> Send(string message, string subject, string[] toEmails, string headerText, string[]? attachments = null)
    {
        ThrowErrorIfNull(message, subject, headerText, toEmails);
        MailMessage mailMessage = GetMailMessage(message, subject, headerText, attachments);

        bool ValiEmail = false;
        #region To Email Address
        for (int i = 0; i < toEmails.Length; i++)
        {
            if (Regex.IsMatch(toEmails[i], pattern))
            {
                ValiEmail = true;
                mailMessage.To.Add(toEmails[i]);
            }
        }
        #endregion

        if (ValiEmail)
        {
            await SendMailMessage(mailMessage);
        }
        return ValiEmail;
    }
    public async Task<bool> Send(string message, string subject, string[] toEmails, string[] cCEmails, string[]? attachments = null)
    {
        ThrowErrorIfNull(message, subject, toEmails, cCEmails);
        MailMessage mailMessage = GetMailMessage(message, subject, Configuration.Header, attachments);

        bool ValiEmail = false;
        #region To Email Address
        for (int i = 0; i < toEmails.Length; i++)
        {
            if (Regex.IsMatch(toEmails[i], pattern))
            {
                ValiEmail = true;
                mailMessage.To.Add(toEmails[i]);
            }
        }
        #endregion
        #region     CC Email Address
        for (int i = 0; i < cCEmails.Length; i++)
        {
            if (Regex.IsMatch(cCEmails[i], pattern))
            {
                mailMessage.To.Add(cCEmails[i]);
            }
        }
        #endregion

        if (ValiEmail)
        {
           await SendMailMessage(mailMessage);
        }
        return ValiEmail;
    }
    public async Task<bool> Send(string message, string subject, string[] toEmails, string[] cCEmails, string headerText, string[]? attachments = null)
    {
        ThrowErrorIfNull(message, subject, headerText, toEmails, cCEmails);
        MailMessage mailMessage = GetMailMessage(message, subject, headerText, attachments);

        bool ValiEmail = false;
        #region To Email Address
        for (int i = 0; i < toEmails.Length; i++)
        {
            if (Regex.IsMatch(toEmails[i], pattern))
            {
                ValiEmail = true;
                mailMessage.To.Add(toEmails[i]);
            }
        }
        #endregion
        #region     CC Email Address
        for (int i = 0; i < cCEmails.Length; i++)
        {
            if (Regex.IsMatch(cCEmails[i], pattern))
            {
                mailMessage.To.Add(cCEmails[i]);
            }
        }
        #endregion
        if (ValiEmail)
        {
           await SendMailMessage(mailMessage);
        }
        return ValiEmail;
    }

    private MailMessage GetMailMessage(string message, string subject, string headerText, string[]? attachments = null)
    {
        MailMessage mailMessage = new()
        {
            From        = new MailAddress(Configuration.From, headerText),
            Subject     = subject,
            Body        = message,
            IsBodyHtml  = true,
            Priority    = MailPriority.High
        };
        if(attachments != null)
        {
            foreach (string attachment in attachments)
            {
                if (File.Exists(attachment))
                    mailMessage.Attachments.Add(new Attachment(attachment));
            }
        }
        mailMessage.ReplyToList.Add(Configuration.ReplyTo);
        return mailMessage;
    }
    private async Task SendMailMessage(MailMessage mailMessage)
    {
        if (!Configuration.SendEmail)
        {
            Logger.LogError("Email sending is not configured, please enable from AppSettings.json");
            return;
        }
        NetworkCredential nc = new(Configuration.UserName, Configuration.Password);
        using SmtpClient sc = new()
        {
            Host        = Configuration.Host,
            Port        = Configuration.Port,
            Credentials = nc,
            EnableSsl   = Configuration.EnableSsl
        };
        EmailAuditLog auditLog = new ()
        {
            Attachments     = mailMessage.Attachments.Count > 0 ? mailMessage.Attachments.Select(s => s.Name ?? string.Empty).Join(",") : null,
            CCEmails        = mailMessage.CC.Count > 0 ? mailMessage.CC.Select(s => s.Address).Join(",") : null,
            HeaderText      = mailMessage.From?.DisplayName ?? string.Empty,
            MessageBody     = mailMessage.Body,
            Subject         = mailMessage.Subject,
            ToEmails        = mailMessage.To.Select(s => s.Address).Join(","),
            Success         = true
        };
        var sent = await EmailAuditRepo.FindAll(e => e.ToEmails   == auditLog.ToEmails
                                         && e.CCEmails      == auditLog.CCEmails
                                         && e.Subject       == auditLog.Subject
                                         && e.Attachments   == auditLog.Attachments
                                         && e.MessageBody   == auditLog.MessageBody)
                                 .ResultObject.AnyAsync();
        if (!sent)
        {
            var auditResult = await EmailAuditRepo.CreateAsync(auditLog, SystemConstant.SystemUserId);
            if (auditResult.HasErrors)
            {
                var error = auditResult.GetErrors();
                Logger.LogCritical("EmailAuditLog create error: {error}", error);
            }

            try
            {
                sc.Send(mailMessage);
            }
            catch (Exception ex)
            {
                auditLog.Error = $"{ex.Message} <br /> {ex.InnerException?.Message}";
                auditLog.Success = false;
                Logger.LogCritical("EmailAuditLog create error: {Message}", ex.Message);
                var auditUpdateResult = await EmailAuditRepo.UpdateAsync(auditLog, SystemConstant.SystemUserId);
                if (auditUpdateResult.HasErrors)
                {
                    var error = auditUpdateResult.GetErrors();
                    Logger.LogCritical("EmailAuditLog update error: {error}", error);
                }
                throw;
            }
            var emails = mailMessage.To.Select(s => s.Address).Join();
            Logger.LogInformation("Email sent successfully to [{emails}] with subject {Subject}", emails, mailMessage.Subject);
        }
    }
    private static void ThrowErrorIfNull(string message, params object[] param)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));
        if (param != null && param.Length > 0)
        {
            for (int i = 0; i < param.Length; i++)
            {
                object p0 = param[i];
                if (string.IsNullOrEmpty(p0.ToString()))
                    throw new ArgumentNullException(nameof(param));
            }
        }
    }
}

