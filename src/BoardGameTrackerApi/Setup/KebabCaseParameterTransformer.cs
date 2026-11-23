using System.Text.RegularExpressions;

namespace BoardGameTracker.Api.Setup;

/// <summary>
/// Turns route tokens such as [controller] into kebab-case, e.g. BoardGames to board-games
/// </summary>
internal sealed partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex WordBoundary { get; }

    public string? TransformOutbound(object? value)
    {
        return value is null
            ? null
            : WordBoundary.Replace(value.ToString()!, "$1-$2").ToLowerInvariant();
    }
}
