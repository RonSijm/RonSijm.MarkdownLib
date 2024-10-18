using FluentAssertions;

using RonSijm.MarkdownLib.Tokenizer;
using RonSijm.MarkdownLib.TokensToHtml;

namespace RonSijm.MarkdownLib.Tests;

public class HtmlTests
{
    private readonly Lexer _lexer = new(new LexerOptions());
    private readonly HtmlConverter _htmlConverter = new(new HtmlConverterOptions());

    [Fact]
    public void Parse_Null_Null()
    {
        var tokens = _lexer.Lex(null);
        var actual = _htmlConverter.Parse(tokens);


        actual.Should().Be(null);
    }

    [Fact]
    public void Parse_EmptyString_EmptyString()
    {
        var tokens = _lexer.Lex(string.Empty);
        var actual = _htmlConverter.Parse(tokens);

        //assert
        actual.Should().Be(null);
    }

    [Theory]
    [InlineData("# Hello World", "<h1 id='hello-world'>Hello World</h1>\n")]
    public void Parse_MarkdownText_HtmlText(string source, string expected)
    {
        var tokens = _lexer.Lex(source);
        var actual = _htmlConverter.Parse(tokens);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hot keys: <kbd>Ctrl+[</kbd> and <kbd>Ctrl+]</kbd>", "<p>Hot keys: <kbd>Ctrl+[</kbd> and <kbd>Ctrl+]</kbd></p>\n")]
    [InlineData("<div>Some text here</div>", "<div>Some text here</div>")]
    public void Parse_MarkdownWithHtmlTags_HtmlText(string source, string expected)
    {
        var tokens = _lexer.Lex(source);
        var actual = _htmlConverter.Parse(tokens);

        actual.Should().Be(expected);
    }

    [Fact]
    public void Parse_MarkdownTableWithMissingAlign_HtmlText()
    {
        //arrange
        var table = @"|                  | Header1                        | Header2              |
 ----------------- | ---------------------------- | ------------------
| Single backticks | `'Isn't this fun?'`            | 'Isn't this fun?' |
| Quotes           | `""Isn't this fun?""`            | ""Isn't this fun?"" |
| Dashes           | `-- is en-dash, --- is em-dash` | -- is en-dash, --- is em-dash |";


        var expected = "<table>\n<thead>\n<tr>\n<th></th>\n<th></th>\n<th>Header1</th>\n<th>Header2</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td></td>\n<td>Single backticks</td>\n<td><code>&#39;Isn&#39;t this fun?&#39;</code></td>\n<td>&#39;Isn&#39;t this fun?&#39;</td>\n<td></td>\n</tr>\n<tr>\n<td></td>\n<td>Quotes</td>\n<td><code>&quot;Isn&#39;t this fun?&quot;</code></td>\n<td>&quot;Isn&#39;t this fun?&quot;</td>\n<td></td>\n</tr>\n<tr>\n<td></td>\n<td>Dashes</td>\n<td><code>-- is en-dash, --- is em-dash</code></td>\n<td>-- is en-dash, --- is em-dash</td>\n<td></td>\n</tr>\n</tbody>\n</table>\n";

        var tokens = _lexer.Lex(table);
        var actual = _htmlConverter.Parse(tokens);

        actual.Should().Be(expected);

    }

    [Fact]
    public void Parse_ComplexMarkdownText_HtmlText()
    {
        // arrange
        var markdown = @"
Heading
=======
 
Sub-heading
-----------
  
### Another deeper heading
  
Paragraphs are separated
by a blank line.
 
Leave 2 spaces at the end of a line to do a  
line break
 
Text attributes *italic*, **bold**, 
`monospace`, ~~strikethrough~~ .
 
A [link](http://example.com).

Shopping list:
 
* apples
* oranges
* pears
 
Numbered list:
 
1. apples
2. oranges
3. pears
";

        var expected = "<h1 id='heading'>Heading</h1>\n<h2 id='sub-heading'>Sub-heading</h2>\n<h3 id='another-deeper-heading'>Another deeper heading</h3>\n<p>Paragraphs are separated\nby a blank line.</p>\n<p>Leave 2 spaces at the end of a line to do a<br>line break</p>\n<p>Text attributes <em>italic</em>, <strong>bold</strong>, \n<code>monospace</code>, <del>strikethrough</del> .</p>\n<p>A <a href='http://example.com' target='_blank' rel='nofollow'>link</a>.</p>\n<p>Shopping list:</p>\n<ul>\n<li>apples</li>\n<li>oranges</li>\n<li>pears</li>\n</ul>\n<p>Numbered list:</p>\n<ol>\n<li>apples</li>\n<li>oranges</li>\n<li>pears</li>\n</ol>\n";

        var tokens = _lexer.Lex(markdown);
        var actual = _htmlConverter.Parse(tokens);

        actual.Should().Be(expected);
    }
}