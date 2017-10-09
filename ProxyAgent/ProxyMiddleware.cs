// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class ProxyMiddleware
    {
        private readonly string endpoint;
        private readonly IProxy proxy;
        private readonly ILogger log;

        public ProxyMiddleware(
            // ReSharper disable once UnusedParameter.Local
            RequestDelegate next, // Required by ASP.NET
            IConfig config,
            IProxy proxy,
            ILogger log)
        {
            this.proxy = proxy;
            this.log = log;
            this.endpoint = config.Endpoint;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.proxy.ProcessAsync(this.endpoint, context.Request, context.Response);
                this.log.Debug("--------------------------------------------------------------------------------", () => { });
            }
            catch (Exception e)
            {
                this.log.Error("Proxied request failed", () => new { e });
            }
        }
    }
}