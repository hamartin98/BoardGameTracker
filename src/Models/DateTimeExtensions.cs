namespace BoardGameTracker.Models;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts to UTC; a value without a timezone is treated as UTC
    /// </summary>
    // Npgsql only accepts UTC DateTimes for timestamp with time zone columns
    public static DateTime? ToUtc(this DateTime? value)
    {
        return value?.Kind switch
        {
            null => null,
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc),
        };
    }
}
