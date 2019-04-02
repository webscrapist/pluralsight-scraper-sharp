using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VH.PluralsightScraper
{
    internal static class ConsoleView
    {
        public static void ShowGettingChannels()
        {
            _stopWatch = Stopwatch.StartNew();
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

        public static void Show(IEnumerable<Channel> channelsList)
        {
            Channel[] sortedChannels = channelsList.OrderBy(_ => _.ChannelName).ToArray();

            for (var i = 0; i < sortedChannels.Length; i++)
            {
                Show(sortedChannels[i], channelIndex: i + 1);
            }
            
            Console.WriteLine();
            Console.WriteLine($"channels count: [{sortedChannels.Length}]");
            Console.WriteLine($"       elapsed: [{_stopWatch.Elapsed}]");
        }

        private static void Show(Channel channel, int channelIndex)
        {
            if (channel.ScrapException == null)
            {
                Console.WriteLine($" {channelIndex:D2}) {channel.ChannelName} ({channel.Courses.Count()} courses)");
            }
            else
            {
                ConsoleColor previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($" {channelIndex:D2}) {channel.ChannelName}");
                Console.WriteLine($"   [{channel.Url}]");
                
                Console.WriteLine();
                Console.WriteLine($"   {channel.ScrapException.Message}");

                Console.ForegroundColor = previousColor;
            }
        }

        private static Stopwatch _stopWatch;
    }
}
