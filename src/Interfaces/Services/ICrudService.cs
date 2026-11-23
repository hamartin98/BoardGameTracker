using BoardGameTracker.Models;

namespace BoardGameTracker.Interfaces.Services;

/// <summary>
/// CRUD operations whose create and update take their own input types
/// </summary>
public interface ICrudService<ModelType, FilterType, CreateType, UpdateType>
    where ModelType : class
    where FilterType : class
    where CreateType : class
    where UpdateType : class
{
    Task<ModelType> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ModelType> CreateAsync(CreateType model, CancellationToken cancellationToken);

    Task<ModelType> UpdateAsync(
        UpdateType model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    );

    Task<PaginatedResponse<ModelType>> GetAllAsync(
        FilterType filter,
        CancellationToken cancellationToken
    );

    Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken);
}

/// <summary>
/// CRUD operations whose create and update take the entity itself
/// </summary>
public interface ICrudService<ModelType, FilterType>
    : ICrudService<ModelType, FilterType, ModelType, ModelType>
    where ModelType : class
    where FilterType : class { }
