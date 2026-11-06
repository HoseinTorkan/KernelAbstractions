using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class DataSizeJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new DataSizeJsonConverter() }
    };

    [Fact]
    public void Serialize_ReturnsCorrectJsonShape()
    {
        var size = DataSize.Create(1.5m, DataSizeUnit.Gigabyte).Value;

        var json = JsonSerializer.Serialize(size, Options);

        Assert.Contains("\"value\":1.5", json);
        Assert.Contains("\"unit\":\"Gigabyte\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsDataSize()
    {
        var json = """{"value": 500, "unit": "Megabyte"}""";

        var size = JsonSerializer.Deserialize<DataSize>(json, Options);

        Assert.Equal(500m, size.Value);
        Assert.Equal(DataSizeUnit.Megabyte, size.Unit);
    }

    [Fact]
    public void Deserialize_UnknownUnit_ThrowsJsonException()
    {
        var json = """{"value": 1, "unit": "Petabyte"}""";

        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<DataSize>(json, Options));

        Assert.Contains("not recognized", exception.Message);
    }

    [Fact]
    public void Deserialize_NegativeValue_ThrowsJsonException()
    {
        var json = """{"value": -1, "unit": "Gigabyte"}""";

        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<DataSize>(json, Options));

        Assert.Contains("Kernel.ValueObjects.DataSize.INVALID_VALUE", exception.Message);
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        var original = DataSize.Create(1024, DataSizeUnit.Terabyte).Value;

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<DataSize>(json, Options);

        Assert.Equal(original, deserialized);
    }
}
