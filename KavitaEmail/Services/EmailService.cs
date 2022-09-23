using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Skeleton.DTOs;

namespace Skeleton.Services;

public interface IEmailService
{
    Task SendEmailForEmailConfirmation(ConfirmationEmailDto userEmailOptions);
    Task SendEmailMigrationEmail(EmailMigrationDto dto);
    Task SendPasswordResetEmail(PasswordResetDto dto);
    Task SendToDevice(string emailAddress, IList<Attachment> attachments);
}

public class EmailService : IEmailService
{
    private const string TemplatePath = @"{0}.html";
    private readonly SmtpConfig _smtpConfig;

    public EmailService(IOptions<SmtpConfig> smtpConfig)
    {
        _smtpConfig = smtpConfig.Value;
    }
    
    public async Task SendEmailForEmailConfirmation(ConfirmationEmailDto dto)
    {
        var placeholders = new List<KeyValuePair<string, string>>
        {
            new ("{{InvitingUser}}", dto.InvitingUser),
            new ("{{Link}}", dto.ServerConfirmationLink)
        };

        var emailOptions = new EmailOptionsDto()
        {
            Subject = UpdatePlaceHolders("You've been invited to join {{InvitingUser}}'s Server", placeholders),
            Body = UpdatePlaceHolders(GetEmailBody("EmailConfirm"), placeholders),
            ToEmails = new List<string>()
            {
                dto.EmailAddress
            }
        };

        await SendEmail(emailOptions);
    }

    public async Task SendEmailMigrationEmail(EmailMigrationDto dto)
    {
        var placeholders = new List<KeyValuePair<string, string>>
        {
            new ("{{Link}}", dto.ServerConfirmationLink),
            new ("{{User}}", dto.Username),
        };

        var emailOptions = new EmailOptionsDto()
        {
            Subject = UpdatePlaceHolders("Please validate your email to complete email migration", placeholders),
            Body = UpdatePlaceHolders(GetEmailBody("EmailMigration"), placeholders),
            ToEmails = new List<string>()
            {
                dto.EmailAddress
            }
        };

        await SendEmail(emailOptions);
    }

    public async Task SendPasswordResetEmail(PasswordResetDto dto)
    {
        var placeholders = new List<KeyValuePair<string, string>>
        {
            new ("{{Link}}", dto.ServerConfirmationLink),
        };

        var emailOptions = new EmailOptionsDto()
        {
            Subject = UpdatePlaceHolders("A password reset has been requested", placeholders),
            Body = UpdatePlaceHolders(GetEmailBody("EmailPasswordReset"), placeholders),
            ToEmails = new List<string>()
            {
                dto.EmailAddress
            }
        };

        await SendEmail(emailOptions);
    }

    public async Task SendToDevice(string emailAddress, IList<Attachment> attachments)
    {
        
        var emailOptions = new EmailOptionsDto()
        {
            Subject = "Send file from Kavita",
            ToEmails = new List<string>()
            {
                emailAddress
            },
            Attachments = attachments
        };

        await SendEmail(emailOptions);
    }

    private async Task SendEmail(EmailOptionsDto userEmailOptions)
    {
        using var mail = new MailMessage
        {
            Subject = userEmailOptions.Subject,
            Body = userEmailOptions.Body,
            From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
            IsBodyHtml = _smtpConfig.IsBodyHtml,
            BodyEncoding = Encoding.Default,
        };

        if (userEmailOptions.Attachments != null)
        {
            foreach (var attachment in userEmailOptions.Attachments)
            {
                mail.Attachments.Add(attachment);
            }
        }
        
        foreach (var toEmail in userEmailOptions.ToEmails)
        {
            mail.To.Add(toEmail);
        }
        
        
        using var smtpClient = new SmtpClient
        {
            Host = _smtpConfig.Host,
            Port = _smtpConfig.Port,
            EnableSsl = _smtpConfig.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
            Credentials = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password),
            Timeout = 20000
        };

        await smtpClient.SendMailAsync(mail);
    }

    private static string GetEmailBody(string templateName)
    {
        var templateDirectory = Path.Join(Directory.GetCurrentDirectory(), "config", "templates", TemplatePath);
        var body = File.ReadAllText(string.Format(templateDirectory, templateName));
        return body;
    }

    private static string UpdatePlaceHolders(string text, IList<KeyValuePair<string, string>> keyValuePairs)
    {
        if (string.IsNullOrEmpty(text) || keyValuePairs == null) return text;
        
        foreach (var (key, value) in keyValuePairs)
        {
            if (text.Contains(key))
            {
                text = text.Replace(key, value);
            }
        }

        return text;
    }

}