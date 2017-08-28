// Copyright (c) Microsoft. All rights reserved.

using System.Net;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient
{
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; }
        HttpHeaders Headers { get; }
        string Content { get; }

        bool IsSuccess { get; }
        bool IsError { get; }
        bool IsIncomplete { get; }
        bool IsNonRetriableError { get; }
        bool IsRetriableError { get; }
        bool IsBadRequest { get; }
        bool IsUnauthorized { get; }
        bool IsForbidden { get; }
        bool IsNotFound { get; }
        bool IsTimeout { get; }
        bool IsConflict { get; }
        bool IsServerError { get; }
        bool IsServiceUnavailable { get; }
        bool CanHaveBody { get; }
    }

    public class HttpResponse : IHttpResponse
    {
        private const int TooManyRequests = 429;

        public HttpResponse()
        {
        }

        public HttpResponse(
            HttpStatusCode statusCode,
            string content,
            HttpHeaders headers)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
            this.Content = content;
        }

        public HttpStatusCode StatusCode { get; internal set; }
        public HttpHeaders Headers { get; internal set; }
        public string Content { get; internal set; }

        public bool IsSuccess => (int) this.StatusCode >= 200 && (int) this.StatusCode <= 299;
        public bool IsError => (int) this.StatusCode >= 400 || (int) this.StatusCode == 0;

        public bool IsIncomplete
        {
            get
            {
                var c = (int) this.StatusCode;
                return (c >= 100 && c <= 199) || (c >= 300 && c <= 399);
            }
        }

        public bool IsNonRetriableError => this.IsError && !this.IsRetriableError;

        public bool IsRetriableError => this.StatusCode == HttpStatusCode.NotFound ||
                                        this.StatusCode == HttpStatusCode.RequestTimeout ||
                                        (int) this.StatusCode == TooManyRequests;

        public bool IsBadRequest => (int) this.StatusCode == 400;
        public bool IsUnauthorized => (int) this.StatusCode == 401;
        public bool IsForbidden => (int) this.StatusCode == 403;
        public bool IsNotFound => (int) this.StatusCode == 404;
        public bool IsTimeout => (int) this.StatusCode == 408;
        public bool IsConflict => (int) this.StatusCode == 409;
        public bool IsServerError => (int) this.StatusCode >= 500;
        public bool IsServiceUnavailable => (int) this.StatusCode == 503;

        // HTTP status codes without a body - see RFC 2616
        public bool CanHaveBody
        {
            get
            {
                var c = (int) this.StatusCode;
                return c != 304 && c != 204 && c != 205;
            }
        }

    }
}
