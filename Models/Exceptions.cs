namespace BoardGameTracker.Models;

public class VersionMismatchException : Exception
{
    public VersionMismatchException(string? message = "Version mismatch.")
        : base(message) { }
}

public class EntityNotFoundException<EntityType> : Exception
    where EntityType : class
{
    public EntityNotFoundException()
        : base($"{typeof(EntityType).Name} not found.") { }
}

public class InvalidForeignKeyException<ReferencedEntityType> : Exception
{
    public InvalidForeignKeyException()
        : base($"Invalid foreign key reference for {nameof(ReferencedEntityType)}.") { }
}
