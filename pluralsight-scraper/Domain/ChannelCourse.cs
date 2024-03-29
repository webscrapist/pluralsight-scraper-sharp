﻿namespace VH.PluralsightScraper.Domain
{
    internal class ChannelCourse : IDomainEntity
    {
        public int Id { get; private set; }
        public int ChannelId { get; private set; }
        public int CourseId { get; private set; }
        
        public Channel Channel { get; private set; }

        public Course Course
        {
            get => _course;

            set
            {
                _course = value;

                if (value != null)
                {
                    CourseId = value.Id;
                }
            }
        }

        public ChannelCourse(Channel channel, Course course)
        {
            Channel = channel;
            Course = course;
        }

        private ChannelCourse()
        {
            // empty for entity framework
        }

        private Course _course;
    }
}
