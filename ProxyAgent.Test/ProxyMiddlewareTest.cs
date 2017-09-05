// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.ReverseProxy;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using Moq;
using Xunit;

namespace ProxyAgent.Test
{
    public class ProxyMiddlewareTest
    {
        [Fact]
        public void UsesProxy()
        {
            const string endpoint = "http://foobar.com/v1";

            // Arrange
            var next = new Mock<RequestDelegate>();
            var proxy = new Mock<IProxy>();
            var log = new Mock<ILogger>();

            var config = new Mock<IConfig>();
            config.SetupGet(x => x.Endpoint).Returns(endpoint);

            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);

            var target = new ProxyMiddleware(next.Object, config.Object, proxy.Object, log.Object);

            // Act
            target.Invoke(context.Object).Wait();

            // Assert
            proxy.Verify(x => x.ProcessAsync(endpoint, request.Object, response.Object), Times.Once);
        }
    }
}