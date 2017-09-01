// Copyright (c) Microsoft. All rights reserved.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics.ILogger;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class Startup
    {
        // Initialized in `ConfigureServices`
        public IContainer ApplicationContainer { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddIniFile("appsettings.ini", optional: false, reloadOnChange: false);
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This is where you register dependencies, add services to the
        // container. This method is called by the runtime, before the
        // Configure method below.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            this.ApplicationContainer = DependencyResolution.Setup(services);

            // Print some useful information at bootstrap time
            PrintBootstrapInfo(this.ApplicationContainer);

            // Create the IServiceProvider based on the container
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));

            // Uncomment these lines if you want to host static files in wwwroot/
            // More info: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware
            // app.UseDefaultFiles();
            // app.UseStaticFiles();

            app.UseMvc();

            app.UseMiddleware<ProxyMiddleware>();

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            appLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }

        private static void PrintBootstrapInfo(IContainer container)
        {
            var logger = container.Resolve<ILogger>();
            var config = container.Resolve<IConfig>();
            logger.Info("Proxy agent started", () => new { Uptime.ProcessId });
            logger.Info("TCP port: " + config.Port, () => { });
            logger.Info("Remote endpoint: " + config.Endpoint, () => { });
            logger.Info("Max payload size: " + config.MaxPayloadSize, () => { });
        }
    }
}