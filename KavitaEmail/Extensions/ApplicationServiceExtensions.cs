using API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skeleton.Data;
using Skeleton.DTOs;
using Skeleton.Services;

namespace Skeleton.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddSqLite(config, env);
            services.AddLogging(config);
            services.AddSignalR();
            
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<SmtpConfig>(config.GetSection(SmtpConfig.Key));
        }

        private static void AddSqLite(this IServiceCollection services, IConfiguration config,
            IWebHostEnvironment env)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(env.IsDevelopment());
            });
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
