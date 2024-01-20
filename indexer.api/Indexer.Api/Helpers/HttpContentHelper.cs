using System.Text.RegularExpressions;


namespace Indexer.Api.Helpers;

public static class HttpContentHelper
{
    public static string GetTitleFromContent(string content)
    {
        string pattern = @"<title\b[^>]*>(.*?)</title>";

        Match match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
        if (match.Success)
            return match.Groups[1].Value.Trim();

        return string.Empty;
    }
}