using System.Collections.Generic;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Tests.Unit.Plumbing
{
    internal class CourseFactory
    {
        public ChannelCourse ConvertFromDto(CourseDto courseDto)
        {
            string courseNameKey = courseDto.CourseName.ToLower();

            if (_channelCoursesMap.TryGetValue(courseNameKey, out ChannelCourse channelCourse))
            {
                return channelCourse;
            }
            
            ChannelCourse newChannelCourse = Course.ConvertFromDto(courseDto);

            _channelCoursesMap.Add(courseNameKey, newChannelCourse);

            return newChannelCourse;
        }

        private readonly Dictionary<string, ChannelCourse> _channelCoursesMap = new Dictionary<string, ChannelCourse>();
    }
}
