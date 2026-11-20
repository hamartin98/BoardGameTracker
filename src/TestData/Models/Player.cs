using BoardGameTracker.Models.Entities;
using Xunit;

namespace BoardGameTracker.TestData.Models;

public partial class ModelsTestData
{
    public static Player Alice
    {
        get =>
            new()
            {
                Id = CommonTestData.PlayerGuid1,
                Name = "Alice",
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
            };
    }

    public static Player Bob
    {
        get =>
            new()
            {
                Id = CommonTestData.PlayerGuid2,
                Name = "Bob",
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
            };
    }

    public static void CheckPlayer(Player expected, Player actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.NormalizedName, actual.NormalizedName);
    }
}
