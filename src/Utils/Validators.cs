using System.Text.RegularExpressions;

namespace BoardGameTracker.Utils;

public static partial class Validators
{
    [GeneratedRegex(
        @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"
    )]
    private static partial Regex UrlRegex { get; }

    public static bool IsUrlValid(string url)
    {
        var regex = UrlRegex;
        return regex.IsMatch(url);
    }
}
