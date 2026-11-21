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

    public static PlayerCreateVM CarolCreateVM
    {
        get => new() { Name = "Carol" };
    }

    public static PlayerCreateVM AliceUppercaseCreateVM
    {
        get => new() { Name = "ALICE" };
    }

    public static PlayerCreateVM EmptyNameCreateVM
    {
        get => new() { Name = "" };
    }

    public static PlayerUpdateVM AliceRenamedUpdateVM
    {
        get => new() { Name = "Alice Renamed" };
    }

    public static PlayerUpdateVM AliceUppercaseUpdateVM
    {
        get => new() { Name = "ALICE" };
    }

    public static PlayerUpdateVM AliceRenamedToBobUpdateVM
    {
        get => new() { Name = "bob" };
    }

    public static void CheckPlayerVM(PlayerBaseVM expected, PlayerBaseVM actual)
    {
        Assert.Equal(expected.Name, actual.Name);
    }

    public static void CheckPlayerResponseVM(PlayerResponseVM expected, PlayerResponseVM actual)
    {
        CheckPlayerVM(expected, actual);
        CheckResponseMetaDataVM(expected.MetaData, actual.MetaData);
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
