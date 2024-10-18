namespace RonSijm.MarkdownLib.Tokenizer;

public class LexerOptions
{
    public Func<string, string> Sanitizer { get; set; } = null;

    public bool Sanitize { get; set; } = false;

    public bool Pedantic { get; set; } = false;

    public bool Gfm { get; set; } = true;

    public bool Tables { get; set; } = true;

    public bool SmartLists { get; set; } = false;
}