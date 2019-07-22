using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using VH.PluralsightScraper.Domain;
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

        public async Task<IEnumerable<ChannelDto>> GetChannels(bool isFastRun, CancellationToken cancellationToken)
        {
            var channelsList = new List<ChannelDto>();

            using (Browser browser = await _browserFactory.Create())
            using (Page page = await browser.NewPageAsync())
            {
                await Login(page);
                
                IEnumerable<string> channelUrls = await GetChannelUrls(page);

                if (isFastRun)
                {
                    channelUrls = channelUrls.Take(1);
                }

                foreach (string url in channelUrls.TakeWhile(url => !cancellationToken.IsCancellationRequested))
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
            //await channelPage.WaitForSelectorAsync(TITLE_SELECTOR);

            string jsSelectChannelName = $"Array.from(document.querySelectorAll('{TITLE_SELECTOR}')).map(h => h.innerText)[0];";

            return await page.EvaluateExpressionAsync<string>(jsSelectChannelName);
        }

        private static async Task<IEnumerable<CourseDto>> GetCourses(Page channelPage)
        {
            // todo: for some reason this times out after a few successful calls, not sure why, using a hardcoded timeout in caller method
            //const string COURSES_SELECTOR = "ul._1Ws76NZ6";
            //await channelPage.WaitForSelectorAsync(COURSES_SELECTOR);

            string jsFunctionToGetCoursesDetails = GetCourseDetailsFunction();

            CourseDto[] courses = await channelPage.EvaluateFunctionAsync<CourseDto[]>(jsFunctionToGetCoursesDetails);
            
            LogMissingData(courses, channelPage.Url);

            return courses;
        }

        private static string GetCourseDetailsFunction()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "GetCoursesDetails.js");
            string fileContents = File.ReadAllText(path);

            return fileContents.Replace("${pluralsightPathLevel}", PLURALSIGHT_PATH_LEVEL);
        }

        private static void LogMissingData(IReadOnlyCollection<CourseDto> courses, string channelPageUrl)
        {
            IEnumerable<CoursesMissingData> missingDetailsList = GetMissingDetails(courses);

            foreach (CoursesMissingData missingData in missingDetailsList)
            {
                Log.Error("courses missing data. CourseFieldName: [{CourseFieldName}] " + 
                          "MissingPercentage: [{MissingPercentage}] " + 
                          "MissingCount: [{MissingCount}] " + 
                          "TotalCoursesCount: [{TotalCoursesCount}] " + 
                          "ChannelPageUrl: [{ChannelPageUrl}]",
                          missingData.CourseFieldName,
                          missingData.MissingPercentage,
                          missingData.MissingCount,
                          missingData.TotalCoursesCount,
                          channelPageUrl);
            }
        }

        private static IEnumerable<CoursesMissingData> GetMissingDetails(IReadOnlyCollection<CourseDto> courses)
        {
            Expression<Func<CourseDto, bool>> missingNameExpression          = course => string.IsNullOrWhiteSpace(course.Name);
            Expression<Func<CourseDto, bool>> missingDatePublishedExpression = course => string.IsNullOrWhiteSpace(course.DatePublished) && course.Level != PLURALSIGHT_PATH_LEVEL;
            Expression<Func<CourseDto, bool>> missingLevelExpression         = course => string.IsNullOrWhiteSpace(course.Level);

            var courseFieldsList =
                new[]
                {
                    new { MissingDataExpression = missingNameExpression          , FieldName = nameof(CourseDto.Name)          },
                    new { MissingDataExpression = missingDatePublishedExpression , FieldName = nameof(CourseDto.DatePublished) },
                    new { MissingDataExpression = missingLevelExpression         , FieldName = nameof(CourseDto.Level)         },
                };

            int totalCoursesCount = courses.Count;

            return courseFieldsList.Select(fieldInfo =>
                                           {
                                               Func<CourseDto, bool> missingDataPredicate = fieldInfo.MissingDataExpression.Compile();

                                               int missingCount = courses.Count(missingDataPredicate);

                                               return new CoursesMissingData(fieldInfo.FieldName, 
                                                                             missingCount, 
                                                                             totalCoursesCount);
                                           })
                                   .Where(missingData => missingData.MissingCount > 0);
        }

        private static async Task<ChannelDto> GetChannelDetails(Page page, string url)
        {
            string channelName = null;

            try
            {
                await page.GoToAsync(url, WaitUntilNavigation.Networkidle2);
                
                channelName = await GetChannelName(page);

                IEnumerable<CourseDto> courses = await GetCourses(page);

                return new ChannelDto(url, channelName, courses);
            }
            catch (Exception e)
            {
                return new ChannelDto(url, channelName, scrapException: e);
            }
        }

        private async Task Login(Page page)
        {
            await page.GoToAsync("https://app.pluralsight.com/id/signin", WaitUntilNavigation.Networkidle2);

            await page.TypeAsync("#Username", _username);
            await page.TypeAsync("#Password", _password);
            await page.ClickAsync("#login");

            await page.WaitForNavigationAsync();
        }

        private static async Task<IEnumerable<string>> GetChannelUrls(Page page)
        {
            await page.GoToAsync("https://app.pluralsight.com/channels", WaitUntilNavigation.Networkidle2);

            const string CHANNEL_LINKS_SELECTOR = "li.N4XCI-0S a";

            await page.WaitForSelectorAsync(CHANNEL_LINKS_SELECTOR);

            string jsSelectAllAnchors = $"Array.from(document.querySelectorAll('{CHANNEL_LINKS_SELECTOR}')).map(a => a.href);";

            return await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
        }

        private const string PLURALSIGHT_PATH_LEVEL = "pluralsight-path";

        private readonly BrowserFactory _browserFactory;
        private readonly string _username;
        private readonly string _password;
    }
}
