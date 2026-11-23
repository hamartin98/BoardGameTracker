using BoardGameTracker.Models.Imports;
using BoardGameTracker.ViewModels;
using Mapster;

namespace BoardGameTracker.Mappers;

public class ImportMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ImportCounts, ImportCountsVM>();
        config.NewConfig<ImportResult, ImportResultVM>();
    }
}
