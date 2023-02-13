using Microsoft.AspNetCore.Mvc;

namespace WebResourceMappingInterfaces
{
    public interface IWebResourceMappingApi
    {
        Task<IActionResult> LoadUrl(string url);
    }

}