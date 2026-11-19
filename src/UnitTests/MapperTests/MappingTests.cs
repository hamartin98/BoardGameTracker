using System.Reflection;
using Mapster;
using MapsterMapper;

namespace BoardGameTracker.UnitTests.MapperTests;

public partial class MappingTests
{
    private readonly IMapper Mapper;

    private readonly TypeAdapterConfig Config;

    public MappingTests()
    {
        Config = new TypeAdapterConfig();
        Config.Scan(Assembly.GetExecutingAssembly());

        Mapper = new Mapper(Config);
    }
}
