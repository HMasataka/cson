using Cson;
using Xunit;

namespace Tests;

public class DeserializerTests
{
    [Fact]
    public void Deserialize_PositionalRecord_ReturnsCorrectInstance()
    {
        var json = "{\"Id\": \"user_1\", \"Name\": \"alice\"}";

        var result = Deserializer.Deserialize<Employee>(json);

        Assert.Equal("user_1", result.Id);
        Assert.Equal("alice", result.Name);
    }

    [Fact]
    public void Deserialize_CaseInsensitiveKeys_MatchesConstructorParameters()
    {
        var json = "{\"id\": \"user_2\", \"name\": \"bob\"}";

        var result = Deserializer.Deserialize<Employee>(json);

        Assert.Equal("user_2", result.Id);
        Assert.Equal("bob", result.Name);
    }

    [Fact]
    public void Deserialize_MissingProperty_Throws()
    {
        var json = "{\"Id\": \"user_3\"}";

        Assert.Throws<InvalidOperationException>(() => Deserializer.Deserialize<Employee>(json));
    }

    [Fact]
    public void Deserialize_RoundTrip_PreservesValues()
    {
        var original = new Employee("round_trip_id", "round_trip_name");
        var json = Serializer.Serialize(original);

        var deserialized = Deserializer.Deserialize<Employee>(json);

        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Name, deserialized.Name);
    }

    [Fact]
    public void Deserialize_InvalidJson_Throws()
    {
        var json = "{invalid}";

        Assert.Throws<InvalidOperationException>(() => Deserializer.Deserialize<Employee>(json));
    }
}
