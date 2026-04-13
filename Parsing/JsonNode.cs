namespace Cson.Parsing;

public abstract class JsonNode { }

public sealed class JsonObject : JsonNode
{
    public Dictionary<string, JsonNode> Properties { get; } = new();
}

public sealed class JsonArray : JsonNode
{
    public List<JsonNode> Elements { get; } = new();
}

public sealed class JsonString : JsonNode
{
    public string Value { get; }

    public JsonString(string value)
    {
        Value = value;
    }
}

public sealed class JsonNumber : JsonNode
{
    public double Value { get; }

    public JsonNumber(double value)
    {
        Value = value;
    }
}

public sealed class JsonBool : JsonNode
{
    public bool Value { get; }

    public JsonBool(bool value)
    {
        Value = value;
    }
}

public sealed class JsonNull : JsonNode { }
