using BoardGameTracker.Mappers;
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
        Config.Scan(typeof(BoardGameMapperConfig).Assembly);

        Mapper = new Mapper(Config);
    }
}
