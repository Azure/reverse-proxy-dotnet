// Copyright (c) Microsoft. All rights reserved.

// TODO: tests
// TODO: handle errors
// TODO: use binding

using System;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime
{
    public interface IConfig
    {
        LogLevel LogLevel { get; }
        int Port { get; }
        string Hostname { get; }
    }

    /// <summary>Web application configuration</summary>
    public class Config : IConfig
    {
        private const string ApplicationKey = "reverseproxy:";
        private const string LogLevelKey = ApplicationKey + "loglevel";
        private const string PortKey = ApplicationKey + "port";
        private const string HostnameKey = ApplicationKey + "hostname";

        public LogLevel LogLevel { get; }
        public int Port { get; }
        public string Hostname { get; }

        public Config(IConfigData configData)
        {
            Enum.TryParse(configData.GetString(LogLevelKey), out LogLevel logLevel);
            this.LogLevel = logLevel;

            this.Port = configData.GetInt(PortKey);

            this.Hostname = configData.GetString(HostnameKey);
            if (string.IsNullOrEmpty(this.Hostname))
            {
                throw new InvalidConfigurationException("The remote hostname is empty.");
            }
        }
    }
}