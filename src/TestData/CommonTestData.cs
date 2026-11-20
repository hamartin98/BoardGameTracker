using Xunit;

namespace BoardGameTracker.TestData;

public static class CommonTestData
{
    public static readonly Guid Guid1 = new("77934a16-a70a-4ac3-9c0e-000000000001");

    public static readonly Guid Guid2 = new("77934a16-a70a-4ac3-9c0e-200000000001");

    public static readonly Guid DeletedGuid = new("77934a16-a70a-4ac3-9c0e-900000000000");

    public static readonly Guid PlayerGuid1 = new("77934a16-a70a-4ac3-9c0e-000000000101");

    public static readonly Guid PlayerGuid2 = new("77934a16-a70a-4ac3-9c0e-000000000102");

    public static readonly DateTime DateTime = new(2026, 11, 19, 10, 11, 12, DateTimeKind.Utc);

    public static void CheckNullableProperty(object? expected, object? actual)
    {
        if (expected is not null)
        {
            Assert.NotNull(actual);
        }
        else
        {
            Assert.Null(actual);
        }
    }
}
