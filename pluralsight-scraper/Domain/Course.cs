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
        public DateTime Date { get; private set; }
        public ICollection<ChannelCourse> CourseChannels { get; private set; }

        public Course(string name, CourseLevel level, DateTime date)
        {
            Name = name;
            Level = level;
            Date = date;
            CourseChannels = new List<ChannelCourse>();
        }
        
        public static ChannelCourse ConvertFromDto(CourseDto dto)
        {
            string name = dto.CourseName;
            CourseLevel level = dto.CourseLevel.ToEnum();
            DateTime date = DateTime.Parse(dto.CourseDate);

            var course = new Course(name, level, date);

            return new ChannelCourse(course);
        }

        public bool Merge(CourseDto dto)
        {
            var changed = false;

            DateTime newDate = DateTime.Parse(dto.CourseDate);

            if (Date != newDate)
            {
                Date = newDate;
                changed = true;
            }

            CourseLevel newLevel = dto.CourseLevel.ToEnum();

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
            Date = otherCourse.Date;
        }
    }
}
