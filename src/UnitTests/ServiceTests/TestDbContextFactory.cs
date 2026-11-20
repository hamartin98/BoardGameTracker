using BoardGameTracker.Database;
using BoardGameTracker.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardGameTracker.UnitTests.ServiceTests;

public class TestDbContextFactory
{
    private readonly DbContextOptions<BoardGameContext> Options;

    public TestDbContextFactory()
    {
        Options = new DbContextOptionsBuilder<BoardGameContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .AddInterceptors(new UpdateInterceptor(), new SoftDeleteInterceptor())
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
