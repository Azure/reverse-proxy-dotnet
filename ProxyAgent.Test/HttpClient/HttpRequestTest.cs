// Copyright (c) Microsoft. All rights reserved.

using System.Text;
using Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient;
using Xunit;

namespace ProxyAgent.Test.HttpClient
{
    public class HttpRequestTest
    {
        [Fact]
        public void ItDefaultsToJsonMediaType()
        {
            // Arrange
            var target = new HttpRequest();
            var content = Encoding.UTF8.GetBytes("{}");
            string mediaType = null;

            // Act
            target.SetContent(content, mediaType);

            // Assert
            Assert.Equal("application/json", target.ContentType.MediaType);
        }

        [Fact]
        public void ItPassesTheOriginalMediaType()
        {
            // Arrange
            var target = new HttpRequest();
            var content = Encoding.UTF8.GetBytes("{}");
            string mediaType = "application/pdf";

            // Act
            target.SetContent(content, mediaType);

            // Assert
            Assert.Equal(mediaType, target.ContentType.MediaType);
        }

        [Fact]
        public void ItHandlesFileTransferMediaType()
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

            // Assert
            Assert.Null(target.ContentType);
        }
    }
}
