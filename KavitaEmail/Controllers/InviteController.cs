using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeleton.DTOs;
using Skeleton.Misc;
using Skeleton.Services;

namespace Skeleton.Controllers;

/// <summary>
/// Responsible for Invite email flows
/// </summary>
public class InviteController : BaseApiController
{
    private readonly ILogger<InviteController> _logger;
    private readonly IEmailService _emailService;

    public InviteController(ILogger<InviteController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// To invite a user to Kavita
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("confirm")]
    public async Task<ActionResult> SendConfirmationEmail(ConfirmationEmailDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[invite user] Request came in from {InstallId} on version {Version}", installId, version);
        await _emailService.SendEmailForEmailConfirmation(dto);
        return Ok();
    }
    
    /// <summary>
    /// This is used to resend an invite link to the Kavita instance
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("email-migration")]
    public async Task<ActionResult> SendEmailMigrationEmail(EmailMigrationDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[resend-invite] Request came in from {InstallId} on version {Version}", installId, version);
        await _emailService.SendEmailMigrationEmail(dto);
        return Ok();
    }
    
    [HttpPost("email-password-reset")]
    public async Task<ActionResult> SendPasswordResetConfirmation(PasswordResetDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[password-reset] Request came in from {InstallId} on version {Version}", installId, version);
        _logger.LogInformation("[password-reset] Email Password Reset called for {InstallId}", installId);
        await _emailService.SendPasswordResetEmail(dto);
        return Ok();
    }
}
