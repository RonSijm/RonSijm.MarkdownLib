using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.Tokenizer.Rules;

public partial class BlockRules
{
    public virtual Regex Fences => FencesRegex(); // noop
    public virtual Regex TableNoLeadingPipe => NpRegex(); // noop
    public virtual Regex Table => TableRegex(); // noop

    public Regex Newline => NewlineRegex();
    public Regex Code => CodeRegex();

    public Regex Hr => HRRegex();
    public virtual Regex Heading => HeadingRegex();

    public Regex LHeading => LHeadingRegex();
    public Regex Blockquote => BlockquoteRegex();
    public Regex List => ListRegex();
    public Regex Html => HtmlRegex();
    public Regex Def => DefRegex();

    public virtual Regex Paragraph => ParagraphRegex();
    public Regex Text => TextRegex();
    public Regex Bullet => BulletRegex();
    public Regex Item => ItemRegex();



    [GeneratedRegex(@"^\n+")]
    private static partial Regex NewlineRegex();
    [GeneratedRegex(@"^( {4}[^\n]+\n*)+")]
    private static partial Regex CodeRegex();
    [GeneratedRegex("")]
    private static partial Regex FencesRegex();
    [GeneratedRegex(@"^( *[-*_]){3,} *(?:\n+|$)")]
    private static partial Regex HRRegex();
    [GeneratedRegex(@"^ *(#{1,6}) *([^\n]+?) *#* *(?:\n+|$)")]
    private static partial Regex HeadingRegex();
    [GeneratedRegex("")]
    private static partial Regex NpRegex();
    [GeneratedRegex(@"^([^\n]+)\n *(=|-){2,} *(?:\n+|$)")]
    private static partial Regex LHeadingRegex();
    [GeneratedRegex(@"^( *>[^\n]+(\n(?! *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))[^\n]+)*\n*)+")]
    private static partial Regex BlockquoteRegex();
    [GeneratedRegex(@"^( *)((?:[*+-]|\d+\.)) [\s\S]+?(?:\n+(?=\1?(?:[-*_] *){3,}(?:\n+|$))|\n+(?= *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))|\n{2,}(?! )(?!\1(?:[*+-]|\d+\.) )\n*|\s*$)")]
    private static partial Regex ListRegex();
    [GeneratedRegex(@"^ *(?:<!--[\s\S]*?-->|<((?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\b)\w+(?!:\/|[^\w\s@]*@)\b)[\s\S]+?<\/\1>|<(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\b)\w+(?!:\/|[^\w\s@]*@)\b(?:""[^""]*""|'[^']*'|[^'"">])*?>) *(?:\n{2,}|\s*$)")]
    private static partial Regex HtmlRegex();
    [GeneratedRegex(@"^ *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$)")]
    private static partial Regex DefRegex();
    [GeneratedRegex("")]
    private static partial Regex TableRegex();
    [GeneratedRegex(@"^((?:[^\n]+\n?(?!( *[-*_]){3,} *(?:\n+|$)| *(#{1,6}) *([^\n]+?) *#* *(?:\n+|$)|([^\n]+)\n *(=|-){2,} *(?:\n+|$)|( *>[^\n]+(\n(?! *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))[^\n]+)*\n*)+|<(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\b)\w+(?!:\/|[^\w\s@]*@)\b| *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$)))+)\n*")]
    private static partial Regex ParagraphRegex();
    [GeneratedRegex(@"^[^\n]+")]
    private static partial Regex TextRegex();
    [GeneratedRegex(@"(?:[*+-]|\d+\.)")]
    private static partial Regex BulletRegex();
    [GeneratedRegex(@"^( *)((?:[*+-]|\d+\.)) [^\n]*(?:\n(?!\1(?:[*+-]|\d+\.) )[^\n]*)*", RegexOptions.Multiline)]
    private static partial Regex ItemRegex();
}