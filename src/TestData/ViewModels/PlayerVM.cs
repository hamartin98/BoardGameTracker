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

    public static PlayerResponseVM BobVM
    {
        get => new() { MetaData = ResponseMetaDataVM2, Name = "Bob" };
    }

    public static PlayersResponseVM PlayersPage2VM
    {
        get =>
            new()
            {
                Page = 2,
                PageSize = 2,
                TotalCount = 5,
                Items = [AliceVM, BobVM],
            };
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

    public static void CheckPlayersResponseVM(PlayersResponseVM expected, PlayersResponseVM actual)
    {
        Assert.Equal(expected.Page, actual.Page);
        Assert.Equal(expected.PageSize, actual.PageSize);
        Assert.Equal(expected.TotalCount, actual.TotalCount);
        Assert.Equal(expected.Items.Count, actual.Items.Count);
        for (int i = 0; i < expected.Items.Count; i++)
        {
            CheckPlayerResponseVM(expected.Items[i], actual.Items[i]);
        }
    }
}
