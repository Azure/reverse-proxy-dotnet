// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly IConfig config;

        public StatusController(IConfig config)
        {
            this.config = config;
        }

        [HttpGet]
        public Dictionary<string, string> Get()
        {
            if (this.config.ConfigStatus)
            {
                return new Dictionary<string, string>
                {
                    { "Status", "Alive and Well" },
                    { "Endpoint", this.config.Endpoint },
                    { "MaxPayloadSize", this.config.MaxPayloadSize.ToString() },
                    { "ProcessId", Uptime.ProcessId },
                    { "Uptime.Start", Uptime.Start.ToString() },
                    { "Uptime.Duration", Uptime.Duration.ToString(@"dd\.hh\:mm\:ss") }
                };
            }

            return new Dictionary<string, string>
            {
                { "Status", "Alive and Well" }
            };
        }
    }
}
