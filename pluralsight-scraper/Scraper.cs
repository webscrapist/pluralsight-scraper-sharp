using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper
{
    internal class Scraper
    {
        public Scraper(BrowserFactory browserFactory, string username, string password)
        {
            _browserFactory = browserFactory ?? throw new ArgumentNullException(nameof(browserFactory));
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public async Task<IEnumerable<ChannelDto>> GetChannels(CancellationToken cancellationToken)
        {
            var channelsList = new List<ChannelDto>();

            using (Browser browser = await _browserFactory.Create())
            using (Page page = await browser.NewPageAsync())
            {
                await Login(page);
                
                IEnumerable<string> channelUrls =  await GetChannelUrls(page);

                // santi: [next] consider doing this in parallel
                foreach (string url in channelUrls)
                {
                    ChannelDto channel = await GetChannelDetails(page, url);
                    channelsList.Add(channel);
                }
            }

            return channelsList;
        }

        private static async Task<string> GetChannelName(Page page)
        {
            const string TITLE_SELECTOR = "h1";

            // todo: for some reason this times out after a few successful calls, not sure why, using a hardcoded timeout in caller method
            //await page.WaitForSelectorAsync(TITLE_SELECTOR);

            // santi: [next] refactor using page.QuerySelectorAllAsync()
            string jsSelectChannelName = $@"Array.from(document.querySelectorAll('{TITLE_SELECTOR}')).map(h => h.innerText)[0];";

            return await page.EvaluateExpressionAsync<string>(jsSelectChannelName);
        }

        private static async Task<IEnumerable<CourseDto>> GetCourses(Page page)
        {
            // todo: for some reason this times out after a few successful calls, not sure why, using a hardcoded timeout in caller method
            //const string COURSES_SELECTOR = "ul._1Ws76NZ6";
            //await page.WaitForSelectorAsync(COURSES_SELECTOR);

            // santi: [next] consider moving this to a js file
            const string JS_FUNCTION_TO_GET_COURSES_DETAILS = @"() => {
  const selectors = Array.from(document.querySelectorAll('div.css-kxulf3 a'));

  return selectors.map(s => {
    const courseName = s.innerText;

    const subSelectors = Array.from(s.parentNode.parentNode.parentNode.parentNode.querySelectorAll('span.css-1kcrbi9'));

    const isPluralsightPath = subSelectors.length == 2;

    if (isPluralsightPath) {
      return { 
        courseName: courseName, 
        courseLevel: 'pluralsight-path',
        courseDate: ''
      }
    }

    const isPluralsightCourse = subSelectors.length == 3;

    if (isPluralsightCourse) {
      return { 
        courseName: courseName, 
        courseLevel: subSelectors[2].innerText,
        courseDate: subSelectors[3].innerText
      }
    }

    return { 
      courseName: courseName, 
      courseLevel: 'unknown',
      courseDate: ''
    }
  });
}";

            // santi: IT SEEMS THAT SELECTORS ARE NOT CAPTURING COURSE DATA

            return await page.EvaluateFunctionAsync<CourseDto[]>(JS_FUNCTION_TO_GET_COURSES_DETAILS);
        }

        private static async Task<ChannelDto> GetChannelDetails(Page page, string url)
        {
            string channelName = null;

            try
            {
                await page.GoToAsync(url);

                await page.WaitForTimeoutAsync(milliseconds: 1000);

                channelName = await GetChannelName(page);

                IEnumerable<CourseDto> courses = await GetCourses(page);  // santi: to array

                return new ChannelDto(url, channelName, courses);
            }
            catch (Exception e)
            {
                return new ChannelDto(url, channelName, scrapException: e);
            }
        }

        private async Task Login(Page page)
        {
            await page.GoToAsync("https://app.pluralsight.com/id/signin");

            await page.TypeAsync("#Username", _username);
            await page.TypeAsync("#Password", _password);
            await page.ClickAsync("#login");

            await page.WaitForNavigationAsync();
        }

        private static async Task<IEnumerable<string>> GetChannelUrls(Page page)
        {
            await page.GoToAsync("https://app.pluralsight.com/channels");

            const string CHANNEL_LINKS_SELECTOR = "li.N4XCI-0S a";

            await page.WaitForSelectorAsync(CHANNEL_LINKS_SELECTOR);

            string jsSelectAllAnchors = $@"Array.from(document.querySelectorAll('{CHANNEL_LINKS_SELECTOR}')).map(a => a.href);";

            return await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
        }

        private readonly BrowserFactory _browserFactory;
        private readonly string _username;
        private readonly string _password;
    }
}
