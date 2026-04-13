using Cson.Parsing;
using Xunit;

namespace Tests;

public class ParserTests
{
    [Fact]
    public void Parse_EmptyObject_ReturnsJsonObject()
    {
        var tokens = new Lexer("{}").Tokenize();

        var node = new Parser(tokens).Parse();

        var obj = Assert.IsType<JsonObject>(node);
        Assert.Empty(obj.Properties);
    }

    [Fact]
    public void Parse_EmptyArray_ReturnsJsonArray()
    {
        var tokens = new Lexer("[]").Tokenize();

        var node = new Parser(tokens).Parse();

        var arr = Assert.IsType<JsonArray>(node);
        Assert.Empty(arr.Elements);
    }

    [Fact]
    public void Parse_ObjectWithStringProperty_ReturnsCorrectStructure()
    {
        var tokens = new Lexer("{\"name\": \"alice\"}").Tokenize();

        var node = new Parser(tokens).Parse();

        var obj = Assert.IsType<JsonObject>(node);
        Assert.Single(obj.Properties);
        var value = Assert.IsType<JsonString>(obj.Properties["name"]);
        Assert.Equal("alice", value.Value);
    }

    [Fact]
    public void Parse_NestedObject_ReturnsCorrectStructure()
    {
        var tokens = new Lexer("{\"a\": {\"b\": 1}}").Tokenize();

        var node = new Parser(tokens).Parse();

        var outer = Assert.IsType<JsonObject>(node);
        var inner = Assert.IsType<JsonObject>(outer.Properties["a"]);
        var num = Assert.IsType<JsonNumber>(inner.Properties["b"]);
        Assert.Equal(1.0, num.Value);
    }

    [Fact]
    public void Parse_ArrayWithMixedTypes_ReturnsCorrectElements()
    {
        var tokens = new Lexer("[1, \"two\", true, null]").Tokenize();

        var node = new Parser(tokens).Parse();

        var arr = Assert.IsType<JsonArray>(node);
        Assert.Equal(4, arr.Elements.Count);
        Assert.IsType<JsonNumber>(arr.Elements[0]);
        Assert.IsType<JsonString>(arr.Elements[1]);
        Assert.IsType<JsonBool>(arr.Elements[2]);
        Assert.IsType<JsonNull>(arr.Elements[3]);
    }

    [Fact]
    public void Parse_TrailingTokens_Throws()
    {
        var tokens = new Lexer("{}{}").Tokenize();

        Assert.Throws<InvalidOperationException>(() => new Parser(tokens).Parse());
    }
}
