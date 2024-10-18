namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct HtmlToken : IToken
{
    public bool Pre { get; set; }
    public string Text { get; set; }
}