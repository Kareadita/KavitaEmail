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
/// This is only for v0.5.6.17 and below users. Do not remove at least until v0.8.
/// </summary>
public class EmailController : BaseApiController
{
    private readonly ILogger<EmailController> _logger;
    private readonly IEmailService _emailService;
    private readonly IValidationService _validationService;

    public EmailController(ILogger<EmailController> logger, IEmailService emailService, IValidationService validationService)
    {
        _logger = logger;
        _emailService = emailService;
        _validationService = validationService;
    }
    
    /// <summary>
    /// This is used to validate the service is reachable
    /// </summary>
    /// <returns></returns>
    [HttpGet("test")]
    public ActionResult<bool> Test()
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[test] Request came in from {InstallId} on version {Version}", installId, version);
        return Ok(true);
    }

    [HttpPost("confirm")]
    public async Task<ActionResult> SendConfirmationEmail(ConfirmationEmailDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[email-confirm] Request came in from {InstallId} on version {Version}", installId, version);
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
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[email-migration] Request came in from {InstallId} on version {Version}", installId, version);
        if (!await _validationService.ValidateInstall(dto.InstallId)) return BadRequest("Not valid");
        await _emailService.SendEmailMigrationEmail(dto);
        return Ok();
    }
    
    [HttpPost("email-password-reset")]
    public async Task<ActionResult> SendPasswordResetConfirmation(PasswordResetDto dto)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[email-password-reset] Request came in from {InstallId} on version {Version}", installId, version);
        if (!await _validationService.ValidateInstall(dto.InstallId)) return BadRequest("Not valid");
        _logger.LogInformation("[email-password-reset] Email Password Reset called for {InstallId}", installId);
        await _emailService.SendPasswordResetEmail(dto);
        return Ok();
    }

    /// <summary>
    /// Calls a public api on the requesting server to validate if it's reachable
    /// </summary>
    /// <returns></returns>
    [HttpGet("reachable")]
    public async Task<ActionResult<bool>> CanReachServer(string host)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[reachable] Request came in from {InstallId} on version {Version}", installId, version);
        var apiUrl = Request.Scheme + "://" + host + Request.PathBase + "/api/";
        FlurlHttp.ConfigureClient(apiUrl, cli =>
            cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());

        try
        {
            var response = await (apiUrl + "admin/exists")
                .WithHeader("Accept", "application/json")
                .WithHeader("User-Agent", "Kavita Email Service")
                .WithHeader("Content-Type", "application/json")
                .WithTimeout(TimeSpan.FromSeconds(30))
                .GetAsync();

            if (response.StatusCode != StatusCodes.Status200OK)
            {
                return Ok(false);
            }
        }
        catch (Exception)
        {
            return Ok(false);
        }

        return Ok(true);
    }
}