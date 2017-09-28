// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions;
using Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy
{
    public interface IProxy
    {
        Task<ProxyStatus> PingAsync();

        Task ProcessAsync(
            string hostname,
            HttpRequest requestIn,
            HttpResponse responseOut);
    }

    public class Proxy : IProxy
    {
        private const string LOCATION_HEADER = "Location";
        private const string HSTS_HEADER = "Strict-Transport-Security";

        // Headers not forwarded to the remote endpoint
        private static readonly HashSet<string> ExcludedRequestHeaders =
            new HashSet<string>
            {
                "connection",
                "content-length",
                "keep-alive",
                "host",
                "upgrade",
                "upgrade-insecure-requests"
            };

        // Headers returned by the remote endpoint
        // which are not returned to the client
        private static readonly HashSet<string> ExcludedResponseHeaders =
            new HashSet<string>
            {
                "connection",
                "content-length",
                "server",
                "transfer-encoding",
                "upgrade",
                HSTS_HEADER
            };

        private readonly IHttpClient client;

        private readonly IConfig config;

        private readonly ILogger log;

        public Proxy(
            IHttpClient httpclient,
            IConfig config,
            ILogger log)
        {
            this.client = httpclient;
            this.config = config;
            this.log = log;
        }

        public async Task<ProxyStatus> PingAsync()
        {
            var request = new HttpClient.HttpRequest();
            request.SetUriFromString(this.config.Endpoint);
            request.Options.EnsureSuccess = false;
            request.Options.Timeout = 5000;

            // The HTTP client uses cert. pinning, allowing self-signed certs
            request.Options.AllowInsecureSslServer = true;

            var response = await this.client.GetAsync(request);
            var content = response.Content.Length > 120
                ? response.Content.Substring(0, 120) + "..."
                : response.Content;

            return new ProxyStatus
            {
                StatusCode = (int) response.StatusCode,
                Message = content
            };
        }

        public async Task ProcessAsync(
            string hostname,
            HttpRequest requestIn,
            HttpResponse responseOut)
        {
            IHttpRequest request;
        
            try
            {
                this.RedirectToHttpsIfNeeded(requestIn);
                request = this.BuildRequest(requestIn, hostname);
            }
            catch (RequestPayloadTooLargeException)
            {
                responseOut.StatusCode = (int) HttpStatusCode.RequestEntityTooLarge;
                ApplicationRequestRouting.DisableInstanceAffinity(responseOut);
                return;
            }
            catch (RedirectException e)
            {
                responseOut.StatusCode = (int) e.StatusCode;
                responseOut.Headers.Add(LOCATION_HEADER, e.Location);
                ApplicationRequestRouting.DisableInstanceAffinity(responseOut);
                return;
            }

            IHttpResponse response;
            var method = requestIn.Method.ToUpperInvariant();
            this.log.Debug("Request method", () => new {method});
            switch (method)
            {
                case "GET":
                    response = await this.client.GetAsync(request);
                    break;
                case "DELETE":
                    response = await this.client.DeleteAsync(request);
                    break;
                case "OPTIONS":
                    response = await this.client.OptionsAsync(request);
                    break;
                case "HEAD":
                    response = await this.client.HeadAsync(request);
                    break;
                case "POST":
                    response = await this.client.PostAsync(request);
                    break;
                case "PUT":
                    response = await this.client.PutAsync(request);
                    break;
                case "PATCH":
                    response = await this.client.PatchAsync(request);
                    break;
                default:
                    // Note: this could flood the logs due to spiders...
                    this.log.Info("Request method not supported", () => new {method});
                    responseOut.StatusCode = (int) HttpStatusCode.NotImplemented;
                    ApplicationRequestRouting.DisableInstanceAffinity(responseOut);
                    return;
            }

            await this.BuildResponseAsync(response, responseOut, requestIn);
        }

        private void RedirectToHttpsIfNeeded(HttpRequest requestIn)
        {
            if (requestIn.IsHttps || !this.config.RedirectHttpToHttps) return;

            var location = "https://" + requestIn.Host + requestIn.Path.Value + requestIn.QueryString;
            throw new RedirectException(HttpStatusCode.Moved, location);
        }

        // Prepare the request to send to the remote endpoint
        private IHttpRequest BuildRequest(HttpRequest requestIn, string toHostname)
        {
            var requestOut = new HttpClient.HttpRequest();

            // Forward HTTP request headers
            foreach (var header in requestIn.Headers)
            {
                if (ExcludedRequestHeaders.Contains(header.Key.ToLowerInvariant()))
                {
                    this.log.Debug("Ignoring request header", () => new {header.Key, header.Value});
                    continue;
                }

                this.log.Debug("Adding request header", () => new {header.Key, header.Value});
                foreach (var value in header.Value)
                {
                    requestOut.AddHeader(header.Key, value);
                }
            }

            var url = toHostname + requestIn.Path.Value + requestIn.QueryString;
            this.log.Debug("URL", () => new {url});
            requestOut.SetUriFromString(url);

            // Forward request payload
            var method = requestIn.Method.ToUpperInvariant();
            if (HttpClient.HttpClient.MethodsWithPayload.Contains(method))
            {
                requestOut.SetContent(this.GetRequestPayload(requestIn));
            }

            // Allow error codes without throwing an exception
            requestOut.Options.EnsureSuccess = false;

            // The HTTP client uses cert. pinning, allowing self-signed certs
            requestOut.Options.AllowInsecureSslServer = true;

            return requestOut;
        }

        private async Task BuildResponseAsync(IHttpResponse response, HttpResponse responseOut, HttpRequest requestIn)
        {
            // Forward the HTTP status code
            this.log.Debug("Status code", () => new {response.StatusCode});
            responseOut.StatusCode = (int) response.StatusCode;

            // The Headers property can be null in case of errors
            if (response.Headers != null)
            {
                // Forward the HTTP headers
                foreach (var header in response.Headers)
                {
                    if (ExcludedResponseHeaders.Contains(header.Key.ToLowerInvariant()))
                    {
                        this.log.Debug("Ignoring response header", () => new {header.Key, header.Value});
                        continue;
                    }

                    this.log.Debug("Adding response header", () => new {header.Key, header.Value});
                    foreach (var value in header.Value)
                    {
                        responseOut.Headers.Add(header.Key, value);
                    }
                }
            }

            // HSTS support
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security
            // Note: The Strict-Transport-Security header is ignored by the browser when your
            // site is accessed using HTTP; this is because an attacker may intercept HTTP
            // connections and inject the header or remove it.
            if (requestIn.IsHttps && this.config.StrictTransportSecurityEnabled)
            {
                responseOut.Headers.Add(HSTS_HEADER, "max-age=" + this.config.StrictTransportSecurityPeriod);
            }

            // Last header before writing to the socket
            ApplicationRequestRouting.DisableInstanceAffinity(responseOut);

            // Some status codes like 204 and 304 can't have a body
            if (response.CanHaveBody && !string.IsNullOrEmpty(response.Content))
            {
                await responseOut.WriteAsync(response.Content);
            }
        }

        private string GetRequestPayload(HttpRequest request)
        {
            string text;
            var memstream = new MemoryStream();
            request.Body.CopyTo(memstream);
            memstream.Position = 0;
            using (var reader = new StreamReader(memstream))
            {
                text = reader.ReadToEnd();

                // TODO: throw the error before loading the entire payload in memory
                //       use Kestrel options in .NET Core 2.0 to limit the payload size 
                if (text.Length > this.config.MaxPayloadSize)
                {
                    this.log.Warn("User request payloaad is too large", () => new {text.Length});
                    throw new RequestPayloadTooLargeException();
                }
            }

            return text;
        }
    }
}