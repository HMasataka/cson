namespace Cson.Parsing;

public sealed class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public JsonNode Parse()
    {
        var node = ParseValue();

        if (_position < _tokens.Count)
            throw new InvalidOperationException("Unexpected tokens after JSON value");

        return node;
    }

    private JsonNode ParseValue()
    {
        if (_position >= _tokens.Count)
            throw new InvalidOperationException("Unexpected end of tokens");

        var token = _tokens[_position];

        return token.Kind switch
        {
            TokenKind.LeftBrace => ParseObject(),
            TokenKind.LeftBracket => ParseArray(),
            TokenKind.String => ParseString(),
            TokenKind.Number => ParseNumber(),
            TokenKind.True => ParseBool(),
            TokenKind.False => ParseBool(),
            TokenKind.Null => ParseNull(),
            _ => throw new InvalidOperationException($"Unexpected token '{token.Kind}' at position {_position}"),
        };
    }

    private JsonObject ParseObject()
    {
        Expect(TokenKind.LeftBrace);
        var obj = new JsonObject();

        if (_position < _tokens.Count && _tokens[_position].Kind == TokenKind.RightBrace)
        {
            _position++;
            return obj;
        }

        while (true)
        {
            if (_position >= _tokens.Count || _tokens[_position].Kind != TokenKind.String)
                throw new InvalidOperationException("Expected string key in object");

            var key = _tokens[_position].Value;
            _position++;

            Expect(TokenKind.Colon);

            var value = ParseValue();
            obj.Properties[key] = value;

            if (_position >= _tokens.Count)
                throw new InvalidOperationException("Unexpected end of tokens in object");

            if (_tokens[_position].Kind == TokenKind.RightBrace)
            {
                _position++;
                return obj;
            }

            Expect(TokenKind.Comma);
        }
    }

    private JsonArray ParseArray()
    {
        Expect(TokenKind.LeftBracket);
        var array = new JsonArray();

        if (_position < _tokens.Count && _tokens[_position].Kind == TokenKind.RightBracket)
        {
            _position++;
            return array;
        }

        while (true)
        {
            array.Elements.Add(ParseValue());

            if (_position >= _tokens.Count)
                throw new InvalidOperationException("Unexpected end of tokens in array");

            if (_tokens[_position].Kind == TokenKind.RightBracket)
            {
                _position++;
                return array;
            }

            Expect(TokenKind.Comma);
        }
    }

    private JsonString ParseString()
    {
        var value = _tokens[_position].Value;
        _position++;
        return new JsonString(value);
    }

    private JsonNumber ParseNumber()
    {
        var value = double.Parse(_tokens[_position].Value, System.Globalization.CultureInfo.InvariantCulture);
        _position++;
        return new JsonNumber(value);
    }

    private JsonBool ParseBool()
    {
        var value = _tokens[_position].Kind == TokenKind.True;
        _position++;
        return new JsonBool(value);
    }

    private JsonNull ParseNull()
    {
        _position++;
        return new JsonNull();
    }

    private void Expect(TokenKind kind)
    {
        if (_position >= _tokens.Count)
            throw new InvalidOperationException($"Expected '{kind}' but reached end of tokens");

        if (_tokens[_position].Kind != kind)
            throw new InvalidOperationException($"Expected '{kind}' but got '{_tokens[_position].Kind}'");

        _position++;
    }
}
