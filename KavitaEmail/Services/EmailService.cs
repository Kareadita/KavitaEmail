using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Skeleton.DTOs;

namespace Skeleton.Services;

public interface IEmailService
{
    Task SendEmailForEmailConfirmation(ConfirmationEmailDto userEmailOptions);
    Task SendEmailForEmailChange(ConfirmationEmailDto userEmailOptions);
    Task SendEmailMigrationEmail(EmailMigrationDto dto);
    Task SendPasswordResetEmail(PasswordResetDto dto);
    Task SendToDevice(string emailAddress, IList<string> attachments);
    Task SendTestEmail(string adminEmail);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private const string TemplatePath = @"{0}.html";
    private readonly SmtpConfig _smtpConfig;

    public EmailService(IOptions<SmtpConfig> smtpConfig, ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpConfig = smtpConfig.Value;
    }
    
    public async Task SendEmailForEmailConfirmation(ConfirmationEmailDto userEmailOptions)
    {
        var placeholders = new List<KeyValuePair<string, string>>
        {
            new ("{{InvitingUser}}", userEmailOptions.InvitingUser),
            new ("{{Link}}", userEmailOptions.ServerConfirmationLink)
        };

        var emailOptions = new EmailOptionsDto()
        {
            Subject = UpdatePlaceHolders("You've been invited to join {{InvitingUser}}'s Server", placeholders),
            Body = UpdatePlaceHolders(GetEmailBody("EmailConfirm"), placeholders),
            ToEmails = new List<string>()
            {
                userEmailOptions.EmailAddress
            }
        };

        await SendEmail(emailOptions);
    }
    
    
    public async Task SendEmailForEmailChange(ConfirmationEmailDto userEmailOptions)
    {
        
        var placeholders = new List<KeyValuePair<string, string>>
        {
            new ("{{InvitingUser}}", userEmailOptions.InvitingUser),
            new ("{{Link}}", userEmailOptions.ServerConfirmationLink)
        };

        var emailOptions = new EmailOptionsDto()
        {
            Subject = UpdatePlaceHolders("Your email has been changed on {{InvitingUser}}'s Server", placeholders),
            Body = UpdatePlaceHolders(GetEmailBody("EmailChange"), placeholders),
            ToEmails = new List<string>()
            {
                userEmailOptions.EmailAddress
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

    public async Task SendToDevice(string emailAddress, IList<string> attachments)
    {
        
        var emailOptions = new EmailOptionsDto()
        {
            Subject = "Send file from Kavita",
            ToEmails = new List<string>()
            {
                emailAddress
            },
            Body = GetEmailBody("SendToDevice"),
            Attachments = attachments
        };

        await SendEmail(emailOptions);
    }

    public async Task SendTestEmail(string adminEmail)
    {
        var emailOptions = new EmailOptionsDto()
        {
            Subject = "KavitaEmail Test",
            Body = GetEmailBody("EmailTest"),
            ToEmails = new List<string>()
            {
                adminEmail
            }
        };

        await SendEmail(emailOptions);
    }

    private async Task SendEmail(EmailOptionsDto userEmailOptions)
    {
        var email = new MimeMessage()
        {
            Subject = userEmailOptions.Subject,
        };
        email.From.Add(new MailboxAddress(_smtpConfig.SenderDisplayName, _smtpConfig.SenderAddress));


        var body = new BodyBuilder
        {
            HtmlBody = userEmailOptions.Body
        };

        if (userEmailOptions.Attachments != null)
        {
            foreach (var attachment in userEmailOptions.Attachments)
            {
                await body.Attachments.AddAsync(attachment);
            }
        }

        email.Body = body.ToMessageBody();
        
        foreach (var toEmail in userEmailOptions.ToEmails)
        {
            email.To.Add(new MailboxAddress(toEmail, toEmail));
        }

        using var smtpClient = new MailKit.Net.Smtp.SmtpClient
        {
            Timeout = 20000
        };
      
        await smtpClient.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port);
        if (!string.IsNullOrEmpty(_smtpConfig.UserName) && !string.IsNullOrEmpty(_smtpConfig.Password))
        {
            await smtpClient.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password);
        }

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        try
        {
            await smtpClient.SendAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an issue sending the email");
            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
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