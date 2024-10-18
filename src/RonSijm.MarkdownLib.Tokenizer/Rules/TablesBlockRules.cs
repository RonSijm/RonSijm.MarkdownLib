using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.Tokenizer.Rules;

public partial class TablesBlockRules : GfmBlockRules
{
    public override Regex TableNoLeadingPipe => NpTableRegex();
    public override Regex Table => TableRegex();


    [GeneratedRegex(@"^ *(\S.*\|.*)\n *([-:]+ *\|[-| :]*)\n((?:.*\|.*(?:\n|$))*)\n*")]
    private static partial Regex NpTableRegex();
    [GeneratedRegex(@"^ *\|(.+)\n *\|( *[-:]+[-| :]*)\n((?: *\|.*(?:\n|$))*)\n*")]
    private static partial Regex TableRegex();
}