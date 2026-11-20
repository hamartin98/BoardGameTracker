using BoardGameTracker.Models.Filters;
using BoardGameTracker.TestData;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;

namespace BoardGameTracker.UnitTests.FilterTests;

public class AbstractFilterTests
{
    [Fact]
    public void DefaultsTest()
    {
        var filter = new PlayerFilter();

        Assert.Equal(1, filter.Page);
        Assert.Equal(10, filter.PageSize);
    }

    [Theory]
    [InlineData(-5, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(7, 7)]
    public void PageClampTest(int page, int expected)
    {
        var filter = new PlayerFilter { Page = page };

        Assert.Equal(expected, filter.Page);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    [InlineData(101, 100)]
    [InlineData(int.MaxValue, 100)]
    public void PageSizeClampTest(int pageSize, int expected)
    {
        var filter = new PlayerFilter { PageSize = pageSize };

        Assert.Equal(expected, filter.PageSize);
    }

    [Theory]
    [InlineData(0, 0, 1, 1)]
    [InlineData(3, 500, 3, 100)]
    public void ClampAppliesWhenMappedFromVMTest(
        int page,
        int pageSize,
        int expectedPage,
        int expectedPageSize
    )
    {
        var config = new Mapster.TypeAdapterConfig();
        config.Scan(typeof(BoardGameTracker.Mappers.PlayerMapperConfig).Assembly);
        var mapper = new Mapper(config);

        var actual = mapper.Map<PlayerFilter>(
            new PlayerFilterVM { Page = page, PageSize = pageSize }
        );

        Assert.Equal(expectedPage, actual.Page);
        Assert.Equal(expectedPageSize, actual.PageSize);
    }

    public class CreatedAtTestData : TheoryData<DateTime, DateTime>
    {
        public CreatedAtTestData()
        {
            Add(CommonTestData.DateTime, CommonTestData.DateTime);
            Add(CommonTestData.DateTime.ToLocalTime(), CommonTestData.DateTime);
            Add(
                DateTime.SpecifyKind(CommonTestData.DateTime, DateTimeKind.Unspecified),
                CommonTestData.DateTime
            );
        }
    }

    [Theory]
    [ClassData(typeof(CreatedAtTestData))]
    public void CreatedAtIsStoredAsUtcTest(DateTime value, DateTime expected)
    {
        var filter = new PlayerFilter { CreatedAtFrom = value, CreatedAtTo = value };

        Assert.Equal(expected, filter.CreatedAtFrom);
        Assert.Equal(DateTimeKind.Utc, filter.CreatedAtFrom?.Kind);
        Assert.Equal(expected, filter.CreatedAtTo);
        Assert.Equal(DateTimeKind.Utc, filter.CreatedAtTo?.Kind);
    }

    [Fact]
    public void CreatedAtNullStaysNullTest()
    {
        var filter = new PlayerFilter { CreatedAtFrom = null, CreatedAtTo = null };

        Assert.Null(filter.CreatedAtFrom);
        Assert.Null(filter.CreatedAtTo);
    }
}
