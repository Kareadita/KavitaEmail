using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeleton.Misc;
using Skeleton.Services;

namespace Skeleton.Controllers;

[Route("api/")]
public class CommonController : BaseApiController
{
    private readonly ILogger<CommonController> _logger;
    private readonly IEmailService _emailService;

    public CommonController(ILogger<CommonController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// This is used to validate the service is reachable. Will always return true
    /// </summary>
    /// <param name="adminEmail">The admin's email. An email will be sent to validate email service works</param>
    /// <param name="sendEmail">Should an email be sent to admin's email</param>
    /// <returns></returns>
    [HttpGet("test")]
    public async Task<ActionResult<bool>> Test(string adminEmail, bool sendEmail)
    {
        Request.Headers.TryGetValue("x-kavita-installId", out var installId);
        Request.Headers.TryGetValue("x-kavita-version", out var version);
        _logger.LogInformation("[test] Request came in from {InstallId} on version {Version}", installId, version);

        if (sendEmail)
        {
            await _emailService.SendTestEmail(adminEmail);   
        }
        return Ok(true);
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