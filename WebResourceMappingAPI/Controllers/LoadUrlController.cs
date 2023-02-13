using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using WebResourceMappingAPI.Helpers;
using WebResourceMappingAPI.Models;
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

        /// <summary>
        ///  Receives an Url and return the list of content for the site.
        /// </summary>
        /// <param name="url">Url to process and download</param>
        /// <returns>A list of all images, a count of all words, and a count of each word within the content.</returns>
        [HttpGet]
        [Route("/LoadUrl")]
        public async Task<IActionResult> LoadUrl(string url) {
            var uri = new Uri(url);
            WebsiteContentModel model = new WebsiteContentModel(uri.GetLeftPart(UriPartial.Authority));
            try
            {
                var result = await HttpClient.GetAsync(uri.AbsoluteUri);
                
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    model.ErrorMessages = result.Content.ReadAsStringAsync().Result;
                    return BadRequest(model);
                }

                

                return Ok(result.Content.ProcessContent(model));
            }
            catch (Exception ex) {
                model.ErrorMessages = ex.Message;
                return BadRequest(model);
            }
            finally
            {
                HttpClient.CancelPendingRequests();
            }
        }
        
    }
}
