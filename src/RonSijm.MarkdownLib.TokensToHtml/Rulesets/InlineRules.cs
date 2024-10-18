using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml.RuleSets;

public partial class InlineRules
{
    public virtual Regex Escape => EscapeRegex();
    public Regex AutoLink => AutoLinkRegex();
    public virtual Regex Url => UrlRegex(); // noop
    public Regex Tag => TagRegex();
    public Regex Link => LinkRegex();
    public Regex RefLink => RefLinkRegex();
    public Regex NoLink => NoLinkRegex();
    public virtual Regex Strong => StrongRegex();
    public virtual Regex Em => EmRegex();
    public Regex Code => CodeRegex();
    public virtual Regex Br => BrRegex();
    public virtual Regex Del => DelRegex(); // noop
    public virtual Regex Text => TextRegex();

    [GeneratedRegex(@"^\\([\\`*{}\[\]()#+\-.!_>])")]
    private static partial Regex EscapeRegex();
    [GeneratedRegex(@"^<([^ >]+(@|:\/)[^ >]+)>")]
    private static partial Regex AutoLinkRegex();
    [GeneratedRegex("")]
    private static partial Regex UrlRegex();
    [GeneratedRegex(@"^<!--[\s\S]*?-->|^<\/?\w+(?:""[^""]*""|'[^']*'|[^'"">])*?>")]
    private static partial Regex TagRegex();
    [GeneratedRegex(@"^!?\[((?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\(\s*<?([\s\S]*?)>?(?:\s+['""]([\s\S]*?)['""])?\s*\)")]
    private static partial Regex LinkRegex();
    [GeneratedRegex(@"^!?\[((?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\s*\[([^\]]*)\]")]
    private static partial Regex RefLinkRegex();
    [GeneratedRegex(@"^!?\[((?:\[[^\]]*\]|[^\[\]])*)\]")]
    private static partial Regex NoLinkRegex();
    [GeneratedRegex(@"^__([\s\S]+?)__(?!_)|^\*\*([\s\S]+?)\*\*(?!\*)")]
    private static partial Regex StrongRegex();
    [GeneratedRegex(@"^\b_((?:__|[\s\S])+?)_\b|^\*((?:\*\*|[\s\S])+?)\*(?!\*)")]
    private static partial Regex EmRegex();
    [GeneratedRegex(@"^(`+)\s*([\s\S]*?[^`])\s*\1(?!`)")]
    private static partial Regex CodeRegex();
    [GeneratedRegex(@"^ {2,}\n(?!\s*$)")]
    private static partial Regex BrRegex();
    [GeneratedRegex("")]
    private static partial Regex DelRegex();
    [GeneratedRegex(@"^[\s\S]+?(?=[\\<!\[_*`]| {2,}\n|$)")]
    private static partial Regex TextRegex();
}