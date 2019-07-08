using VH.PluralsightScraper.Data;

namespace VH.PluralsightScraper
{
    internal static class CompositionRoot
    {
        public static Scraper CreateScraper(string username, string password, bool headless)
        {
            var browserFactory = new BrowserFactory(headless);
            return new Scraper(browserFactory, username, password);
        }

        public static ChannelsReplicator CreateChannelsReplicator()
        {
            // santi: [next] implement
            throw new System.NotImplementedException();
        }
    }
}
