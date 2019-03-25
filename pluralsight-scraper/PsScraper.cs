using IronWebScraper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VH.PluralsightScraper
{
    internal class PsScraper : WebScraper
    {
        #region Overrides of WebScraper

        public override void Parse(Response response)
        {
            // santi: implement
            Console.WriteLine($"got response with final url: [{response.FinalUrl}]");
        }

        public override void Init()
        {
            LoggingLevel = LogLevel.All;
            WorkingDirectory = @"C:\Users\Santiago\Desktop\delete\scraper-output";
            ObeyRobotsDotTxt = false;

            foreach (HttpIdentity identity in
                CommonUserAgents.ChromeDesktopUserAgents.Select
                    (
                     userAgent => new HttpIdentity
                                  {
                                      NetworkUsername = "santiago@vasquezhouse.com",
                                      NetworkPassword = "aburra75",
                                      UseCookies = true,
                                      UserAgent = userAgent,
                                  }
                    )
            )
            {
                Identities.Add(identity);
            }

            EnableWebCache(TimeSpan.FromMinutes(60));
            
            Request("https://app.pluralsight.com/id/signin", ParseSignInLoad);
        }

        #endregion

        private void ParseSignInLoad(Response response)
        {
            var postVariables = new Dictionary<string, string>();

            // get all inputs
            foreach (HtmlNode inputField in response.Css("#signInForm input"))
            {
                string value;

                string id = inputField.GetAttribute("id");

                switch (id.ToLower())
                {
                    case "password":
                        value = "aburra75";
                        break;

                    case "redirecturl":
                        value = "https://app.pluralsight.com/channels";
                        break;

                    case "username":
                        value = "santiago@vasquezhouse.com";
                        break;

                    default:
                        value = inputField.GetAttribute("value");
                        break;
                }

                postVariables.Add(id, value);
            }


            HttpIdentity httpIdentity = ChooseIdentityForRequest(response.Request);
            
            PostRequest("https://app.pluralsight.com/id/", 
                        ParseSignInResult, 
                        postVariables,
                        httpIdentity);
        }

        private void ParseSignInResult(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
