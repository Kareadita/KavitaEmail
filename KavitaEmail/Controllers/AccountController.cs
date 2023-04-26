using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeleton.DTOs;
using Skeleton.Services;

namespace Skeleton.Controllers;

public class AccountController : BaseApiController
{
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailService _emailService;

    public AccountController(ILogger<AccountController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    [HttpPost("email-change")]
    public async Task<ActionResult> SendConfirmationEmailForEmail(ConfirmationEmailDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[email-change] Request came in from {InstallId} on version {Version}", installId, version);

        try
        {
            await _emailService.SendEmailForEmailConfirmation(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an exception when sending an email for email change");
            return BadRequest();
        }
        
        return Ok();
    }
}