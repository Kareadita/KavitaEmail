using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeleton.DTOs;
using Skeleton.Services;

namespace Skeleton.Controllers
{
    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler() {
            return new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
        }
    }
    public class EmailController : BaseApiController
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailService _emailService;

        public EmailController(ILogger<EmailController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }
        
        /// <summary>
        /// This is used to validate the service is reachable
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public ActionResult<bool> Test()
        {
            return Ok(true);
        }

        [HttpPost("confirm")]
        public async Task<ActionResult> SendConfirmationEmail(ConfirmationEmailDto dto)
        {
            await _emailService.SendEmailForEmailConfirmation(dto);
            return Ok();
        }
        
        [HttpPost("email-migration")]
        public async Task<ActionResult> SendEmailMigrationEmail(EmailMigrationDto dto)
        {
            await _emailService.SendEmailMigrationEmail(dto);
            return Ok();
        }
        
        [HttpPost("email-password-reset")]
        public async Task<ActionResult> SendPasswordResetConfirmation(PasswordResetDto dto)
        {
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
            catch (Exception ex)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}