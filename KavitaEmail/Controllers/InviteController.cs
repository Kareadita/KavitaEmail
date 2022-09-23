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
    private readonly IValidationService _validationService;

    public InviteController(ILogger<InviteController> logger, IEmailService emailService, IValidationService validationService)
    {
        _logger = logger;
        _emailService = emailService;
        _validationService = validationService;
    }

    [HttpPost("confirm")]
    public async Task<ActionResult> SendConfirmationEmail(ConfirmationEmailDto dto)
    {
        if (!await _validationService.ValidateInstall(dto.InstallId))
        {
            _logger.LogError("An installID {InstallId} was not valid, request rejected for confirmation email", dto.InstallId);
            return BadRequest("Not valid");
        }
        await _emailService.SendEmailForEmailConfirmation(dto);
        return Ok();
    }
    
    [HttpPost("email-migration")]
    public async Task<ActionResult> SendEmailMigrationEmail(EmailMigrationDto dto)
    {
        if (!await _validationService.ValidateInstall(dto.InstallId))
        {
            _logger.LogInformation("InstallId {InstallId} could not be validated against Stat service. Send Email for Migration rejected", dto.EmailAddress);
            return BadRequest("Not valid");
        }
        await _emailService.SendEmailMigrationEmail(dto);
        return Ok();
    }
    
    [HttpPost("email-password-reset")]
    public async Task<ActionResult> SendPasswordResetConfirmation(PasswordResetDto dto)
    {
        if (!await _validationService.ValidateInstall(dto.InstallId))
        {
            _logger.LogInformation("InstallId {InstallId} could not be validated against Stat service. Send Password Reset confirmation rejected", dto.EmailAddress);
            return BadRequest("Not valid");
        }
        _logger.LogInformation("Email Password Reset called");
        await _emailService.SendPasswordResetEmail(dto);
        return Ok();
    }
}
