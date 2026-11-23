namespace BoardGameTracker.TestData;

/// <summary>
/// Random source that returns a set index, clamped below the requested maximum
/// </summary>
public class FixedRandom(int value) : Random
{
    public int Value { get; set; } = value;

    public override int Next(int maxValue) => Math.Min(Value, maxValue - 1);
}
