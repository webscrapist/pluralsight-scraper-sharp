using System;
using System.Collections.Generic;
using Serilog;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Domain
{
    internal class Course : IDomainEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public CourseLevel Level { get; private set; }
        public DateTime? DatePublished { get; private set; }
        public ICollection<ChannelCourse> CourseChannels { get; private set; }

        public Course(string name, CourseLevel level, DateTime? datePublished)
        {
            Name = name;
            Level = level;
            DatePublished = datePublished;
            CourseChannels = new List<ChannelCourse>();
        }
        
        public static ChannelCourse ConvertFromDto(CourseDto courseDto)
        {
            string name = courseDto.Name;
            CourseLevel level = courseDto.Level.ToEnum();

            DateTime? datePublished = null;

            if (courseDto.DatePublished == null)
            {
                Log.Warning("missing date for course: [{CourseName}]", name);
            }
            else
            {
                if (DateTime.TryParse(courseDto.DatePublished, out DateTime tempDatePublished))
                {
                    datePublished = tempDatePublished;
                }
                else
                {
                    Log.Warning("invalid date [{DatePublished}] for course: [{CourseName}]", courseDto.DatePublished, name);
                }
            }

            var course = new Course(name, level, datePublished);

            return new ChannelCourse(course);
        }

        public bool Merge(CourseDto courseDto)
        {
            var changed = false;

            DateTime newDatePublished = DateTime.Parse(courseDto.DatePublished);

            if (DatePublished != newDatePublished)
            {
                DatePublished = newDatePublished;
                changed = true;
            }

            CourseLevel newLevel = courseDto.Level.ToEnum();

            if (Level == newLevel)
            {
                return changed;
            }

            Level = newLevel;

            return true;
        }

        public void Merge(Course otherCourse)
        {
            Name = otherCourse.Name;
            Level = otherCourse.Level;
            DatePublished = otherCourse.DatePublished;
        }
    }
}
