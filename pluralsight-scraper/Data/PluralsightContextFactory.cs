using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VH.PluralsightScraper.Authentication;

namespace VH.PluralsightScraper.Data
{
    /// <summary>
    /// used at design time by migrations
    /// </summary>
    internal class PluralsightContextFactory : IDesignTimeDbContextFactory<PluralsightContext>
    {
        #region Implementation of IDesignTimeDbContextFactory<out PluralsightContext>

        public PluralsightContext CreateDbContext(string[] args)
        {
            const string FAKE_CONNECTION_STRING = "host=foo-host;port=5432;database=foo-database;user id=foo-user;password=foo-password";

            DbContextOptions options = new DbContextOptionsBuilder<PluralsightContext>().UseNpgsql(FAKE_CONNECTION_STRING).Options;

            var session = new WindowsSession();
            
            return new PluralsightContext(options, session);
        }

        #endregion
    }
}
