namespace Skeleton.DTOs;

public class SmtpConfig
{
    public static string Key { get; } = "SMTP";
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public bool EnableSsl { get; set; } = true;
    public bool UseDefaultCredentials { get; set; } = false;
    public bool IsBodyHtml { get; set; } = true;
    /// <summary>
    /// If enabled, this allows Kavita to upload files (temporarily) and send the files as attachments
    /// </summary>
    public bool AllowSendTo { get; set; }
}