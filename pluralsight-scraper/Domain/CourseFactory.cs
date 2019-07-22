using System.Collections.Generic;
using Serilog;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Domain
{
    internal class EntityFactory
    {
        public Course CreateCourse(CourseDto courseDto)
        {
            if (_coursesMap.TryGetValue(courseDto.ComparisonKey, out Course course))
            {
                return course;
            }

            string name = courseDto.Name;
            CourseLevel level = courseDto.Level.ToEnum();

            if (string.IsNullOrWhiteSpace(courseDto.DatePublished))
            {
                Log.Warning("missing date for course: [{CourseName}]", name);
            }
            else
            {
                if (courseDto.DatePublishedParsed == null)
                {
                    Log.Warning("invalid date [{DatePublished}] for course: [{CourseName}]", courseDto.DatePublished, name);
                }
            }

            course = new Course(name, level, courseDto.DatePublishedParsed);

            _coursesMap.Add(courseDto.ComparisonKey, course);

            return course;
        }

        public ChannelCourse CreateChannelCourse(Channel channel, Course course)
        {
            string key = CreateChannelCourseKey(channel, course);

            if (_channelCoursesMap.TryGetValue(key, out ChannelCourse channelCourse))
            {
                return channelCourse;
            }

            channelCourse = new ChannelCourse(channel, course);

            _channelCoursesMap.Add(key, channelCourse);

            return channelCourse;
        }

        private static string CreateChannelCourseKey(Channel channel, Course course)
        {
            return $"[{channel.Name.ToLower()}]{course.ComparisonKey}";
        }

        private readonly Dictionary<string, Course> _coursesMap = new Dictionary<string, Course>();
        private readonly Dictionary<string, ChannelCourse> _channelCoursesMap = new Dictionary<string, ChannelCourse>();
    }
}
