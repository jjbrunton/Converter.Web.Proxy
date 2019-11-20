using System;
using Refit;
using ConversionProxy.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConversionProxy.Proxies;
using Microsoft.OpenApi.Models;
using Sonarr.Models;
using Radarr.Models;
using ConversionProxy.Filters;
using Hangfire.LiteDB;
using Microsoft.Extensions.Logging;
using Hangfire.Console;

namespace ConversionProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.Configuration.UseLiteDbStorage(Configuration[key: "ConnectionStrings:Database"]);
            

            services.AddMvc();
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            

            services.AddSingleton<ISettingsService, SettingsService>();
            var sp = services.BuildServiceProvider();
            var settings = sp.GetService<ISettingsService>();
            services.AddRefitClient<IPlexAutoscanProxy>().ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Settings.PlexAutoscanUrl));
            services.AddRefitClient<IRadarrProxy>().ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Settings.RadarrUrl));
            services.AddRefitClient<ISonarrProxy>().ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Settings.SonarrUrl));
            services.AddSingleton<IFolderMappingService, FolderMappingService>();
            services.AddScoped<INotificationService<SonarrWebhookPayload>, SonarrService>();
            services.AddScoped<INotificationService<RadarrWebhookPayload>, RadarrService>();
            services.AddScoped<IDownloadProcesserService<RadarrWebhookPayload>, RadarrProcessorService>();
            services.AddScoped<IDownloadProcesserService<SonarrWebhookPayload>, SonarrProcessorService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConversionProxy", Version = "v1" });
            });
            this.ConfigureHangfire(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISettingsService settingsService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConversionProxy V1");
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            loggerFactory.AddLog4Net();
            
            app.UseMvc();
            var options = new BackgroundJobServerOptions { WorkerCount = settingsService.Settings.MaxConcurrentConversions };
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new [] { new AnonymousDashboardFilter() }
            });
            app.UseHangfireServer(options);
        }

        private void ConfigureHangfire(IServiceCollection services) 
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseConsole()
                .UseLiteDbStorage(Configuration[key: "ConnectionStrings:Database"])
                );           

        }
    }
}
