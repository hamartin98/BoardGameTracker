using BoardGameTracker.ViewModels;
using Xunit;

namespace BoardGameTracker.TestData.ViewModels;

public partial class ViewModelsTestData
{
    public static ResponseMetaDataVM ResponseMetaDataVM1
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid1,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                Version = 1,
                IsDeleted = false,
            };
    }

    public static ResponseMetaDataVM ResponseMetaDataVM2
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid2,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                Version = 1,
                IsDeleted = false,
            };
    }

    public static ResponseMetaDataVM ResponseMetaDataVM3
    {
        get =>
            new()
            {
                Id = CommonTestData.Guid3,
                CreatedAt = CommonTestData.DateTime,
                UpdatedAt = CommonTestData.DateTime,
                Version = 1,
                IsDeleted = false,
            };
    }

    public static void CheckResponseMetaDataVM(
        ResponseMetaDataVM expected,
        ResponseMetaDataVM actual
    )
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
        Assert.Equal(expected.Version, actual.Version);
        Assert.Equal(expected.IsDeleted, actual.IsDeleted);
    }
}
