using System;
using System.Collections.Generic;
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

        public string ComparisonKey => CourseDto.CreateComparisonKey(Name, DatePublished);

        public Course(string name, CourseLevel level, DateTime? datePublished)
        {
            Name = name;
            Level = level;
            DatePublished = datePublished;
            CourseChannels = new List<ChannelCourse>();
        }
        
        public bool Merge(CourseDto courseDto)
        {
            var changed = false;

            DateTime? newDatePublished = courseDto.DatePublishedParsed;
                
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
