using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Enums;
using BoardGameTracker.ViewModels.Filters;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static PlayerResponseVM AliceVM
    {
        get => new() { MetaData = ResponseMetaDataVM1, Name = "Alice" };
    }

    public static PlayerCreateVM AliceCreateVM
    {
        get => new() { Name = "Alice" };
    }

    public static PlayerFilterVM AliFilterVM
    {
        get =>
            new()
            {
                Name = "ali",
                Page = 2,
                PageSize = 20,
                OrderBy = "name",
                OrderDirection = OrderByDirectionVM.DESC,
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
