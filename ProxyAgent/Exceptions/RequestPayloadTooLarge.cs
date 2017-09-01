// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions
{
    /// <summary>
    /// This exception is thrown when the service is configured incorrectly.
    /// In order to recover, the service owner should fix the configuration
    /// and re-deploy the service.
    /// </summary>
    public class RequestPayloadTooLargeException : Exception
    {
        public RequestPayloadTooLargeException()
        {
        }

        public RequestPayloadTooLargeException(string message) : base(message)
        {
        }

        public RequestPayloadTooLargeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}