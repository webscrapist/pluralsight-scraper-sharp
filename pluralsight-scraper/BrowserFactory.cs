using System.Threading.Tasks;
using PuppeteerSharp;

namespace VH.PluralsightScraper
{
    internal class BrowserFactory
    {
        public BrowserFactory(bool headless)
        {
            _headless = headless;
        }

        public async Task<Browser> Create()
        {
            await DownloadChromiumIfNeeded();
            return await Puppeteer.LaunchAsync(new LaunchOptions { Headless = _headless });
        }

        private static async Task DownloadChromiumIfNeeded()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        }

        private readonly bool _headless;
    }
}
