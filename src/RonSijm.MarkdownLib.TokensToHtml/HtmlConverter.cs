using RonSijm.MarkdownLib.DataContracts;
using RonSijm.MarkdownLib.DataContracts.Tokens;

namespace RonSijm.MarkdownLib.TokensToHtml;

public class HtmlConverter(HtmlConverterOptions options)
{
    private readonly HtmlConverterOptions _options = options ?? new HtmlConverterOptions();
    private InlineLexer _inline;
    private Stack<IToken> _tokens = new();

    private IToken _token;

    public virtual string Parse(TokensResult src)
    {
        if (src == null)
        {
            return null;
        }

        _inline = new InlineLexer(src.Links, _options);
        _tokens = new Stack<IToken>(src.Reverse());

        var @out = string.Empty;
        while (Next() != null)
        {
            @out += Tok();
        }

        return @out;
    }

    protected virtual IToken Next()
    {
        return _token = (_tokens.Count != 0) ? _tokens.Pop() : null;
    }

    protected virtual IToken Peek()
    {
        return _tokens.Peek();
    }

    protected virtual string ParseText(TextToken textToken)
    {
        var body = textToken.Text;

        while (Peek() is TextToken)
        {
            var next = (TextToken)Next();

            body += '\n' + next.Text;
        }

        return _inline.Output(body);
    }

    protected virtual string Tok()
    {
        switch (_token)
        {
            case SpaceToken:
                return string.Empty;
            case HrToken:
                return _options.Renderer.Hr();
            case HeaderToken headerToken:
                return _options.Renderer.Heading(_inline.Output(headerToken.Text), headerToken.Depth, headerToken.Text);
            case CodeToken codeToken:
                return _options.Renderer.Code(codeToken.Text, codeToken.Lang, false);
            case TableToken tableToken:
                {
                    string header = string.Empty, body = string.Empty;

                    // header
                    var cell = string.Empty;
                    for (var i = 0; i < tableToken.Header.Count; i++)
                    {
                        cell += _options.Renderer.TableCell(
                            _inline.Output(tableToken.Header[i]),
                            new TableCellFlags { Header = true, Align = i < tableToken.Align.Count ? tableToken.Align[i] : null }
                        );
                    }
                    header += _options.Renderer.TableRow(cell);

                    foreach (var row in tableToken.Cells)
                    {
                        cell = string.Empty;
                        for (var j = 0; j < row.Length; j++)
                        {
                            cell += _options.Renderer.TableCell(_inline.Output(row[j]), new TableCellFlags { Header = false, Align = j < tableToken.Align.Count ? tableToken.Align[j] : null });
                        }

                        body += _options.Renderer.TableRow(cell);
                    }
                    return _options.Renderer.Table(header, body);
                }
            case BlockquoteStartToken:
                {
                    var body = string.Empty;

                    while (Next() is not BlockquoteEndToken)
                    {
                        body += Tok();
                    }

                    return _options.Renderer.Blockquote(body);
                }
            case ListStartToken listStartToken:
                {
                    var body = string.Empty;
                    var ordered = listStartToken.Ordered;

                    while (Next() is not ListEndToken)
                    {
                        body += Tok();
                    }

                    return _options.Renderer.List(body, ordered);
                }
            case ListItemStartToken:
                {
                    var body = string.Empty;

                    while (Next() is not ListItemEndToken)
                    {
                        string result;

                        if (_token is TextToken textToken)
                        {
                            result = ParseText(textToken);
                        }
                        else
                        {
                            result = Tok();
                        }

                        body += result;
                    }

                    return _options.Renderer.ListItem(body);
                }
            case LooseItemStartToken:
                {
                    var body = string.Empty;

                    while (Next() is not ListItemEndToken)
                    {
                        body += Tok();
                    }

                    return _options.Renderer.ListItem(body);
                }
            case HtmlToken htmlToken:
                {
                    var html = !htmlToken.Pre && !_options.Pedantic
                        ? _inline.Output(htmlToken.Text)
                        : htmlToken.Text;

                    return _options.Renderer.Html(html);
                }
            case ParagraphToken paragraphToken:
                {
                    return _options.Renderer.Paragraph(_inline.Output(paragraphToken.Text));
                }
            case TextToken textToken:
                {
                    return _options.Renderer.Paragraph(ParseText(textToken));
                }
        }

        throw new Exception();
    }
}