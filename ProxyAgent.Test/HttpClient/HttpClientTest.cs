// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient;
using Xunit;

namespace ProxyAgent.Test.HttpClient
{
    public class HttpClientTest
    {
        // Note: HttpClient is not testable, so this is only a PoC.
        // The following is not testing the real code, but validating a copy of the code
        // used in HttpClient.SetHeaders().
        [Fact]
        public void HttpClient_SetHeaders_CanSetHeadersOnContent()
        {
            // Arrange
            var target = new HttpRequest();
            var mediaType = "multipart/form-data; boundary=--------------------------950452406595708528247546";
            var body = "--------------------------950452406595708528247546\n" +
                       "Content-Disposition: form-data; name=\"file\"; filename=\"SetFuelLevel-method.js\"\n" +
                       "Content-Type: application/octet-stream\n" +
                       "\n" +
                       "foo\n" +
                       "--------------------------950452406595708528247546--";
            var content = Encoding.UTF8.GetBytes(body);

            // Act
            target.SetContent(content, mediaType);
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost"),
                Content = target.Content
            };
            var headerAdded = httpRequest.Content?.Headers?.TryAddWithoutValidation("Content-Type", mediaType);

            // Assert
            Assert.True(headerAdded);
        }
    }
}
