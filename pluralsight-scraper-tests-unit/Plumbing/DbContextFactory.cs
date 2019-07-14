using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using VH.PluralsightScraper.Authentication;
using VH.PluralsightScraper.Data;

namespace VH.PluralsightScraper.Tests.Unit.Plumbing
{
    internal class DbContextFactory
    {
        public async Task<PluralsightContext> Create(CancellationToken cancellationToken)
        {
            var session = new Mock<ISession>(MockBehavior.Strict);
            session.Setup(_ => _.CurrentUser).Returns("in-memory-unit-test");

            if (_dbContextOptions != null)
            {
                return new PluralsightContext(_dbContextOptions, session.Object);
            }

            _dbContextOptions = await CreateInMemoryContextOptions(cancellationToken);

            var context = new PluralsightContext(_dbContextOptions, session.Object);
                
            await context.Database.EnsureCreatedAsync(cancellationToken);

            return context;
        }

        private static async Task<DbContextOptions> CreateInMemoryContextOptions(CancellationToken cancellationToken)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync(cancellationToken);

            ILoggerFactory serilogFactory = new LoggerFactory().AddSerilog();

            return new DbContextOptionsBuilder<PluralsightContext>().EnableSensitiveDataLogging()
                                                                    .UseSqlite(connection)
                                                                    .UseLoggerFactory(serilogFactory)
                                                                    .Options;
        }
        
        private DbContextOptions _dbContextOptions;
    }
}
