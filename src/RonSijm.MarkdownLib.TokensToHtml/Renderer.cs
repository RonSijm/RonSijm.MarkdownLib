﻿using System.Text.RegularExpressions;

namespace RonSijm.MarkdownLib.TokensToHtml;

public class Renderer(HtmlConverterOptions options)
{
    public HtmlConverterOptions Options { get; set; } = options ?? new HtmlConverterOptions();

    public virtual string Code(string code, string lang, bool escaped)
    {
        var transformedCode = code;

        if (Options.Highlight != null)
        {
            var output = Options.Highlight(code, lang);
            if (output != null && output != code)
            {
                escaped = true;
                transformedCode = output;
            }
        }

        transformedCode = escaped ? transformedCode : StringHelper.Escape(transformedCode, true);
        var langClass = Options.LangPrefix + StringHelper.Escape(lang ?? string.Empty, true);

        return $"<pre{AttributesToString(Options.PreformattedTextAttributes)}><code class='{langClass}'>{transformedCode}\n</code></pre>\n";
    }

    public virtual string Blockquote(string quote)
    {
        return $"<blockquote>\n{quote}</blockquote>\n";
    }

    public virtual string Html(string html)
    {
        return html;
    }

    public virtual string Heading(string text, int level, string raw)
    {
        return $"<h{level} id='{Options.HeaderPrefix}{Regex.Replace(raw.ToLower(), @"[^\w]+", "-")}'>{text}</h{level}>\n";
    }

    public virtual string Hr()
    {
        return Options.XHtml ? "<hr/>\n" : "<hr>\n";
    }

    public virtual string List(string body, bool ordered)
    {
        var type = ordered ? "ol" : "ul";
        return $"<{type}>\n{body}</{type}>\n";
    }

    public virtual string ListItem(string text)
    {
        return $"<li>{text}</li>\n";
    }

    public virtual string Paragraph(string text)
    {
        return $"<p>{text}</p>\n";
    }

    public virtual string Table(string header, string body)
    {
        return $"<table{AttributesToString(Options.TableAttributes)}>\n<thead>\n{header}</thead>\n<tbody>\n{body}</tbody>\n</table>\n";
    }

    public virtual string TableRow(string content)
    {
        return $"<tr>\n{content}</tr>\n";
    }

    public virtual string TableCell(string content, TableCellFlags flags)
    {
        var type = flags.Header ? "th" : "td";
        var tag = !string.IsNullOrEmpty(flags.Align)
            ? $"<{type} style='text-align:{flags.Align}'>"
            : $"<{type}>";

        return tag + content + $"</{type}>\n";
    }

    public virtual string Strong(string text)
    {
        return $"<strong>{text}</strong>";
    }

    public virtual string Em(string text)
    {
        return $"<em>{text}</em>";
    }

    public virtual string Codespan(string text)
    {
        return $"<code>{text}</code>";
    }

    public virtual string Br()
    {
        return Options.XHtml ? "<br/>" : "<br>";
    }

    public virtual string Del(string text)
    {
        return $"<del>{text}</del>";
    }

    public virtual string Link(string href, string title, string text)
    {
        if (Options.Sanitize)
        {
            string prot;
                
            try
            {
                prot = Regex.Replace(StringHelper.DecodeUriComponent(StringHelper.Unescape(href)), @"[^\w:]", string.Empty).ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }

            if (prot.IndexOf("javascript:", StringComparison.Ordinal) == 0 || prot.IndexOf("vbscript:", StringComparison.Ordinal) == 0)
            {
                return string.Empty;
            }
        }

        var output = $"<a href='{href}'";

        if (!string.IsNullOrEmpty(title))
        {
            output += $" title='{title}'";
        }

        if (Options.ExternalLinks && (href.StartsWith("//", StringComparison.Ordinal) || href.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)))
        {
            output += " target='_blank' rel='nofollow'";
        }

        output += $">{text}</a>";
        return output;
    }

    public virtual string Image(string href, string title, string text)
    {
        var output = $"<img src='{href}' alt='{text}'{AttributesToString(Options.ImageAttributes)}";

        if (!string.IsNullOrEmpty(title))
        {
            output += $" title='{title}'";
        }

        output += Options.XHtml ? "/>" : ">";
        return output;
    }

    public virtual string Text(string text)
    {
        return text;
    }

    public static string AttributesToString(IDictionary<string, string> attributes)
    {
        if (attributes == null || attributes.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(string.Empty, attributes.Select(kv => $" {kv.Key}='{kv.Value}'"));
    }
}