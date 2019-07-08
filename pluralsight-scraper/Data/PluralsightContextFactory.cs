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
            // santi: [next] implement
            // read from config
            const string CONNECTION_STRING = "host=hostname;port=5432;database=databaseName;user id=userName;password=secret";

            DbContextOptions options = new DbContextOptionsBuilder<PluralsightContext>().UseNpgsql(CONNECTION_STRING).Options;

            var session = new WindowsSession();
            
            return new PluralsightContext(options, session);
        }

        #endregion
    }
}
