using IronWebScraper;
using System.Linq;

namespace VH.PluralsightScraper
{
    internal class HelloScraper : WebScraper
    {
        #region Overrides of WebScraper

        public override void Init()
        {
            LoggingLevel = LogLevel.All;
            Request("https://blog.scrapinghub.com", Parse);
            ObeyRobotsDotTxt = false;
        }

        public override void Parse(Response response)
        {
            WorkingDirectory = @"C:\Users\Santiago\Desktop\delete\scraper-output";

            // Loop on all Links
            foreach (string title in response.Css("h2.entry-title a").Select(titleLink => titleLink.TextContentClean))
            {
                // Save Result to File
                Scrape(new ScrapedData { { "Title", title } }, "HelloScraper.Jsonl");
            }

            if (!response.CssExists("div.prev-post > a[href]"))
            {
                return;
            }

            // Get Link URL
            string nextPage = response.Css("div.prev-post > a[href]")[0].Attributes["href"];

            // Scrape Next URL
            Request(nextPage, Parse);
        }

        #endregion
    }
}
