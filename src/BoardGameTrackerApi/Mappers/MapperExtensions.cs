using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers;

public static class MapperExtensions
{
    /// <summary>
    /// Ignores the AbstractEntity members, which are owned by EF and the save interceptors
    /// </summary>
    public static TypeAdapterSetter<TSource, TDestination> IgnoreEntityMetadata<
        TSource,
        TDestination
    >(this TypeAdapterSetter<TSource, TDestination> setter)
        where TDestination : AbstractEntity
    {
        return setter.Ignore(
            dest => dest.Id,
            dest => dest.CreatedAt,
            dest => dest.UpdatedAt,
            dest => dest.Version,
            dest => dest.IsDeleted
        );
    }
}
