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
            builder.Property(channel => channel.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(channel => new { channel.Name })
                   .IsUnique();

            builder.Property(channel => channel.Url)
                   .IsRequired()
                   .HasMaxLength(100);
        }

        #endregion
    }
}
