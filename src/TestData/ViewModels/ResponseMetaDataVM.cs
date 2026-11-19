using BoardGameTracker.ViewModels;

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
}
