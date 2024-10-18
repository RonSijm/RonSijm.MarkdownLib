using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml.RuleSets;

public partial class BreaksInlineRules : GfmInlineRules
{
    public override Regex Br => BrFieldRegex();
    public override Regex Text => TextFieldRegex();

    [GeneratedRegex(@"^ *\n(?!\s*$)")]
    private static partial Regex BrFieldRegex();
    [GeneratedRegex(@"^[\s\S]+?(?=[\\<!\[_*`~]|https?:\/\/| *\n|$)")]
    private static partial Regex TextFieldRegex();
}