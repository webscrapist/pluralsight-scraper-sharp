using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Data.EFConfigs
{
    internal class ChannelCourseConfiguration : IEntityTypeConfiguration<ChannelCourse>
    {
        #region Implementation of IEntityTypeConfiguration<ChannelCourse>

        public void Configure(EntityTypeBuilder<ChannelCourse> builder)
        {
            builder.Property(channelCourse => channelCourse.ChannelId).IsRequired();
            builder.Property(channelCourse => channelCourse.CourseId).IsRequired();

            builder.HasIndex(channelCourse => new
                                              {
                                                  channelCourse.ChannelId, 
                                                  channelCourse.CourseId
                                              })
                   .IsUnique();
        }

        #endregion
    }
}