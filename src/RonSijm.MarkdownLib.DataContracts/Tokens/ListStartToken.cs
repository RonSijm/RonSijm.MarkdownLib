namespace RonSijm.MarkdownLib.DataContracts.Tokens;

public struct ListStartToken : IToken
{
    public bool Ordered { get; set; }
}