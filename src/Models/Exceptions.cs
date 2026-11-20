namespace BoardGameTracker.Models;

public class VersionMismatchException : Exception
{
    public VersionMismatchException(
        string? message = "Version mismatch.",
        Exception? innerException = null
    )
        : base(message, innerException) { }
}

/// <summary>
/// Non-generic base so handlers can match any not-found exception regardless of entity type.
/// </summary>
public abstract class EntityNotFoundException : Exception
{
    protected EntityNotFoundException(string message)
        : base(message) { }
}

public class EntityNotFoundException<EntityType> : EntityNotFoundException
    where EntityType : class
{
    public EntityNotFoundException()
        : base($"{typeof(EntityType).Name} not found.") { }
}

/// <summary>
/// Non-generic base so handlers can match any invalid foreign key exception regardless of entity type.
/// </summary>
public abstract class InvalidForeignKeyException : Exception
{
    protected InvalidForeignKeyException(string message)
        : base(message) { }
}

public class InvalidForeignKeyException<ReferencedEntityType> : InvalidForeignKeyException
{
    public InvalidForeignKeyException()
        : base($"Invalid foreign key reference for {typeof(ReferencedEntityType).Name}.") { }
}

public class InvalidOrderByException : ApplicationException
{
    public InvalidOrderByException(string property)
        : base($"Cannot order by '{property}'.") { }
}
