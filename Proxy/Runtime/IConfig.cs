using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime
{
    public interface IConfig
    {
        // The remote endpoint the proxy communicates with
        string Endpoint { get; }

        // The remote endpoint ssl certificate thumbprint the proxy communicates with
        string SslCertThumbprint { get; }

        // Whether to redirect HTTP requests to HTTPS
        bool RedirectHttpToHttps { get; }

        // Whether to tell clients to default to HTTPS in the future
        bool StrictTransportSecurityEnabled { get; }

        // How long clients should persist the HSTS rule, specified in seconds
        int StrictTransportSecurityPeriod { get; }

        // Maximum request payload size
        int MaxPayloadSize { get; }

        // Whether to expose configuration settings in /status
        bool StatusEndpointEnabled { get; }

        // Application logging level
        LogLevel LogLevel { get; }

        // Whether to automatically follow the redirect given by the end point.
        bool AutoRedirect { get; }
    }
}
