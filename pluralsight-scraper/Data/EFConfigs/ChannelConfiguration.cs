using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Data.EFConfigs
{
    internal class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        #region Implementation of IEntityTypeConfiguration<Channel>

        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => new { c.Name })
                   .IsUnique();

            builder.Property(c => c.Url)
                   .IsRequired()
                   .HasMaxLength(100);
        }

        #endregion
    }
}
