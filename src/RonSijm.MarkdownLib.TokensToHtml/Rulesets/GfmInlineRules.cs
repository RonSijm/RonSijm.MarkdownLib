using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml.RuleSets;

public partial class GfmInlineRules : InlineRules
{
    public override Regex Escape => EscapeFieldRegex();
    public override Regex Url => UrlRegex();
    public override Regex Del => DelRegex();
    public override Regex Text => TextRegex();

    [GeneratedRegex(@"^\\([\\`*{}\[\]()#+\-.!_>~|])")]
    private static partial Regex EscapeFieldRegex();
    [GeneratedRegex(@"^(https?:\/\/[^\s<]+[^<.,:;""')\]\s])")]
    private static partial Regex UrlRegex();
    [GeneratedRegex(@"^~~(?=\S)([\s\S]*?\S)~~")]
    private static partial Regex DelRegex();
    [GeneratedRegex(@"^[\s\S]+?(?=[\\<!\[_*`~]|https?:\/\/| {2,}\n|$)")]
    private static partial Regex TextRegex();
}