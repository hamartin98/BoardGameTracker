using BoardGameTracker.Database;
using BoardGameTracker.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class TestDbContextFactory
{
    private readonly DbContextOptions<BoardGameContext> Options;

    public TestDbContextFactory(TimeProvider? timeProvider = null)
    {
        Options = new DbContextOptionsBuilder<BoardGameContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .AddInterceptors(
                new CreateInterceptor(timeProvider ?? TimeProvider.System),
                new UpdateInterceptor(timeProvider ?? TimeProvider.System),
                new SoftDeleteInterceptor(timeProvider ?? TimeProvider.System)
            )
            .Options;
    }

    /// <summary>
    /// New context on the shared database, like a new request
    /// </summary>
    public BoardGameContext Create()
    {
        return new BoardGameContext(Options);
    }
}
