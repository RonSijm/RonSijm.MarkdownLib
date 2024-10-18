using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml;

public static class StringHelper
{
    public static string DecodeUriComponent(string str)
    {
        return Uri.UnescapeDataString(str);
    }

    public static string Escape(string html, bool encode = false)
    {
        return Regex.Replace(html, !encode ? @"&(?!#?\w+;)" : @"&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    public static string Unescape(string html)
    {
        return Regex.Replace(html, @"&([#\w]+);", match =>
        {
            var n = match.Groups[1].Value;

            n = n.ToLower();
            if (n == "colon")
            {
                return ":";
            }

            if (n[0] == '#')
            {
                return n[1] == 'x'
                    ? ((char)Convert.ToInt32(n[2..], 16)).ToString()
                    : ((char)Convert.ToInt32(n[1..])).ToString();
            }
            return string.Empty;
        });
    }



    public static string NotEmpty(IList<string> source, int index1, int index2)
    {
        return (source.Count > index1 && !string.IsNullOrEmpty(source[index1])) ? source[index1] : source[index2];
    }
}