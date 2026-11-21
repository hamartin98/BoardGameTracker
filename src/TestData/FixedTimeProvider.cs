namespace BoardGameTracker.TestData;

/// <summary>
/// Clock that returns a set time until it is changed
/// </summary>
public class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;

    public override DateTimeOffset GetUtcNow() => UtcNow;
}
