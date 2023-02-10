using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebResourceMappingInterfaces;

namespace WebResourceMappingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadUrlController : ControllerBase, IWebResourceMappingApi
    {
        HttpClient HttpClient;
        public LoadUrlController(HttpClient httpClient) {
            HttpClient = httpClient?? throw new ArgumentNullException(nameof(httpClient));
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> LoadUrl(string url) {

            var result = await HttpClient.GetAsync(url);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                return BadRequest(result); 
            }

            return Ok(result);
        }
    }
}
