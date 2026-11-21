namespace BoardGameTracker.UnitTests.ServiceTests;

public class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
