using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using VH.PluralsightScraper.Data;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;
using VH.PluralsightScraper.Feedback;
using VH.PluralsightScraper.Logging;

namespace VH.PluralsightScraper
{
    internal class Program
    {
        static Program()
        {
            Replicator = CompositionRoot.CreateChannelsReplicator(Configuration);
        }
        
        public static async Task Main(string[] args)
        {
            try
            {
                SerilogManager.Init(Configuration);

                Log.Information("********************  APP STARTED  ********************");
                
                string username = args.Username();
                string password = args.Password();
                bool headless = args.RunHeadless();
                bool isFastRun = args.IsFastRun();

                _cancellation = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, eventArgs) =>
                                          {
                                              ConsoleView.ShowCancelRequested();
                                              _cancellation.Cancel();
                                          };

                IEnumerable<ChannelDto> channels = await GetChannels(username, password, headless, isFastRun);

                await ReplicateChannels(channels);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "app crashed");
            }
            finally
            {
                Log.Information("********************  APP FINISHED ********************");
                SerilogManager.Close();
            }
        }

        private static async Task<IEnumerable<ChannelDto>> GetChannels(string username, 
                                                                       string password, 
                                                                       bool headless, 
                                                                       bool isFastRun)
        {
            ConsoleView.ShowGettingChannels();

            Scraper scraper = CompositionRoot.CreateScraper(username, password, headless);

            ChannelDto[] channels = (await scraper.GetChannels(isFastRun, _cancellation.Token)).ToArray();

            ConsoleView.Show(channels);

            return channels;
        }

        private static async Task ReplicateChannels(IEnumerable<ChannelDto> channels)
        {
            ReplicateResult result = await Replicator.Replicate(channels, _cancellation.Token);
            ConsoleView.Show(result);
        }

        internal static readonly IConfiguration Configuration =
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: false)
                .Build();

        private static CancellationTokenSource _cancellation;
        private static readonly ChannelsReplicator Replicator;
    }
}
