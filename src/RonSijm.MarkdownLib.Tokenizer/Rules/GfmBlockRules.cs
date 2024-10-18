using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.Tokenizer.Rules;

public partial class GfmBlockRules : BlockRules
{
    public override Regex Fences => FencesRegex();
    public override Regex Paragraph => ParagraphRegex();
    public override Regex Heading => HeadingRegex();


    [GeneratedRegex(@"^ *(`{3,}|~{3,}) *(\S+)? *\n([\s\S]+?)\s*\1 *(?:\n+|$)")]
    private static partial Regex FencesRegex();
    [GeneratedRegex(@"^((?:[^\n]+\n?(?! *(`{3,}|~{3,}) *(\S+)? *\n([\s\S]+?)\s*\2 *(?:\n+|$)|( *)((?:[*+-]|\d+\.)) [\s\S]+?(?:\n+(?=\3?(?:[-*_] *){3,}(?:\n+|$))|\n+(?= *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))|\n{2,}(?! )(?!\1(?:[*+-]|\d+\.) )\n*|\s*$)|( *[-*_]){3,} *(?:\n+|$)| *(#{1,6}) *([^\n]+?) *#* *(?:\n+|$)|([^\n]+)\n *(=|-){2,} *(?:\n+|$)|( *>[^\n]+(\n(?! *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$))[^\n]+)*\n*)+|<(?!(?:a|em|strong|small|s|cite|q|dfn|abbr|data|time|code|var|samp|kbd|sub|sup|i|b|u|mark|ruby|rt|rp|bdi|bdo|span|br|wbr|ins|del|img)\b)\w+(?!:\/|[^\w\s@]*@)\b| *\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\n+|$)))+)\n*")]
    private static partial Regex ParagraphRegex();
    [GeneratedRegex(@"^ *(#{1,6}) *([^\n]+?) *#* *(?:\n+|$)")]
    private static partial Regex HeadingRegex();
}