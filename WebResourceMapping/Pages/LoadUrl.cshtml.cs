using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebResourceMappingAPI.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace WebResourceMapping.Pages
{
    public class LoadUrlModel : PageModel
    {
        private readonly string _api;
        public LoadUrlModel(IConfiguration config)
        {
            _api = config.GetValue<string>("ApiURL");
        }
        public WebsiteContentModel Content { get; set; }
        public void OnGet()
        {
            var url = Request.Query["url"];
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }
            using(var client = new HttpClient())
            {

                var apiCallUrl = new Uri($"{_api}?url={url}");
                
                client.BaseAddress = apiCallUrl;
                var result = client.GetAsync(apiCallUrl).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                Content = JsonConvert.DeserializeObject<WebsiteContentModel>(json);
            }
        }
    }
}
