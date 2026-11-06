using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public abstract class StringValueObjectTypeConverterTests<TConverter, TValueObject>
    where TConverter : TypeConverter, new()
    where TValueObject : struct
{
    private static readonly JsonSerializerOptions ErrorJsonOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = false,
    };

    // از فیلد به Property تغییر کرد تا CA1051 رفع بشه
    protected TConverter Converter { get; } = new();

    protected abstract string ValidInput { get; }
    protected abstract string InvalidInput { get; }
    protected abstract string ExpectedErrorCode { get; }

    [Fact]
    public void ConvertFrom_ValidInput_ReturnsValueObject()
    {
        var result = Converter.ConvertFrom(null, CultureInfo.InvariantCulture, ValidInput);

        Assert.IsType<TValueObject>(result);
    }

    [Fact]
    public void ConvertFrom_InvalidInput_ThrowsFormatExceptionWithJsonError()
    {
        var exception = Assert.Throws<FormatException>(
            () => Converter.ConvertFrom(null, CultureInfo.InvariantCulture, InvalidInput));

        var error = JsonSerializer.Deserialize<Error>(exception.Message, ErrorJsonOptions);

        Assert.NotNull(error);
        Assert.Equal(ExpectedErrorCode, error!.Code);
    }

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        Assert.True(Converter.CanConvertFrom(null, typeof(string)));
    }

    [Fact]
    public void CanConvertFrom_Int_ReturnsFalse()
    {
        Assert.False(Converter.CanConvertFrom(null, typeof(int)));
    }
}
