using System.Collections.Generic;
using System.Linq;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Domain
{
    internal class Channel : IDomainEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public List<ChannelCourse> ChannelCourses { get; private set; }

        public Channel(ChannelDto channelDto, EntityFactory entityFactory)
        {
            Name = channelDto.Name;
            Url = channelDto.Url;

            ChannelCourses = channelDto.Courses
                                       .Select(courseDto =>
                                               {
                                                   Course course = entityFactory.CreateCourse(courseDto);
                                                   return entityFactory.CreateChannelCourse(channel: this, course);
                                               })
                                       .ToList();
        }

        public Channel(string name, string url, IEnumerable<Course> coursesList, EntityFactory entityFactory)
        {
            Name = name;
            Url = url;

            ChannelCourses = coursesList.Select(course => entityFactory.CreateChannelCourse(channel: this, course))
                                        .ToList();
        }
        
        public bool Merge(ChannelDto channel, EntityFactory entityFactory)
        {
            var changed = false;

            if (Url != channel.Url)
            {
                Url = channel.Url;
                changed = true;
            }

            ChannelCourse[] missingCourses = FindMissingCourses(channel, entityFactory);
            ChannelCourse[] extraCourses = FindExtraCourses(channel).ToArray();
            (ChannelCourse c, CourseDto dto)[] existingCourses = FindExistingCourses(channel).ToArray();

            if (missingCourses.Any())
            {
                ChannelCourses.AddRange(missingCourses);
                changed = true;
            }

            foreach (ChannelCourse c in extraCourses)
            {
                ChannelCourses.Remove(c);
                changed = true;
            }
            
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

        private IEnumerable<(ChannelCourse c, CourseDto dto)> FindExistingCourses(ChannelDto channel)
        {
            return ChannelCourses.Join(channel.Courses,
                                       channelCourse => channelCourse.Course.ComparisonKey,
                                       courseDto => courseDto.ComparisonKey,
                                       (c, dto) => ( c, dto ))
                                 .ToArray();
        }

        private IEnumerable<ChannelCourse> FindExtraCourses(ChannelDto channel)
        {
            return ChannelCourses.Where(cc => channel.Courses.All(courseDto => cc.Course.ComparisonKey != courseDto.ComparisonKey))
                                 .ToArray();
        }

        private ChannelCourse[] FindMissingCourses(ChannelDto channel, EntityFactory entityFactory)
        {
            return channel.Courses

                          .Where(courseDto => ChannelCourses.All(cc => cc.Course.ComparisonKey != courseDto.ComparisonKey))
                          
                          .Select(courseDto =>
                                  {
                                      Course course = entityFactory.CreateCourse(courseDto);
                                      return entityFactory.CreateChannelCourse(channel: this, course);
                                  })
                          
                          .ToArray();
        }

        private Channel()
        {
            // empty for entity framework
            ChannelCourses = new List<ChannelCourse>();
        }
    }
}
