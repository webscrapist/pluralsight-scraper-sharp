using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Data.EFConfigs
{
    internal class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        #region Implementation of IEntityTypeConfiguration<Course>

        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => new { c.Name, c.DatePublished })
                   .IsUnique();
        }

        #endregion
    }
}
