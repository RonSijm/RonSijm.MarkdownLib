namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct HeaderToken : IToken
{
    public int Depth { get; set; }
    public string Text { get; set; }
}