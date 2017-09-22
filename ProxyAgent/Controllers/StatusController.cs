// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly IConfig config;
        private readonly IProxy proxy;

        public StatusController(
            IConfig config,
            IProxy proxy)
        {
            this.config = config;
            this.proxy = proxy;
        }

        [HttpGet]
        public async Task<Dictionary<string, string>> GetAsync()
        {
            var status = "OK:Alive and Well";
            var ping = await this.proxy.PingAsync();
            if (ping.StatusCode == 0 || ping.StatusCode >= 500)
            {
                status = "ERROR:" + ping.Message;
            }

            if (this.config.StatusEndpointEnabled)
            {
                return new Dictionary<string, string>
                {
                    { "Status", status },
                    { "ProcessId", Uptime.ProcessId },
                    { "Uptime.Start", Uptime.Start.ToString() },
                    { "Uptime.Duration", Uptime.Duration.ToString(@"dd\.hh\:mm\:ss") },
                    { "Endpoint", this.config.Endpoint },
                    { "MaxPayloadSize", this.config.MaxPayloadSize.ToString() },
                    { "RedirectHttpToHttps", this.config.RedirectHttpToHttps.ToString() },
                    { "StrictTransportSecurityEnabled", this.config.StrictTransportSecurityEnabled.ToString() },
                    { "StrictTransportSecurityPeriod", this.config.StrictTransportSecurityPeriod.ToString() },
                };
            }

            return new Dictionary<string, string>
            {
                { "Status", status }
            };
        }
    }
}