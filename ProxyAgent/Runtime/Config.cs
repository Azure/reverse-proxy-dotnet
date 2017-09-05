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
        string Endpoint { get; }
        int MaxPayloadSize { get; }
        bool ConfigStatus { get; }
        LogLevel LogLevel { get; }
    }

    /// <summary>Web application configuration</summary>
    public class Config : IConfig
    {
        private const string ApplicationKey = "reverseproxy:";
        private const string EndpointKey = ApplicationKey + "endpoint";
        private const string MaxPayloadSizeKey = ApplicationKey + "maxPayloadSize";
        private const string ConfigStatusKey = ApplicationKey + "configStatus";
        private const string LogLevelKey = ApplicationKey + "loglevel";

        public string Endpoint { get; }
        public int MaxPayloadSize { get; }
        public bool ConfigStatus { get; }
        public LogLevel LogLevel { get; }

        public Config(IConfigData configData)
        {
            Enum.TryParse(configData.GetString(LogLevelKey), out LogLevel logLevel);
            this.LogLevel = logLevel;

            this.ConfigStatus = configData.GetBool(ConfigStatusKey);
            this.MaxPayloadSize = configData.GetInt(MaxPayloadSizeKey);

            this.Endpoint = configData.GetString(EndpointKey);
            if (string.IsNullOrEmpty(this.Endpoint))
            {
                throw new InvalidConfigurationException("The remote hostname is empty.");
            }
        }
    }
}