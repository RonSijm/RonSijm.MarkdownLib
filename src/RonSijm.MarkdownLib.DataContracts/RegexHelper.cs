using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.DataContracts;

public static class RegexHelper
{
    public static string ReplaceRegex(this string input, string pattern, string replacement)
    {
        return Regex.Replace(input, pattern, replacement);
    }


    public static string[] SplitRegex(this string input, string pattern)
    {
        return Regex.Split(input, pattern);
    }

    public static IList<string> Exec(this Regex regex, string src)
    {
        var match = regex.Match(src);

        if (!match.Success)
        {
            return Array.Empty<string>();
        }

        return match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
    }

    public static string[] Match(this string src, Regex regex)
    {
        return regex.Matches(src).Select(x => x.Value).ToArray();
    }
}