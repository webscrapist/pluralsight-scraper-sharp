using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VH.PluralsightScraper.Authentication;

namespace VH.PluralsightScraper.Data
{
    /// <summary>
    /// used by migrations
    /// </summary>
    internal class PluralsightContextFactory : IDesignTimeDbContextFactory<PluralsightContext>
    {
        #region Implementation of IDesignTimeDbContextFactory<out PluralsightContext>

        public PluralsightContext CreateDbContext(string[] args)
        {
            string postgreSqlConnString = Program.Configuration.GetPostgreSqlConnString();

            DbContextOptions options = new DbContextOptionsBuilder<PluralsightContext>()
                                       .UseNpgsql(postgreSqlConnString)
                                       .Options;

            var session = new WindowsSession();
            
            return new PluralsightContext(options, session);
        }

        #endregion
    }
}
