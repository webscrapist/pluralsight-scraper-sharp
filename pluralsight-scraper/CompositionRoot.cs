namespace VH.PluralsightScraper
{
    internal static class CompositionRoot
    {
        public static Scraper CreateScraper(string username, string password, bool headless)
        {
            var browserFactory = new BrowserFactory(headless);
            return new Scraper(browserFactory, username, password);
        }
    }
}
