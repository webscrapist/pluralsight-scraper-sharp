using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Data
{
    internal static class ModelBuilderExtensions
    {
        public static void CreateTrackingPropertiesFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
        {
            IEnumerable<Type> domainTypes = assembly.GetTypes()
                                                    .Where(t => t.GetInterfaces().Contains(typeof(IDomainEntity)));

            foreach (Type type in domainTypes)
            {
                modelBuilder.Entity(type).Property<string>(TrackingProperties.CREATED_BY).HasMaxLength(50).IsRequired();
                modelBuilder.Entity(type).Property<DateTime>(TrackingProperties.CREATED_AT_UTC).IsRequired();

                modelBuilder.Entity(type).Property<string>(TrackingProperties.LAST_UPDATED_BY).HasMaxLength(50);
                modelBuilder.Entity(type).Property<DateTime?>(TrackingProperties.LAST_UPDATED_AT_UTC);
            }
        }
    }
}
