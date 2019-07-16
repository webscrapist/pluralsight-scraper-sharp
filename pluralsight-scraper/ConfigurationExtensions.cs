using Microsoft.Extensions.Configuration;

namespace VH.PluralsightScraper
{
    internal static class ConfigurationExtensions
    {
        public static string GetPostgreSqlConnString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("PluralsightData");
        }
    }
}
