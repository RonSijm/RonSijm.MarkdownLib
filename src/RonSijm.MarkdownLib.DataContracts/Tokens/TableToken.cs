namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct TableToken : IToken
{
    public IList<string> Header { get; set; }
    public IList<string> Align { get; set; }
    public string[][] Cells { get; set; }
}