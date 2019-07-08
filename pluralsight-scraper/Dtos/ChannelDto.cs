using System;
using System.Collections.Generic;

namespace VH.PluralsightScraper.Dtos
{
    internal class ChannelDto
    {
        public string Name { get; }

        public string Url { get; }

        public Exception ScrapException { get; }

        public IEnumerable<CourseDto> Courses { get; }

        public ChannelDto(string url, string channelName, Exception scrapException)
            : this(url, scrapException, channelName, courses: null)
        {
            // empty
        }

        public ChannelDto(string url, string channelName, IEnumerable<CourseDto> courses)
            : this(url, null, channelName, courses)
        {
            // empty
        }

        private ChannelDto(string url,
                           Exception scrapException,
                           string name,
                           IEnumerable<CourseDto> courses)
        {
            Name = name;
            Courses = courses;
            Url = url;
            ScrapException = scrapException;
        }
    }
}