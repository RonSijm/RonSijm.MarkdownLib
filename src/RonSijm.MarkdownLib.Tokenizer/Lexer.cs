using System.Text.RegularExpressions;

using RonSijm.MarkdownLib.DataContracts;
using RonSijm.MarkdownLib.DataContracts.Tokens;
using RonSijm.MarkdownLib.Tokenizer.Rules;

namespace RonSijm.MarkdownLib.Tokenizer;

public class Lexer
{
    private readonly LexerOptions _options;
    private readonly BlockRules _rules;


    public Lexer(LexerOptions options)
    {
        _options = options ?? new LexerOptions();

        if (_options.Gfm)
        {
            if (_options.Tables)
            {
                _rules = new TablesBlockRules();
            }
            else
            {
                _rules = new GfmBlockRules();
            }
        }
        else
        {
            _rules = new BlockRules();
        }
    }

    public virtual TokensResult Lex(string src)
    {
        if (string.IsNullOrWhiteSpace(src))
        {
            return null;
        }

        src = src
            .ReplaceRegex(@"\r\n|\r", "\n")
            .Replace("\t", "    ")
            .Replace("\u00a0", " ")
            .Replace("\u2424", "\n");

        return Token(src, true);
    }

    protected virtual TokensResult Token(string srcOrig, bool top, TokensResult result = null)
    {
        var src = Regex.Replace(srcOrig, @"^ +$", "", RegexOptions.Multiline);
        var tokens = result ?? new TokensResult();

        while (!string.IsNullOrEmpty(src))
        {
            src = NewLine(src, tokens);

            if (Code(tokens, ref src))
            {
                continue;
            }

            if (Fences(tokens, ref src))
            {
                continue;
            }

            if (Heading(tokens, ref src))
            {
                continue;
            }

            if (TableNoLeadingPipe(top, tokens, ref src))
            {
                continue;
            }

            if (LHeading(tokens, ref src))
            {
                continue;
            }

            if (Hr(tokens, ref src))
            {
                continue;
            }

            if (Blockquote(top, tokens, ref src))
            {
                continue;
            }

            // list
            if (List(tokens, ref src))
            {
                continue;
            }

            if (Html(tokens, ref src))
            {
                continue;
            }

            if (Def(top, tokens, ref src))
            {
                continue;
            }

            if (Table(top, tokens, ref src))
            {
                continue;
            }

            if (TopLevelParagraph(top, tokens, ref src))
            {
                continue;
            }

            if (Text(tokens, ref src))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(src))
            {
                throw new Exception("Infinite loop on byte: " + (int)src[0]);
            }
        }

        return tokens;
    }

    private bool Text(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Text.Exec(src)).Any())
        {
            return false;
        }

        // Top-level should never reach here.
        src = src[cap[0].Length..];
        tokens.Add(new TextToken
        {
            Text = cap[0]
        });

        return true;
    }

    private bool TopLevelParagraph(bool top, TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!top || !(cap = _rules.Paragraph.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Add(new ParagraphToken
        {
            Text = cap[1][cap[1].Length - 1] == '\n'
                ? cap[1][..^1]
                : cap[1]
        });

        return true;
    }

    private bool Table(bool top, TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!top || !(cap = _rules.Table.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];

        var item = new TableToken
        {
            Header = cap[1].ReplaceRegex(@"^ *| *\| *$", "").SplitRegex(@" *\| *"),
            Align = cap[2].ReplaceRegex(@"^ *|\| *$", "").SplitRegex(@" *\| *"),
            Cells = cap[3].ReplaceRegex(@"(?: *\| *)?\n$", "").Split('\n').Select(x => new[] { x }).ToArray()
        };

        for (var i = 0; i < item.Align.Count; i++)
        {
            if (Regex.IsMatch(item.Align[i], @"^ *-+: *$"))
            {
                item.Align[i] = "right";
            }
            else if (Regex.IsMatch(item.Align[i], @"^ *:-+: *$"))
            {
                item.Align[i] = "center";
            }
            else if (Regex.IsMatch(item.Align[i], @"^ *:-+ *$"))
            {
                item.Align[i] = "left";
            }
            else
            {
                item.Align[i] = null;
            }
        }

        for (var i = 0; i < item.Cells.Length; i++)
        {
            item.Cells[i] = item.Cells[i][0]
                .ReplaceRegex(@"^ *\| *| *\| *$", "")
                .SplitRegex(@" *\| *");
        }

        tokens.Add(item);

        return true;

    }

    private bool Def(bool top, TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if ((!top) || !(cap = _rules.Def.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Links[cap[1].ToLower()] = new LinkObj
        {
            Href = cap[2],
            Title = cap[3]
        };
        return true;

    }

    private bool Html(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Html.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];

        var pre = (_options.Sanitizer == null) && (cap[1] == "pre" || cap[1] == "script" || cap[1] == "style");
        var text = cap[0];

        if (_options.Sanitize)
        {
            tokens.Add(new ParagraphToken
            {
                Pre = pre,
                Text = text,
            });
        }
        else
        {
            tokens.Add(new HtmlToken
            {
                Pre = pre,
                Text = text,
            });
        }


        return true;

    }

    private bool List(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.List.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        var bull = cap[2];

        tokens.Add(new ListStartToken
        {
            Ordered = bull.Length > 1
        });

        // Get each top-level item.
        cap = cap[0].Match(_rules.Item);

        var next = false;
        var l = cap.Count;
        var i = 0;

        for (; i < l; i++)
        {
            var item = cap[i];

            // Remove the list item's bullet
            // so it is seen as the next token.
            var space = item.Length;
            item = item.ReplaceRegex(@"^ *([*+-]|\d+\.) +", "");

            // Outdent whatever the
            // list item contains. Hacky.
            if (item.IndexOf("\n ", StringComparison.Ordinal) > -1)
            {
                space -= item.Length;
                item = !_options.Pedantic
                    ? Regex.Replace(item, "^ {1," + space + "}", "", RegexOptions.Multiline)
                    : Regex.Replace(item, @"/^ {1,4}", "", RegexOptions.Multiline);
            }

            // Determine whether the next list item belongs here.
            // Backpedal if it does not belong in this list.
            if (_options.SmartLists && i != l - 1)
            {
                var b = _rules.Bullet.Exec(cap[i + 1])[0]; // !!!!!!!!!!!
                if (bull != b && !(bull.Length > 1 && b.Length > 1))
                {
                    src = string.Join("\n", cap.Skip(i + 1)) + src;
                    i = l - 1;
                }
            }

            // Determine whether item is loose or not.
            // Use: /(^|\n)(?! )[^\n]+\n\n(?!\s*$)/
            // for discount behavior.
            var loose = next || Regex.IsMatch(item, @"\n\n(?!\s*$)");
            if (i != l - 1)
            {
                next = item[^1] == '\n';
                if (!loose)
                {
                    loose = next;
                }
            }

            if (loose)
            {
                tokens.Add(new LooseItemStartToken());
            }
            else
            {
                tokens.Add(new ListItemStartToken());
            }

            // Recurse.
            Token(item, false, tokens);

            tokens.Add(new ListItemEndToken());
        }

        tokens.Add(new ListEndToken());

        return true;

    }

    private bool Blockquote(bool top, TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Blockquote.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];

        tokens.Add(new BlockquoteStartToken());

        var capStr = Regex.Replace(cap[0], @"^ *> ?", "", RegexOptions.Multiline);

        // Pass `top` to keep the current
        // "toplevel" state. This is exactly
        // how markdown.pl works.
        Token(capStr, top, tokens); //, true);

        tokens.Add(new BlockquoteEndToken());

        return true;

    }

    private bool Hr(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Hr.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Add(new HrToken());
        return true;

    }

    private bool LHeading(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.LHeading.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Add(new HeaderToken
        {
            Depth = cap[2] == "=" ? 1 : 2,
            Text = cap[1]
        });
        return true;

    }

    private bool TableNoLeadingPipe(bool top, TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!top || !(cap = _rules.TableNoLeadingPipe.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];

        var item = new TableToken
        {
            Header = cap[1].ReplaceRegex(@"^ *| *\| *$", "").SplitRegex(@" *\| *"),
            Align = cap[2].ReplaceRegex(@"^ *|\| *$", "").SplitRegex(@" *\| *"),
            Cells = cap[3].ReplaceRegex(@"\n$", "").Split('\n').Select(x => new[] { x }).ToArray()
        };

        for (var i = 0; i < item.Align.Count; i++)
        {
            if (Regex.IsMatch(item.Align[i], @"^ *-+: *$"))
            {
                item.Align[i] = "right";
            }
            else if (Regex.IsMatch(item.Align[i], @"^ *:-+: *$"))
            {
                item.Align[i] = "center";
            }
            else if (Regex.IsMatch(item.Align[i], @"^ *:-+ *$"))
            {
                item.Align[i] = "left";
            }
            else
            {
                item.Align[i] = null;
            }
        }

        for (var i = 0; i < item.Cells.Length; i++)
        {
            item.Cells[i] = item.Cells[i][0].SplitRegex(@" *\| *");
        }

        tokens.Add(item);

        return true;

    }

    private bool Heading(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Heading.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Add(new HeaderToken
        {
            Depth = cap[1].Length,
            Text = cap[2]
        });
        return true;

    }

    private bool Fences(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Fences.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        tokens.Add(new CodeToken
        {
            Lang = cap[2],
            Text = cap[3]
        });
        return true;

    }

    private bool Code(TokensResult tokens, ref string src)
    {
        IList<string> cap;

        if (!(cap = _rules.Code.Exec(src)).Any())
        {
            return false;
        }

        src = src[cap[0].Length..];
        var capStr = Regex.Replace(cap[0], @"^ {4}", "", RegexOptions.Multiline);
        tokens.Add(new CodeToken
        {
            Text = !_options.Pedantic
                ? Regex.Replace(capStr, @"\n+$", "")
                : capStr
        });
        return true;

    }

    private string NewLine(string src, TokensResult tokens)
    {
        IList<string> cap;

        if (!(cap = _rules.Newline.Exec(src)).Any())
        {
            return src;
        }

        src = src[cap[0].Length..];
        if (cap[0].Length > 1)
        {
            tokens.Add(new SpaceToken());
        }

        return src;
    }
}