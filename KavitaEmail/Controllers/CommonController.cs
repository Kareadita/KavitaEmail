using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeleton.Misc;

namespace Skeleton.Controllers;

[Route("api/")]
public class CommonController : BaseApiController
{
    private readonly ILogger<CommonController> _logger;

    public CommonController(ILogger<CommonController> logger)
    {
        _logger = logger;
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