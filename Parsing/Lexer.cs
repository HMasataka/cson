namespace Cson.Parsing;

public sealed class Lexer
{
    private readonly string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            var ch = _input[_position];

            if (char.IsWhiteSpace(ch))
            {
                _position++;
                continue;
            }

            var token = ch switch
            {
                '{' => Advance(TokenKind.LeftBrace, "{"),
                '}' => Advance(TokenKind.RightBrace, "}"),
                '[' => Advance(TokenKind.LeftBracket, "["),
                ']' => Advance(TokenKind.RightBracket, "]"),
                ':' => Advance(TokenKind.Colon, ":"),
                ',' => Advance(TokenKind.Comma, ","),
                '"' => ReadString(),
                _ when ch == '-' || char.IsDigit(ch) => ReadNumber(),
                't' => ReadLiteral("true", TokenKind.True),
                'f' => ReadLiteral("false", TokenKind.False),
                'n' => ReadLiteral("null", TokenKind.Null),
                _ => throw new InvalidOperationException($"Unexpected character '{ch}' at position {_position}"),
            };

            tokens.Add(token);
        }

        return tokens;
    }

    private Token Advance(TokenKind kind, string value)
    {
        _position++;
        return new Token(kind, value);
    }

    private Token ReadString()
    {
        _position++; // skip opening quote
        var sb = new System.Text.StringBuilder();

        while (_position < _input.Length)
        {
            var ch = _input[_position];

            if (ch == '"')
            {
                _position++; // skip closing quote
                return new Token(TokenKind.String, sb.ToString());
            }

            if (ch == '\\')
            {
                _position++;
                if (_position >= _input.Length)
                    throw new InvalidOperationException("Unexpected end of input in string escape");

                sb.Append(ReadEscapeCharacter());
                continue;
            }

            sb.Append(ch);
            _position++;
        }

        throw new InvalidOperationException("Unterminated string");
    }

    private char ReadEscapeCharacter()
    {
        var escaped = _input[_position];
        _position++;

        return escaped switch
        {
            '"' => '"',
            '\\' => '\\',
            '/' => '/',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'u' => ReadUnicodeEscape(),
            _ => throw new InvalidOperationException($"Invalid escape sequence '\\{escaped}'"),
        };
    }

    private char ReadUnicodeEscape()
    {
        if (_position + 4 > _input.Length)
            throw new InvalidOperationException("Unexpected end of input in unicode escape");

        var hex = _input.Substring(_position, 4);
        _position += 4;
        return (char)Convert.ToInt32(hex, 16);
    }

    private Token ReadNumber()
    {
        var start = _position;

        if (_input[_position] == '-')
            _position++;

        while (_position < _input.Length && char.IsDigit(_input[_position]))
            _position++;

        if (_position < _input.Length && _input[_position] == '.')
        {
            _position++;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
                _position++;
        }

        if (_position < _input.Length && (_input[_position] == 'e' || _input[_position] == 'E'))
        {
            _position++;
            if (_position < _input.Length && (_input[_position] == '+' || _input[_position] == '-'))
                _position++;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
                _position++;
        }

        return new Token(TokenKind.Number, _input.Substring(start, _position - start));
    }

    private Token ReadLiteral(string expected, TokenKind kind)
    {
        if (_position + expected.Length > _input.Length ||
            _input.Substring(_position, expected.Length) != expected)
            throw new InvalidOperationException($"Expected '{expected}' at position {_position}");

        _position += expected.Length;
        return new Token(kind, expected);
    }
}
