using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using WebResourceMappingAPI.Controllers;
using WebResourceMappingInterfaces;
using Xunit;

namespace WebResourceMappingTests
{
    public class ApiTests
    {
        private IWebResourceMappingApi Api;
        HttpClient httpClient;
        readonly Mock<HttpMessageHandler> handler = new Mock<HttpMessageHandler>();

        [Fact]
        public async void LoadUrlTestExpectOkResult()
        {
            handler.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{}"),
               });

            httpClient = new HttpClient(handler.Object);

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}