// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions
{
    /// <summary>
    /// This exception is thrown when a client sends a request
    /// too large to handle, depending on the configured settings.
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
