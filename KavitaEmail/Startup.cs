using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Skeleton.Extensions;

namespace Skeleton
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config, _env);
            services.AddControllers();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
            services.AddCors();

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/jpeg", "image/jpg" });
                options.EnableForHttps = true;
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            
            
            services.Configure<FormOptions>(options =>
            {
                var sizeLimit = _config.GetSection("SMTP:SizeLimit").Get<int>();
                if (sizeLimit == 0) sizeLimit = 26_214_400;
                options.MemoryBufferThreshold = sizeLimit; // 25MB
            });

            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            app.UseResponseCompression();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();
            
            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            });
            
            app.UseSerilogRequestLogging();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            applicationLifetime.ApplicationStarted.Register(() =>
            {

                var version = Assembly.GetExecutingAssembly().GetName().Version;
                try
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("KavitaEmail - v{Version}", version);
                }
                catch (Exception)
                {
                    /* Swallow Exception */
                }
                Console.WriteLine($"KavitaEmail - v{version}");
            });
            
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine($"Kavita Email Service has started up");
            });
        }
        
    }
}