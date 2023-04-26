using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skeleton.DTOs;
using Skeleton.Services;

namespace Skeleton.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddLogging(config);

            services.AddScoped<IEmailService, EmailService>();
            services.Configure<SmtpConfig>(config.GetSection(SmtpConfig.Key));
        }

        private static void AddLogging(this IServiceCollection services, IConfiguration config)
        {
            services.AddLogging(loggingBuilder =>
            {
                var loggingSection = config.GetSection("Logging");
                loggingBuilder.AddFile(loggingSection);
            });
        }
    }
}
