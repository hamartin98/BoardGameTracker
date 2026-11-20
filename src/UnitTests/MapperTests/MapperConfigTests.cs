using System.Reflection;
using BoardGameTracker.Mappers;
using Mapster;

namespace BoardGameTracker.UnitTests.MapperTests;

public class MapperConfigTests
{
    private static readonly Assembly MapperAssembly = typeof(BoardGameMapperConfig).Assembly;

    [Fact]
    public void ConfigShouldCompileWithoutErrors()
    {
        var config = new TypeAdapterConfig();
        config.Scan(MapperAssembly);

        var exception = Record.Exception(() => config.Compile());
        Assert.Null(exception);
    }

    [Fact]
    public void EveryDestinationMemberShouldHaveSourceOrBeIgnored()
    {
        var config = new TypeAdapterConfig { RequireDestinationMemberSource = true };
        config.Scan(MapperAssembly);

        List<string> errors = [];
        foreach (var types in config.RuleMap.Keys)
        {
            try
            {
                config.Compile(types.Source, types.Destination);
            }
            catch (Exception exception)
            {
                errors.Add(
                    $"{types.Source.Name} -> {types.Destination.Name}: {exception.InnerException?.Message ?? exception.Message}"
                );
            }
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }
}
