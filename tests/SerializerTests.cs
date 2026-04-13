using Cson;
using Cson.Parsing;
using Xunit;

namespace Tests;

public class SerializerTests
{
    [Fact]
    public void Serialize_StringProperty_ReturnsValidJson()
    {
        var obj = new Employee("1", "alice");

        var json = Serializer.Serialize(obj);

        Assert.Equal("{\"Id\":\"1\",\"Name\":\"alice\"}", json);
    }

    [Fact]
    public void SerializeIndented_Object_ReturnsFormattedJson()
    {
        var obj = new Employee("1", "alice");

        var json = Serializer.SerializeIndented(obj);

        var expected = "{\n  \"Id\": \"1\",\n  \"Name\": \"alice\"\n}";
        Assert.Equal(expected, json.Replace("\r\n", "\n"));
    }

    [Fact]
    public void Serialize_Null_ReturnsNullString()
    {
        var json = Serializer.Serialize(null);

        Assert.Equal("null", json);
    }

    [Fact]
    public void Serialize_Boolean_ReturnsLowercase()
    {
        Assert.Equal("true", Serializer.Serialize(true));
        Assert.Equal("false", Serializer.Serialize(false));
    }

    [Fact]
    public void Serialize_Number_ReturnsCorrectRepresentation()
    {
        Assert.Equal("42", Serializer.Serialize(42));
        Assert.Equal("3.14", Serializer.Serialize(3.14));
    }

    [Fact]
    public void Serialize_StringWithEscapeCharacters_EscapesCorrectly()
    {
        var json = Serializer.Serialize("hello\nworld\t!");

        Assert.Equal("\"hello\\nworld\\t!\"", json);
    }

    [Fact]
    public void Serialize_Array_ReturnsJsonArray()
    {
        var json = Serializer.Serialize(new[] { 1, 2, 3 });

        Assert.Equal("[1,2,3]", json);
    }

    [Fact]
    public void SerializeIndented_Array_ReturnsFormattedJsonArray()
    {
        var json = Serializer.SerializeIndented(new[] { 1, 2 });

        var expected = "[\n  1,\n  2\n]";
        Assert.Equal(expected, json.Replace("\r\n", "\n"));
    }
}
