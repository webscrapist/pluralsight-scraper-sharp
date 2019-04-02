using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VH.PluralsightScraper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string username = args.Username();
                string password = args.Password();
                bool headless = args.Headless();

                Scraper scraper = CompositionRoot.CreateScraper(username, password, headless);
            
                Task<IEnumerable<Channel>> task = scraper.GetChannels();

                ConsoleView.ShowGettingChannels();

                task.Wait();
                
                ConsoleView.Show(task.Result);
            }
            catch (Exception e)
            {
                ConsoleView.Show(e);
            }
        }
    }
}
