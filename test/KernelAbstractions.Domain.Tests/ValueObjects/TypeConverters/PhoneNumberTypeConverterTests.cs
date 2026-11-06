using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.Globalization;
using System.Text.Json;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class PhoneNumberTypeConverterTests
{
    private readonly PhoneNumberTypeConverter _converter = new();

    [Fact]
    public void ConvertFrom_ValidInput_ReturnsPhoneNumber()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "09121234567");

        var phone = Assert.IsType<PhoneNumber>(result);
        Assert.Equal("09121234567", phone.Value);
    }

    [Fact]
    public void ConvertFrom_InvalidInput_ThrowsFormatException()
    {
        var exception = Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "123"));

        Assert.False(string.IsNullOrWhiteSpace(exception.Message));
    }

    [Fact]
    public void ConvertFrom_InvalidInput_ExceptionMessageContainsValidJson()
    {
        var exception = Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "123"));

        // This proves the contract with KernelModelStateFilter is intact.
        var error = JsonSerializer.Deserialize<Error>(exception.Message, GetOptions());

        Assert.NotNull(error);
        Assert.Equal("Kernel.ValueObjects.PhoneNumber.INVALID_FORMAT", error!.Code);
    }

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        Assert.True(_converter.CanConvertFrom(null, typeof(string)));
    }

    [Fact]
    public void CanConvertFrom_Int_ReturnsFalse()
    {
        Assert.False(_converter.CanConvertFrom(null, typeof(int)));
    }

    private static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = false,
        };
    }
}
