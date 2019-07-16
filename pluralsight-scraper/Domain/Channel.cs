using System;
using System.Collections.Generic;
using System.Linq;
using VH.PluralsightScraper.Dtos;

// santi: [next] Read brent ozar "the case of entity framework core's odd sql"

namespace VH.PluralsightScraper.Domain
{
    internal class Channel : IDomainEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public List<ChannelCourse> ChannelCourses { get; private set; }

        public Channel(ChannelDto channelDto)
        {
            Name = channelDto.Name;
            Url = channelDto.Url;

            ChannelCourses = channelDto.Courses
                                       .Select(Course.ConvertFromDto)
                                       .ToList();
        }

        public Channel(string name, string url, IEnumerable<Course> courses)
        {
            Name = name;
            Url = url;
            ChannelCourses = courses.Select(c => new ChannelCourse(c)).ToList();
        }
        
        public bool Merge(ChannelDto channel)
        {
            var changed = false;

            if (Url != channel.Url)
            {
                Url = channel.Url;
                changed = true;
            }

            ChannelCourse[] missingCourses = 
                channel.Courses
                       .Where(courseDto => ChannelCourses.All(c => !string.Equals(c.Course.Name,
                                                                                  courseDto.Name,
                                                                                  StringComparison.CurrentCultureIgnoreCase)))
                       .Select(Course.ConvertFromDto)
                       .ToArray();

            ChannelCourses.AddRange(missingCourses);

            if (missingCourses.Any())
            {
                changed = true;
            }
            
            IEnumerable<ChannelCourse> extraCourses =
                ChannelCourses.Where(c => channel.Courses.All(courseDto => !string.Equals(courseDto.Name,
                                                                                          c.Course.Name,
                                                                                          StringComparison.CurrentCultureIgnoreCase)));

            foreach (ChannelCourse c in extraCourses)
            {
                ChannelCourses.Remove(c);
                changed = true;
            }
            
            IEnumerable<(ChannelCourse course, CourseDto courseDto)> existingCourses =
                ChannelCourses.Join(channel.Courses,
                                    channelCourse => channelCourse.Course.Name.ToLower(),
                                    courseDto => courseDto.Name.ToLower(),
                                    (c, dto) => ( c, dto ));

            foreach ((ChannelCourse channelCourse, CourseDto courseDto) in existingCourses)
            {
                bool courseChanged = channelCourse.Course.Merge(courseDto);

                if (courseChanged)
                {
                    changed = true;
                }
            }

            return changed;
        }

        // ReSharper disable once UnusedMember.Local
        private Channel()
        {
            // empty for entity framework
            ChannelCourses = new List<ChannelCourse>();
        }
    }
}
