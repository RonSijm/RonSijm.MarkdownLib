using System.Text.RegularExpressions;

using RonSijm.MarkdownLib.DataContracts;
using RonSijm.MarkdownLib.TokensToHtml.RuleSets;

namespace RonSijm.MarkdownLib.TokensToHtml;

public class InlineLexer
{
    private readonly Random _random = new();

    private readonly HtmlConverterOptions _options;
    private readonly InlineRules _rules;
    private readonly IDictionary<string, LinkObj> _links;
    private bool _inLink;


    public InlineLexer(IDictionary<string, LinkObj> links, HtmlConverterOptions options)
    {
        _options = options ?? new HtmlConverterOptions();

        _links = links;
        _rules = new NormalInlineRules();

        if (_links == null)
        {
            throw new Exception("Tokens array requires a `links` property.");
        }

        if (_options.Gfm)
        {
            if (_options.Breaks)
            {
                _rules = new BreaksInlineRules();
            }
            else
            {
                _rules = new GfmInlineRules();
            }
        }
        else if (_options.Pedantic)
        {
            _rules = new PedanticInlineRules();
        }
    }

    public virtual string Output(string src)
    {
        var result = string.Empty;

        while (!string.IsNullOrEmpty(src))
        {
            // escape
            var cap = _rules.Escape.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += cap[1];
                continue;
            }

            // autolink
            cap = _rules.AutoLink.Exec(src);
            string text;

            string href;

            if (cap.Any())
            {
                src = src[cap[0].Length..];
                if (cap[2] == "@")
                {
                    text = cap[1][6] == ':'
                        ? Mangle(cap[1][7..])
                        : Mangle(cap[1]);
                    href = Mangle("mailto:") + text;
                }
                else
                {
                    text = StringHelper.Escape(cap[1]);
                    href = text;
                }
                result += _options.Renderer.Link(href, null, text);
                continue;
            }

            // url (gfm)
            if (!_inLink && (cap = _rules.Url.Exec(src)).Any())
            {
                src = src[cap[0].Length..];
                text = StringHelper.Escape(cap[1]);
                href = text;
                result += _options.Renderer.Link(href, null, text);
                continue;
            }

            // tag
            cap = _rules.Tag.Exec(src);
            if (cap.Any())
            {
                if (!_inLink && Regex.IsMatch(cap[0], "^<a ", RegexOptions.IgnoreCase))
                {
                    _inLink = true;
                }
                else if (_inLink && Regex.IsMatch(cap[0], @"^<\/a>", RegexOptions.IgnoreCase))
                {
                    _inLink = false;
                }
                src = src[cap[0].Length..];
                result += _options.Sanitize
                    ? (_options.Sanitizer != null)
                        ? _options.Sanitizer(cap[0])
                        : StringHelper.Escape(cap[0])
                    : cap[0];
                continue;
            }

            // link
            cap = _rules.Link.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                _inLink = true;
                result += OutputLink(cap, new LinkObj
                {
                    Href = cap[2],
                    Title = cap[3]
                });
                _inLink = false;
                continue;
            }

            // reflink, nolink
            if ((cap = _rules.RefLink.Exec(src)).Any() || (cap = _rules.NoLink.Exec(src)).Any())
            {
                src = src[cap[0].Length..];
                var linkStr = (StringHelper.NotEmpty(cap, 2, 1)).ReplaceRegex(@"\s+", " ");

                _links.TryGetValue(linkStr.ToLower(), out var link);
                    
                if (link == null || string.IsNullOrEmpty(link.Href))
                {
                    result += cap[0][0];
                    src = cap[0][1..] + src;
                    continue;
                }
                _inLink = true;
                result += OutputLink(cap, link);
                _inLink = false;
                continue;
            }

            // strong
            if ((cap = _rules.Strong.Exec(src)).Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Strong(Output(StringHelper.NotEmpty(cap, 2, 1)));
                continue;
            }

            // em
            cap = _rules.Em.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Em(Output(StringHelper.NotEmpty(cap, 2, 1)));
                continue;
            }

            // code
            cap = _rules.Code.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Codespan(StringHelper.Escape(cap[2], true));
                continue;
            }

            // br
            cap = _rules.Br.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Br();
                continue;
            }

            // del (gfm)
            cap = _rules.Del.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Del(Output(cap[1]));
                continue;
            }

            // text
            cap = _rules.Text.Exec(src);
            if (cap.Any())
            {
                src = src[cap[0].Length..];
                result += _options.Renderer.Text(StringHelper.Escape(Smartypants(cap[0])));
                continue;
            }

            if (!string.IsNullOrEmpty(src))
            {
                throw new Exception("Infinite loop on byte: " + (int)src[0]);
            }
        }

        return result;
    }

    protected virtual string OutputLink(IList<string> cap, LinkObj link)
    {
        string href = StringHelper.Escape(link.Href),
            title = !string.IsNullOrEmpty(link.Title) ? StringHelper.Escape(link.Title) : null;

        return cap[0][0] != '!'
            ? _options.Renderer.Link(href, title, Output(cap[1]))
            : _options.Renderer.Image(href, title, StringHelper.Escape(cap[1]));
    }

    protected virtual string Mangle(string text)
    {
        if (!_options.Mangle)
        {
            return text;
        }

        var @out = string.Empty;

        foreach (var character in text)
        {
            var ch = character.ToString();
            if (_random.NextDouble() > 0.5)
            {
                ch = 'x' + Convert.ToString(ch[0], 16);
            }
            @out += "&#" + ch + ";";
        }

        return @out;
    }

    protected virtual string Smartypants(string text)
    {
        if (!_options.Smartypants)
        {
            return text;
        }

        return text
            // em-dashes
            .Replace("---", "\u2014")
            // en-dashes
            .Replace("--", "\u2013")
            // opening singles
            .ReplaceRegex(@"(^|[-\u2014/(\[{""\s])'", "$1\u2018")
            // closing singles & apostrophes
            .Replace("'", "\u2019")
            // opening doubles
            .ReplaceRegex(@"(^|[-\u2014/(\[{\u2018\s])""", "$1\u201c")
            // closing doubles
            .Replace("\"", "\u201d")
            // ellipses
            .Replace("...", "\u2026");
    }
}