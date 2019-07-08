using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VH.PluralsightScraper.Authentication;
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
            string connectionString = GetConnectionString();
            
            DbContextOptions<PluralsightContext> options = 
                new DbContextOptionsBuilder<PluralsightContext>().UseNpgsql(connectionString).Options;

            var session = new WindowsSession();

            var pluralsightContext = new PluralsightContext(options, session);

            return new ChannelsReplicator(pluralsightContext);
        }

        private static string GetConnectionString()
        {
            IConfigurationBuilder configBuilder = 
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                          .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configRoot = configBuilder.Build();

            return configRoot.GetConnectionString("PluralsightData");
        }
    }
}
