// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions
{
    /// <summary>
    /// This exception is thrown when a client attempts to use HTTP and the
    /// proxy is configured to redirect requests to HTTPS.
    /// </summary>
    public class RedirectException : Exception
    {
        public string Location { get; }

        public HttpStatusCode StatusCode { get; }

        public RedirectException(HttpStatusCode statusCode, string location)
        {
            this.StatusCode = statusCode;
            this.Location = location;
        }
    }
}