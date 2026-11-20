using BoardGameTracker.ViewModels;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static PlayerResponseVM AliceVM
    {
        get =>
            new()
            {
                MetaData = new()
                {
                    Id = CommonTestData.PlayerGuid1,
                    CreatedAt = CommonTestData.DateTime,
                    UpdatedAt = CommonTestData.DateTime,
                    Version = 1,
                    IsDeleted = false,
                },
                Name = "Alice",
            };
    }

    public static void CheckPlayerResponseVM(PlayerResponseVM expected, PlayerResponseVM actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.MetaData.Id, actual.MetaData.Id);
        Assert.Equal(expected.MetaData.CreatedAt, actual.MetaData.CreatedAt);
        Assert.Equal(expected.MetaData.UpdatedAt, actual.MetaData.UpdatedAt);
        Assert.Equal(expected.MetaData.Version, actual.MetaData.Version);
        Assert.Equal(expected.MetaData.IsDeleted, actual.MetaData.IsDeleted);
    }
}
