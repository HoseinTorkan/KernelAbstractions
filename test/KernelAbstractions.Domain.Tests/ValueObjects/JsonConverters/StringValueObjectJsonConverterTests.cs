using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public abstract class StringValueObjectJsonConverterTests<TConverter, TValueObject>
    where TConverter : ValueObjectStringJsonConverter<TValueObject>, new()
    where TValueObject : struct
{
    // یک بار ساخته می‌شه و همه‌ی تست‌ها ازش استفاده می‌کنن (CA1869)
    private static readonly JsonSerializerOptions Options = new();

    protected TConverter Converter { get; } = new();

    protected abstract string ValidJson { get; }
    protected abstract TValueObject ExpectedValue { get; }

    [Fact]
    public void Deserialize_ValidJson_ReturnsValueObject()
    {
        var result = JsonSerializer.Deserialize<TValueObject>(ValidJson, Options);

        Assert.Equal(ExpectedValue, result);
    }

    [Fact]
    public void Deserialize_NonStringJson_ThrowsJsonException()
    {
        var json = "123";

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<TValueObject>(json, Options));
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        var json = JsonSerializer.Serialize(ExpectedValue, Options);
        var deserialized = JsonSerializer.Deserialize<TValueObject>(json, Options);

        Assert.Equal(ExpectedValue, deserialized);
    }
}
