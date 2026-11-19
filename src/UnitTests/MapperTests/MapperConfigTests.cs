using System.Reflection;
using Mapster;

namespace BoardGameTracker.UnitTests.MapperTests;

public class MapperConfigTests
{
    private readonly TypeAdapterConfig Config;

    public MapperConfigTests()
    {
        Config = new TypeAdapterConfig();
        Config.Scan(Assembly.GetExecutingAssembly());
    }

    [Fact]
    public void ConfigShouldCompileWithoutErrors()
    {
        var exception = Record.Exception(() => Config.Compile());
        Assert.Null(exception);
    }

    [Fact]
    public void ConfigShouldThrowErrorWitMissingSourceMapping()
    {
        var config = new TypeAdapterConfig { RequireDestinationMemberSource = true };
        config.Scan(Assembly.GetExecutingAssembly());

        // Will throw if destination members missing a source mapping
        config.Compile();
    }
}
