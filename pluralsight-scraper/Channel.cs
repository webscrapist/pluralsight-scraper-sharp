using System;
using System.Collections.Generic;

namespace VH.PluralsightScraper
{
    internal class Channel
    {
        public string ChannelName { get; }

        public IEnumerable<Course> Courses { get; }

        public string Url { get; }

        public Exception ScrapException { get; }

        public Channel(string url, Exception scrapException)
            : this(url, scrapException, channelName: null, courses: null)
        {
            // empty
        }

        public Channel(string url, string channelName, Exception scrapException)
            : this(url, scrapException, channelName, courses: null)
        {
            // empty
        }

        public Channel(string url, string channelName, IEnumerable<Course> courses)
            : this(url, null, channelName, courses)
        {
            // empty
        }

        private Channel(string url,
                        Exception scrapException,
                        string channelName,
                        IEnumerable<Course> courses)
        {
            ChannelName = channelName;
            Courses = courses;
            Url = url;
            ScrapException = scrapException;
        }
    }
}
