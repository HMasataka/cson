using System.Collections;
using System.Reflection;
using System.Text;

namespace Cson;

public static class Serializer
{
    public static string Serialize(object? value)
    {
        var sb = new StringBuilder();
        WriteValue(sb, value, 0, false);
        return sb.ToString();
    }

    public static string SerializeIndented(object? value)
    {
        var sb = new StringBuilder();
        WriteValue(sb, value, 0, true);
        return sb.ToString();
    }

    private static void WriteValue(StringBuilder sb, object? value, int depth, bool indented)
    {
        if (value is null)
        {
            sb.Append("null");
            return;
        }

        switch (value)
        {
            case string s:
                WriteString(sb, s);
                return;
            case bool b:
                sb.Append(b ? "true" : "false");
                return;
            case int or long or float or double or decimal:
                sb.Append(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                return;
            case IEnumerable enumerable:
                WriteArray(sb, enumerable, depth, indented);
                return;
            default:
                WriteObject(sb, value, depth, indented);
                return;
        }
    }

    private static void WriteString(StringBuilder sb, string value)
    {
        sb.Append('"');

        foreach (var ch in value)
        {
            switch (ch)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (ch < 0x20)
                        sb.Append($"\\u{(int)ch:X4}");
                    else
                        sb.Append(ch);
                    break;
            }
        }

        sb.Append('"');
    }

    private static void WriteObject(StringBuilder sb, object value, int depth, bool indented)
    {
        var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        sb.Append('{');

        var first = true;

        foreach (var prop in properties)
        {
            if (!first)
                sb.Append(',');

            first = false;

            if (indented)
            {
                sb.AppendLine();
                sb.Append(new string(' ', (depth + 1) * 2));
            }

            WriteString(sb, prop.Name);
            sb.Append(':');

            if (indented)
                sb.Append(' ');

            WriteValue(sb, prop.GetValue(value), depth + 1, indented);
        }

        if (!first && indented)
        {
            sb.AppendLine();
            sb.Append(new string(' ', depth * 2));
        }

        sb.Append('}');
    }

    private static void WriteArray(StringBuilder sb, IEnumerable enumerable, int depth, bool indented)
    {
        sb.Append('[');

        var first = true;

        foreach (var item in enumerable)
        {
            if (!first)
                sb.Append(',');

            first = false;

            if (indented)
            {
                sb.AppendLine();
                sb.Append(new string(' ', (depth + 1) * 2));
            }

            WriteValue(sb, item, depth + 1, indented);
        }

        if (!first && indented)
        {
            sb.AppendLine();
            sb.Append(new string(' ', depth * 2));
        }

        sb.Append(']');
    }
}
