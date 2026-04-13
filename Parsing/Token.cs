namespace Cson.Parsing;

public enum TokenKind
{
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Colon,
    Comma,
    String,
    Number,
    True,
    False,
    Null,
}

public struct Token
{
    public TokenKind Kind { get; }
    public string Value { get; }

    public Token(TokenKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }
}
