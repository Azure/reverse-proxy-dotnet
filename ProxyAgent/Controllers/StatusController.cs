// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Controllers
{
    // TODO: require auth
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly IConfig config;

        public StatusController(IConfig config)
        {
            this.config = config;
        }

        // GET api/values
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            return new Dictionary<string, string>
            {
                { "Endpoint", this.config.Endpoint },
                { "MaxPayloadSize", this.config.MaxPayloadSize.ToString() }
            };
        }
    }
}
