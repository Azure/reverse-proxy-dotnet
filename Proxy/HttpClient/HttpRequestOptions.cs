// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient
{
    public class HttpRequestOptions
    {
        public bool EnsureSuccess { get; set; } = false;

        public bool AllowInsecureSslServer { get; set; } = false;

        public int Timeout { get; set; } = 30000;
    }
}