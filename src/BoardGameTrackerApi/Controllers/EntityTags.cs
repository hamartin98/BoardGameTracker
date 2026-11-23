using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using Microsoft.Net.Http.Headers;

namespace BoardGameTrackerApi.Controllers;

/// <summary>
/// ETag and If-Match handling; an entity's ETag is its quoted Version
/// </summary>
public static class EntityTags
{
    public static void SetETag(this HttpResponse response, AbstractEntity entity)
    {
        response.Headers.ETag = new EntityTagHeaderValue($"\"{entity.Version}\"").ToString();
    }

    /// <summary>
    /// Version from a single strong entity tag; throws PreconditionRequiredException when missing
    /// </summary>
    public static int ParseIfMatch(string? ifMatch)
    {
        if (string.IsNullOrWhiteSpace(ifMatch))
        {
            throw new PreconditionRequiredException();
        }
        if (
            !EntityTagHeaderValue.TryParse(ifMatch.Trim(), out EntityTagHeaderValue? tag)
            || tag.IsWeak
            || tag.Tag.Length < 3
            || !int.TryParse(tag.Tag.AsSpan(1, tag.Tag.Length - 2), out int version)
        )
        {
            throw new InvalidIfMatchException();
        }
        return version;
    }
}
