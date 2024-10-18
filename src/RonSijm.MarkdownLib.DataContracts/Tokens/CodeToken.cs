namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct CodeToken : IToken
{
    public string Text { get; set; }
    public string Lang { get; set; }
}