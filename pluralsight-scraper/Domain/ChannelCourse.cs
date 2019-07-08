using System;

namespace VH.PluralsightScraper.Domain
{
    internal class ChannelCourse : EntityBase
    {
        public Channel Channel { get; private set; }

        public Course Course
        {
            get => _course;

            set
            {
                _course = value ?? throw new ArgumentNullException(nameof(value));
                CourseId = value.Id;
            }
        }

        public int ChannelId { get; private set; }

        public int CourseId { get; private set; }

        public ChannelCourse(Course course)
        {
            Course = course;
        }

        private ChannelCourse()
        {
            // empty for entity framework
        }

        private Course _course;
    }
}
