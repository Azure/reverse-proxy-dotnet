// Copyright (c) Microsoft. All rights reserved.

// TODO: tests
// TODO: handle errors
// TODO: use binding

using Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime
{
    public interface IConfig
    {
        string Hostname { get; }
    }

    /// <summary>Web application configuration</summary>
    public class Config : IConfig
    {
        private const string ApplicationKey = "reverseproxy:";
        private const string HostnameKey = ApplicationKey + "hostname";

        public string Hostname { get; }

        public Config(IConfigData configData)
        {
            this.Hostname = configData.GetString(HostnameKey);
            if (string.IsNullOrEmpty(this.Hostname))
            {
                throw new InvalidConfigurationException("The remote hostname is empty.");
            }
        }
    }
}