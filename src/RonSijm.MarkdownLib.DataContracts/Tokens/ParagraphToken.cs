namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct ParagraphToken : IToken
{
    public bool Pre { get; set; }
    public string Text { get; set; }
}