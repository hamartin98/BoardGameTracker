namespace BoardGameTracker.Interfaces.Services;

public interface ICrudService<ModelType, FilterType>
    where ModelType : class
    where FilterType : class
{
    Task<ModelType> GetByIdAsync(Guid id, int version, CancellationToken cancellationToken);

    Task<ModelType> CreateAsync(ModelType model, CancellationToken cancellationToken);

    Task<ModelType> UpdateAsync(
        ModelType model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<ModelType>> GetAllAsync(
        FilterType filter,
        CancellationToken cancellationToken
    );

    Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken);
}
