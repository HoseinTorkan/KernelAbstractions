using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using System.Text.Json;
using Xunit;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class SizeJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new SizeJsonConverter() }
    };

    [Fact]
    public void Serialize_ReturnsCorrectJsonShape()
    {
        var size = Size.Create(5.5m, SizeUnit.Centimeter).Value;

        var json = JsonSerializer.Serialize(size, Options);

        Assert.Contains("\"value\":5.5", json);
        Assert.Contains("\"unit\":\"Centimeter\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsSize()
    {
        var json = """{"value": 10, "unit": "Inch"}""";

        var size = JsonSerializer.Deserialize<Size>(json, Options);

        Assert.Equal(10m, size.Value);
        Assert.Equal(SizeUnit.Inch, size.Unit);
    }

    [Fact]
    public void Deserialize_UnknownUnit_ThrowsJsonException()
    {
        var json = """{"value": 10, "unit": "Yard"}""";

        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Size>(json, Options));

        Assert.Contains("not recognized", exception.Message);
    }

    [Fact]
    public void Deserialize_InvalidValue_ThrowsJsonException()
    {
        var json = """{"value": -5, "unit": "Centimeter"}""";

        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Size>(json, Options));

        Assert.Contains("Kernel.ValueObjects.Size.INVALID_VALUE", exception.Message);
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        var original = Size.Create(42.5m, SizeUnit.Meter).Value;

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<Size>(json, Options);

        Assert.Equal(original, deserialized);
    }
}
