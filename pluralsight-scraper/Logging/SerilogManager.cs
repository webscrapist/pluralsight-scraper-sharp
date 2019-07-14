using Microsoft.Extensions.Configuration;
using Serilog;

namespace VH.PluralsightScraper.Logging
{
    internal static class SerilogManager
    {
        public static void Init(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Log.Debug("serilog configured from AppSettings.json");
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
