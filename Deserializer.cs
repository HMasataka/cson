using System.Reflection;
using Cson.Parsing;

namespace Cson;

public static class Deserializer
{
    public static T Deserialize<T>(string json)
    {
        var lexer = new Lexer(json);
        var tokens = lexer.Tokenize();
        var parser = new Parser(tokens);
        var node = parser.Parse();

        return (T)ConvertNode(node, typeof(T));
    }

    private static object ConvertNode(JsonNode node, Type targetType)
    {
        return node switch
        {
            JsonNull => throw new InvalidOperationException($"Cannot assign null to non-nullable type '{targetType}'"),
            JsonString s => ConvertString(s.Value, targetType),
            JsonNumber n => ConvertNumber(n.Value, targetType),
            JsonBool b => b.Value,
            JsonArray a => ConvertArray(a, targetType),
            JsonObject o => ConvertObject(o, targetType),
            _ => throw new InvalidOperationException($"Unknown JsonNode type '{node.GetType()}'"),
        };
    }

    private static object ConvertString(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        throw new InvalidOperationException($"Cannot convert string to '{targetType}'");
    }

    private static object ConvertNumber(double value, Type targetType)
    {
        if (targetType == typeof(int)) return (int)value;
        if (targetType == typeof(long)) return (long)value;
        if (targetType == typeof(float)) return (float)value;
        if (targetType == typeof(double)) return value;
        if (targetType == typeof(decimal)) return (decimal)value;

        throw new InvalidOperationException($"Cannot convert number to '{targetType}'");
    }

    private static object ConvertArray(JsonArray array, Type targetType)
    {
        if (!targetType.IsArray)
            throw new InvalidOperationException($"Cannot convert array to '{targetType}'");

        var elementType = targetType.GetElementType()!;
        var result = Array.CreateInstance(elementType, array.Elements.Count);

        for (var i = 0; i < array.Elements.Count; i++)
        {
            result.SetValue(ConvertNode(array.Elements[i], elementType), i);
        }

        return result;
    }

    private static object ConvertObject(JsonObject obj, Type targetType)
    {
        // positional record: コンストラクタパラメータ名でJSONキーを照合
        var constructor = targetType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
        var parameters = constructor.GetParameters();

        var args = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var key = FindMatchingKey(obj.Properties, param.Name!);

            if (key is null)
                throw new InvalidOperationException($"Missing JSON property for constructor parameter '{param.Name}' on type '{targetType}'");

            args[i] = ConvertNode(obj.Properties[key], param.ParameterType);
        }

        return constructor.Invoke(args);
    }

    private static string? FindMatchingKey(Dictionary<string, JsonNode> properties, string parameterName)
    {
        // case-insensitive でJSONキーを照合
        foreach (var key in properties.Keys)
        {
            if (string.Equals(key, parameterName, StringComparison.OrdinalIgnoreCase))
                return key;
        }

        return null;
    }
}
