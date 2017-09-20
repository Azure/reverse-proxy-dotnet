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
        // The remote endpoint the proxy communicates with
        string Endpoint { get; }

        // Whether to redirect HTTP requests to HTTPS
        bool RedirectHttpToHttps { get; }

        // Whether to tell clients to default to HTTPS in the future
        bool StrictTransportSecurityEnabled { get; }

        // How long clients should persist the HSTS rule, specified in seconds
        int StrictTransportSecurityPeriod { get; }

        // Maximum request payload size
        int MaxPayloadSize { get; }

        // Whether to expose configuration settings in /status
        bool ConfigStatus { get; }

        // Application logging level
        LogLevel LogLevel { get; }
    }

    /// <summary>Web application configuration</summary>
    public class Config : IConfig
    {
        private const bool REDIRECT_HTTP_DEFAULT = true;
        private const bool STS_ENABLED_DEFAULT = true;
        private const int STS_PERIOD_DEFAULT = 2592000;
        private const int MAX_PAYLOAD_SIZE_DEFAULT = 131072;
        private const bool CONFIG_STATUS_DEFAULT = false;
        private const string LOG_LEVEL_DEFAULT = "Warn";

        private const string APPLICATION_KEY = "reverseproxy:";
        private const string ENDPOINT_KEY = APPLICATION_KEY + "endpoint";
        private const string REDIRECTHTTP_KEY = APPLICATION_KEY + "redirectHttpToHttps";
        private const string STS_ENABLED_KEY = APPLICATION_KEY + "strictTransportSecurityEnabled";
        private const string STS_PERIOD_KEY = APPLICATION_KEY + "strictTransportSecurityPeriod";
        private const string MAX_PAYLOAD_SIZE_KEY = APPLICATION_KEY + "maxPayloadSize";
        private const string CONFIG_STATUS_KEY = APPLICATION_KEY + "configStatus";
        private const string LOG_LEVEL_KEY = APPLICATION_KEY + "loglevel";

        public string Endpoint { get; }
        public bool RedirectHttpToHttps { get; }
        public bool StrictTransportSecurityEnabled { get; }
        public int StrictTransportSecurityPeriod { get; }
        public int MaxPayloadSize { get; }
        public bool ConfigStatus { get; }
        public LogLevel LogLevel { get; }

        public Config(IConfigData configData)
        {
            Enum.TryParse(configData.GetString(LOG_LEVEL_KEY, LOG_LEVEL_DEFAULT), out LogLevel logLevel);
            this.LogLevel = logLevel;

            this.ConfigStatus = configData.GetBool(CONFIG_STATUS_KEY, CONFIG_STATUS_DEFAULT);
            this.MaxPayloadSize = configData.GetInt(MAX_PAYLOAD_SIZE_KEY, MAX_PAYLOAD_SIZE_DEFAULT);

            this.RedirectHttpToHttps = configData.GetBool(REDIRECTHTTP_KEY, REDIRECT_HTTP_DEFAULT);
            this.StrictTransportSecurityEnabled = configData.GetBool(STS_ENABLED_KEY, STS_ENABLED_DEFAULT);
            this.StrictTransportSecurityPeriod = configData.GetInt(STS_PERIOD_KEY, STS_PERIOD_DEFAULT);

            this.Endpoint = configData.GetString(ENDPOINT_KEY);
            if (string.IsNullOrEmpty(this.Endpoint))
            {
                throw new InvalidConfigurationException("The remote endpoint hostname is empty.");
            }
        }
    }
}