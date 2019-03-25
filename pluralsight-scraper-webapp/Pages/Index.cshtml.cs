using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.NodeServices;

namespace pluralsight_scraper_webapp.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet([FromServices] INodeServices nodeServices)
        {
            const int NUM1 = 10;
            const int NUM2 = 20;

            int result = nodeServices.InvokeAsync<int>("Node/AddModule.js", NUM1, NUM2).Result;

            string foo = $"result of [{NUM1}] + [{NUM2}] is [{result}]";
        }
    }
}
