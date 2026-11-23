namespace BoardGameTracker.SeedData;

/// <summary>
/// Ids and time used by the seeded entities
/// </summary>
public static class SeedConstants
{
    public static readonly Guid Guid1 = new("77934a16-a70a-4ac3-9c0e-000000000001");

    public static readonly Guid Guid2 = new("77934a16-a70a-4ac3-9c0e-200000000001");

    public static readonly Guid Guid3 = new("77934a16-a70a-4ac3-9c0e-300000000001");

    public static readonly Guid DeletedGuid = new("77934a16-a70a-4ac3-9c0e-900000000000");

    public static readonly DateTime DateTime = new(2026, 11, 19, 10, 11, 12, DateTimeKind.Utc);
}
