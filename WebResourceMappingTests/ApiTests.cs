using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Moq.Protected;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using WebResourceMappingAPI.Controllers;
using WebResourceMappingAPI.Models;
using WebResourceMappingInterfaces;
using Xunit;

namespace WebResourceMappingTests
{
    public class ApiTests
    {
        private IWebResourceMappingApi Api;
        readonly Mock<HttpMessageHandler> handler = new Mock<HttpMessageHandler>();
        private HttpClient GetHttpClient(string returnHtml)
        {
            handler.Protected()
              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(new HttpResponseMessage
              {
                  StatusCode = HttpStatusCode.OK,
                  Content = new StringContent(returnHtml),
              });

            return new HttpClient(handler.Object);
        }
        [Fact]
        public async void LoadUrlTestExpectOkResult()
        {

            var httpClient = GetHttpClient("<html><body>hellow world!</body></html>");

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void LoadUrlTestExpectWebSiteContentModel()
        {
            var httpClient = GetHttpClient("<html><body><p>Three Words Here</p></body></html>");

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);
            Assert.IsType<WebsiteContentModel>((result as OkObjectResult)?.Value);
        }
    }
}