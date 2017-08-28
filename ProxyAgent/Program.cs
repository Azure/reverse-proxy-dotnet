// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config(new ConfigData());

            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    // The default setting is ProcessorCount/2
                    options.ThreadCount = System.Environment.ProcessorCount;
                })
                .UseUrls("http://*:" + config.Port)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}