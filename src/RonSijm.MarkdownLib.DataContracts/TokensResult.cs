namespace RonSijm.MarkdownLib.DataContracts;

public class TokensResult
{
    public IList<IToken> Tokens { get; set; }
    public IDictionary<string, LinkObj> Links { get; set; }

    public IEnumerable<IToken> Reverse()
    {
        return Tokens.Reverse();
    }

    public TokensResult()
    {
        Tokens = new List<IToken>();
        Links = new Dictionary<string, LinkObj>();
    }


    public void Add(IToken token)
    {
        Tokens.Add(token);
    }
}