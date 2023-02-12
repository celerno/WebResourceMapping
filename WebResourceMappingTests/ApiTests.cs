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
        const string THREEWNINEWT = "<html><body><p>Three Words Here</p></body></html>";
        [Fact]
        public async void LoadUrlTestExpectWebSiteContentModel()
        {
            var httpClient = GetHttpClient(THREEWNINEWT);

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);
            Assert.IsType<WebsiteContentModel>((result as OkObjectResult)?.Value);
        }

        [Fact] 
        public async void LoadUrlTestExpect3WordsContent9WordsTotal()
        {
            var httpClient = GetHttpClient(THREEWNINEWT);

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);
            var model = ((result as OkObjectResult)?.Value as WebsiteContentModel);
            Assert.NotNull(model);
            Assert.Equal(3, model.WordCountContent);
            Assert.Equal(9, model.WordCountAll);
        }

        const string HTMLOPENING = "<html><body>";
        const string HTMLCLOSING = "</body></html>";
        const string IMGSRC = "<img src=\"apic.png\" />";
        const string IMGPICTURETAG = "<picture><img src=\"apic.png\" /></picture>";
        const string THREEIMGSINGLENODE = $"{HTMLOPENING}{IMGSRC}{IMGPICTURETAG}{IMGSRC}{HTMLCLOSING}";

        [Fact]
        public async void LoadUrlImagesCountTestExpectThree() {
            var httpClient = GetHttpClient(THREEIMGSINGLENODE);

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);

            var model = ((result as OkObjectResult)?.Value as WebsiteContentModel);
            
            Assert.NotNull(model);
            Assert.NotEmpty(model.Images);
            Assert.Equal(3, model.Images.Length);
            Assert.Contains(model.Images, img=>img == "apic.png");
            
        }

        const string THREE = "THREE";
        const string FOUR = "Four";
        const string SIX = "SIX";

        const string REPEATEDTHREE = $"{THREE}<p>{THREE}</p><div><div><span>{THREE}</span></div></div>";
        const string REPEATEDFOUR = $"{FOUR}<p>{FOUR}</p><div>{REPEATEDTHREE}<div><span>{FOUR}</span>{FOUR}</div></div>";
        const string REPEATEDSIX = $"{SIX} {REPEATEDFOUR}<p>{SIX}</p><div>{SIX}<div><span>{SIX}</span>{SIX}</div>{SIX}</div>";

        [Fact]
        public async void LoadUrlCountRepeatedWords()
        {
            var httpClient = GetHttpClient(REPEATEDSIX);

            this.Api = new LoadUrlController(httpClient);

            string okUrl = "https://anyUrl.com";

            var result = await Api.LoadUrl(okUrl);
            var model = ((result as OkObjectResult)?.Value as WebsiteContentModel);
            Assert.NotNull(model.AllWordCounters);
            Assert.NotNull(model.ContentWordCounters);
            Assert.Equal(3, model.AllWordCounters["THREE"]);
            Assert.Equal(6, model.ContentWordCounters["SIX"]);
        }
    }
}