using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using VH.PluralsightScraper.Authentication;
using VH.PluralsightScraper.Data;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper
{
    internal static class CompositionRoot
    {
        public static Scraper CreateScraper(string username, string password, bool headless)
        {
            var browserFactory = new BrowserFactory(headless);
            return new Scraper(browserFactory, username, password);
        }

        public static ChannelsReplicator CreateChannelsReplicator(IConfiguration configuration)
        {
            string postgreSqlConnString = configuration.GetPostgreSqlConnString();

            ILoggerFactory serilogFactory = new LoggerFactory().AddSerilog();

            DbContextOptions<PluralsightContext> options = 
                new DbContextOptionsBuilder<PluralsightContext>().EnableSensitiveDataLogging()
                                                                 .UseNpgsql(postgreSqlConnString)
                                                                 .UseLoggerFactory(serilogFactory)
                                                                 .Options;

            var session = new WindowsSession();

            var pluralsightContext = new PluralsightContext(options, session);

            var entityFactory = new EntityFactory();

            return new ChannelsReplicator(pluralsightContext, entityFactory);
        }
    }
}
