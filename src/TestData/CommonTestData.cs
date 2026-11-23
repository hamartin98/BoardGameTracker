using BoardGameTracker.SeedData;
using Xunit;

namespace BoardGameTracker.TestData;

public static class CommonTestData
{
    public static readonly Guid Guid1 = SeedConstants.Guid1;

    public static readonly Guid Guid2 = SeedConstants.Guid2;

    public static readonly Guid Guid3 = SeedConstants.Guid3;

    public static readonly Guid DeletedGuid = SeedConstants.DeletedGuid;

    public static readonly DateTime DateTime = SeedConstants.DateTime;

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
