using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.Globalization;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class MoneyTypeConverterTests
{
    // تنها جایی که JsonSerializerOptions ساخته می‌شه (یک بار برای همیشه)
    private static readonly JsonSerializerOptions ErrorJsonOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = false,
    };

    private readonly MoneyTypeConverter _converter = new();

    [Fact]
    public void ConvertFrom_ValidInput_ReturnsMoney()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "100 IRR");

        var money = Assert.IsType<Money>(result);
        Assert.Equal(100m, money.Amount);
        Assert.Equal("IRR", money.Currency.Value);
    }

    [Theory]
    [InlineData("100")]
    [InlineData("100 IRR extra")]
    [InlineData("abc IRR")]
    [InlineData("100 XYZ")]
    public void ConvertFrom_InvalidInput_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input));
    }

    [Fact]
    public void ConvertFrom_InvalidCurrency_ExceptionContainsDomainError()
    {
        var exception = Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "100 XYZ"));

        // از همون ErrorJsonOptions استاتیک استفاده می‌کنه (بدون new)
        var error = JsonSerializer.Deserialize<Error>(exception.Message, ErrorJsonOptions);

        Assert.NotNull(error);
        Assert.Equal("Kernel.ValueObjects.CurrencyCode.INVALID", error!.Code);
    }
}
