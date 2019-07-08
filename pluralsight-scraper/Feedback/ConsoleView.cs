using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Feedback
{
    internal static class ConsoleView
    {
        public static void ShowGettingChannels()
        {
            _stopWatch = Stopwatch.StartNew();

            Console.WriteLine("press Ctrl+C to cancel");
            Console.WriteLine("getting channels...");
        }

        public static void Show(Exception exception)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            while (true)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    Console.WriteLine("  ====== inner exception ======");

                    continue;
                }

                break;
            }

            Console.ForegroundColor = previousColor;
        }

        public static void Show(IEnumerable<ChannelDto> channelsList)
        {
            ChannelDto[] sortedChannels = channelsList.OrderBy(_ => _.Name).ToArray();

            for (var i = 0; i < sortedChannels.Length; i++)
            {
                Show(sortedChannels[i], channelIndex: i + 1);
            }
            
            Console.WriteLine();
            Console.WriteLine($"channels count : [{sortedChannels.Length}]");
            Console.WriteLine($"       elapsed : [{_stopWatch?.Elapsed}]");
        }

        public static void Show(ReplicateResult result)
        {
            var columnsConfig = new[]
                                {
                                    new TableColumn(TableRenderer.COLUMN_NAME_INDEX       , length:  5, alignment: "right", getValue: (detail, i) => i.ToString()),
                                    new TableColumn(TableRenderer.COLUMN_NAME_CHANNEL_NAME, length: 30, alignment: "left" , getValue: (detail, i) => detail.ChannelName),
                                    new TableColumn(TableRenderer.COLUMN_NAME_ACTION      , length: 10, alignment: "left" , getValue: (detail, i) => detail.Action.ToString()),
                                };

            var tableRenderer = new TableRenderer(columnsConfig);

            Console.WriteLine();
            Console.WriteLine(tableRenderer.Headers);

            var rowIndex = 1;

            foreach (ReplicateResultDetail detail in result.Details)
            {
                Console.WriteLine(tableRenderer.DataRow(detail, rowIndex));
                rowIndex += 1;
            }

            Console.WriteLine();
            Console.WriteLine($" channels created   : [{result.ChannelsCreatedCount}]");
            Console.WriteLine($" channels updated   : [{result.ChannelsUpdatedCount}]");
            Console.WriteLine($" channels deleted   : [{result.ChannelsDeletedCount}]");
            Console.WriteLine($" channels unchanged : [{result.ChannelsUnchangedCount}]");

            Console.WriteLine( "-------------------   ------");
            Console.WriteLine($"     total channels : [{result.TotalChannelsCount}]");

            Console.WriteLine();
            Console.WriteLine($"            elapsed : [{_stopWatch?.Elapsed}]");
        }

        public static void ShowCancelRequested()
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("cancel requested...");

            Console.ForegroundColor = previousColor;
        }

        private static void Show(ChannelDto channel, int channelIndex)
        {
            if (channel.ScrapException == null)
            {
                Console.WriteLine($" {channelIndex:D2}) {channel.Name} ({channel.Courses.Count()} courses)");
            }
            else
            {
                ConsoleColor previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($" {channelIndex:D2}) {channel.Name}");
                Console.WriteLine($"   [{channel.Url}]");
                
                Console.WriteLine();
                Console.WriteLine($"   {channel.ScrapException.Message}");

                Console.ForegroundColor = previousColor;
            }
        }

        private static Stopwatch _stopWatch;
    }
}
