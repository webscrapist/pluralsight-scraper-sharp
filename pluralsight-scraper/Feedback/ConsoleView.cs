using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Serilog;
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
            var columnsConfig = 
                new[]
                {
                    new TableColumn(TableRenderer.COLUMN_NAME_INDEX       , length:  5, alignment: "right", getValue: (detail, i) => i.ToString()),
                    new TableColumn(TableRenderer.COLUMN_NAME_CHANNEL_NAME, length: 30, alignment: "left" , getValue: (detail, i) => detail.ChannelName),
                    new TableColumn(TableRenderer.COLUMN_NAME_ACTION      , length: 10, alignment: "left" , getValue: (detail, i) => detail.Action.ToString()),
                };

            var tableRenderer = new TableRenderer(columnsConfig);

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(tableRenderer.Headers);

            var rowIndex = 1;

            foreach (ReplicateResultDetail detail in result.Details)
            {
                sb.AppendLine(tableRenderer.DataRow(detail, rowIndex));
                rowIndex += 1;
            }

            sb.AppendLine();
            sb.AppendLine($" channels created   : [{result.ChannelsCreatedCount}]");
            sb.AppendLine($" channels updated   : [{result.ChannelsUpdatedCount}]");
            sb.AppendLine($" channels deleted   : [{result.ChannelsDeletedCount}]");
            sb.AppendLine($" channels unchanged : [{result.ChannelsUnchangedCount}]");

            sb.AppendLine( "-------------------   ------");
            sb.AppendLine($"     total channels : [{result.TotalChannelsCount}]");

            sb.AppendLine();
            sb.AppendLine($"            elapsed : [{_stopWatch?.Elapsed}]");

            Log.Information(sb.ToString());
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
