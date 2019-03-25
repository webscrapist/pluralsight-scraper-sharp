using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace VH.PluralsightScraper
{
    internal class Program
    {
        private static void Main()
        {
            if (TakeScreenshot().Result)
            {
                Console.WriteLine("all good");
            }
        }

        private static async Task<bool> TakeScreenshot()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

            Page page = await browser.NewPageAsync();

            await page.GoToAsync("http://www.google.com");

            const string OUTPUT_FILE = @"C:\Users\Santiago\Desktop\delete\scraper-output\pic.jpg";

            await page.ScreenshotAsync(OUTPUT_FILE);

            return true;
        }
    }
}
