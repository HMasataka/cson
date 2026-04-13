using Cson.Parsing;
using Xunit;

namespace Tests;

public class LexerTests
{
    [Fact]
    public void Tokenize_EmptyObject_ReturnsBraces()
    {
        var lexer = new Lexer("{}");

        var tokens = lexer.Tokenize();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenKind.LeftBrace, tokens[0].Kind);
        Assert.Equal(TokenKind.RightBrace, tokens[1].Kind);
    }

    [Fact]
    public void Tokenize_StringValue_ReturnsStringToken()
    {
        var lexer = new Lexer("\"hello\"");

        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal(TokenKind.String, tokens[0].Kind);
        Assert.Equal("hello", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_EscapeSequences_ParsesCorrectly()
    {
        var lexer = new Lexer("\"hello\\nworld\\t!\"");

        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal("hello\nworld\t!", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_UnicodeEscape_ParsesCorrectly()
    {
        var lexer = new Lexer("\"\\u0041\"");

        var tokens = lexer.Tokenize();

        Assert.Single(tokens);
        Assert.Equal("A", tokens[0].Value);
    }

    [Fact]
    public void Tokenize_NumberValues_ReturnsNumberTokens()
    {
        var lexer = new Lexer("[42, -3.14, 1e10]");

        var tokens = lexer.Tokenize();

        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenKind.Number, tokens[1].Kind);
        Assert.Equal("42", tokens[1].Value);
        Assert.Equal(TokenKind.Number, tokens[3].Kind);
        Assert.Equal("-3.14", tokens[3].Value);
        Assert.Equal(TokenKind.Number, tokens[5].Kind);
        Assert.Equal("1e10", tokens[5].Value);
    }

    [Fact]
    public void Tokenize_BoolAndNull_ReturnsCorrectTokens()
    {
        var lexer = new Lexer("[true, false, null]");

        var tokens = lexer.Tokenize();

        Assert.Equal(TokenKind.True, tokens[1].Kind);
        Assert.Equal(TokenKind.False, tokens[3].Kind);
        Assert.Equal(TokenKind.Null, tokens[5].Kind);
    }

    [Fact]
    public void Tokenize_InvalidCharacter_Throws()
    {
        var lexer = new Lexer("@");

        Assert.Throws<InvalidOperationException>(() => lexer.Tokenize());
    }

    [Fact]
    public void Tokenize_UnterminatedString_Throws()
    {
        var lexer = new Lexer("\"unterminated");

        Assert.Throws<InvalidOperationException>(() => lexer.Tokenize());
    }
}
