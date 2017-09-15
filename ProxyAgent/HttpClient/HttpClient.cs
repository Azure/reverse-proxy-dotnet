// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient
{
    public interface IHttpClient
    {
        Task<IHttpResponse> GetAsync(IHttpRequest request);

        Task<IHttpResponse> PostAsync(IHttpRequest request);

        Task<IHttpResponse> PutAsync(IHttpRequest request);

        Task<IHttpResponse> PatchAsync(IHttpRequest request);

        Task<IHttpResponse> DeleteAsync(IHttpRequest request);

        Task<IHttpResponse> HeadAsync(IHttpRequest request);

        Task<IHttpResponse> OptionsAsync(IHttpRequest request);
    }

    public class HttpClient : IHttpClient
    {
        private readonly ILogger log;
        private const string CONTENT_TYPE_HEADER = "Content-Type";

        public HttpClient(ILogger logger)
        {
            this.log = logger;
        }

        public async Task<IHttpResponse> GetAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Get);
        }

        public async Task<IHttpResponse> PostAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Post);
        }

        public async Task<IHttpResponse> PutAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Put);
        }

        public async Task<IHttpResponse> PatchAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, new HttpMethod("PATCH"));
        }

        public async Task<IHttpResponse> DeleteAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Delete);
        }

        public async Task<IHttpResponse> HeadAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Head);
        }

        public async Task<IHttpResponse> OptionsAsync(IHttpRequest request)
        {
            return await this.SendAsync(request, HttpMethod.Options);
        }

        private async Task<IHttpResponse> SendAsync(IHttpRequest request, HttpMethod httpMethod)
        {
            var clientHandler = new HttpClientHandler();
            using (var client = new System.Net.Http.HttpClient(clientHandler))
            {
                var httpRequest = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = request.Uri
                };

                // Keep track of headers added on the content to avoid duplications
                // which could lead to invalid headers like
                // -> "Content-Type: application/json, application/json"
                var headersOnContentObject = new HashSet<string>();

                SetServerSslSecurity(request, clientHandler);
                SetTimeout(request, client);
                // Note: SetContent must be called before SetHeaders to prioritize the 
                // Content Type value set in the content.
                // TODO: ensure that's what happens with a unit test
                SetContent(request, httpMethod, httpRequest, ref headersOnContentObject);
                SetHeaders(request, httpRequest, ref headersOnContentObject);

                this.log.Debug("Sending request", () => new {httpMethod, request.Uri, request.Options});

                try
                {
                    using (var response = await client.SendAsync(httpRequest))
                    {
                        if (request.Options.EnsureSuccess) response.EnsureSuccessStatusCode();

                        var headers = new HttpHeaders();
                        foreach (var header in response.Headers)
                        {
                            headers.Add(header.Key, header.Value);
                        }
                        foreach (var header in response.Content.Headers)
                        {
                            headers.Add(header.Key, header.Value);
                        }

                        return new HttpResponse
                        {
                            StatusCode = response.StatusCode,
                            Headers = headers,
                            Content = await response.Content.ReadAsStringAsync()
                        };
                    }
                }
                catch (HttpRequestException e)
                {
                    var errorMessage = e.Message;
                    if (e.InnerException != null)
                    {
                        errorMessage += " - " + e.InnerException.Message;
                    }

                    this.log.Error("Request failed", () => new { errorMessage, e });

                    return new HttpResponse
                    {
                        StatusCode = 0,
                        Content = errorMessage
                    };
                }
                catch (TaskCanceledException e)
                {
                    this.log.Error("Request failed",
                        () => new
                        {
                            Message = e.Message + " The request timed out, the endpoint might be unreachable.",
                            e
                        });

                    return new HttpResponse
                    {
                        StatusCode = 0,
                        Content = e.Message + " The endpoint might be unreachable."
                    };
                }
                catch (Exception e)
                {
                    this.log.Error("Request failed", () => new { e.Message, e });

                    return new HttpResponse
                    {
                        StatusCode = 0,
                        Content = e.Message
                    };
                }
            }
        }

        private void SetContent(
            IHttpRequest request,
            HttpMethod httpMethod,
            HttpRequestMessage httpRequest,
            ref HashSet<string> headersOnContentObject)
        {
            if (httpMethod != HttpMethod.Post && httpMethod != HttpMethod.Put) return;

            httpRequest.Content = request.Content;
            if (request.ContentType != null && request.Content != null)
            {
                headersOnContentObject.Add(CONTENT_TYPE_HEADER.ToLowerInvariant());
                httpRequest.Content.Headers.ContentType = request.ContentType;
            }
        }

        private void SetHeaders(
            IHttpRequest request,
            HttpRequestMessage httpRequest,
            ref HashSet<string> headersOnContentObject)
        {
            foreach (var header in request.Headers)
            {
                if (!headersOnContentObject.Contains(header.Key.ToLowerInvariant()))
                {
                    if (!httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        httpRequest.Content?.Headers?.TryAddWithoutValidation(header.Key, header.Value);
                        headersOnContentObject.Add(header.Key.ToLowerInvariant());
                    }
                }
                else
                {
                    this.log.Debug("Skipping header already present in the content object",
                        () => new {header.Key, header.Value});
                }
            }
        }

        private static void SetServerSslSecurity(IHttpRequest request, HttpClientHandler clientHandler)
        {
            if (request.Options.AllowInsecureSslServer && request.UsesSsl())
            {
                clientHandler.ServerCertificateCustomValidationCallback = delegate { return true; };
            }
        }

        private static void SetTimeout(
            IHttpRequest request,
            System.Net.Http.HttpClient client)
        {
            client.Timeout = TimeSpan.FromMilliseconds(request.Options.Timeout);
        }
    }
}