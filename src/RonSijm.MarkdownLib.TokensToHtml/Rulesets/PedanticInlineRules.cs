using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml.RuleSets;

public partial class PedanticInlineRules : InlineRules
{
    public override Regex Strong => StrongRegex();
    public override Regex Em => EmRegex();

    [GeneratedRegex(@"^__(?=\S)([\s\S]*?\S)__(?!_)|^\*\*(?=\S)([\s\S]*?\S)\*\*(?!\*)")]
    private static partial Regex StrongRegex();
    [GeneratedRegex(@"^_(?=\S)([\s\S]*?\S)_(?!_)|^\*(?=\S)([\s\S]*?\S)\*(?!\*)")]
    private static partial Regex EmRegex();
}