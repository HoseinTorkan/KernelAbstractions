using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class MoneyJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new MoneyJsonConverter() }
    };

    [Fact]
    public void Serialize_ReturnsCorrectJsonShape()
    {
        var money = Money.Create(100, "IRR").Value;

        var json = JsonSerializer.Serialize(money, Options);

        Assert.Contains("\"amount\":100", json);
        Assert.Contains("\"currency\":\"IRR\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsMoney()
    {
        var json = """{"amount": 150.50, "currency": "USD"}""";

        var money = JsonSerializer.Deserialize<Money>(json, Options);

        Assert.Equal(150.50m, money.Amount);
        Assert.Equal("USD", money.Currency.Value);
    }

    [Fact]
    public void Deserialize_InvalidCurrency_ThrowsJsonException()
    {
        var json = """{"amount": 100, "currency": "XYZ"}""";

        var exception = Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Money>(json, Options));

        Assert.Contains("Kernel.ValueObjects.CurrencyCode.INVALID", exception.Message);
    }

    [Fact]
    public void Deserialize_MissingAmount_ThrowsJsonException()
    {
        var json = """{"currency": "IRR"}""";

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Money>(json, Options));
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        var original = Money.Create(123.45m, "EUR").Value;

        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<Money>(json, Options);

        Assert.Equal(original, deserialized);
    }
}
