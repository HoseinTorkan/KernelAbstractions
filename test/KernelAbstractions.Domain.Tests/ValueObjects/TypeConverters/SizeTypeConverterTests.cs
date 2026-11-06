using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using System.Globalization;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class SizeTypeConverterTests
{
    private readonly SizeTypeConverter _converter = new();

    [Theory]
    [InlineData("100 cm", 100, SizeUnit.Centimeter)]
    [InlineData("100 Centimeter", 100, SizeUnit.Centimeter)]
    [InlineData("5.5 in", 5.5, SizeUnit.Inch)]
    [InlineData("2 m", 2, SizeUnit.Meter)]
    [InlineData("1 ft", 1, SizeUnit.Foot)]
    [InlineData("10 mm", 10, SizeUnit.Millimeter)]
    public void ConvertFrom_ValidInput_ReturnsSize(string input, decimal expectedValue, SizeUnit expectedUnit)
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        var size = Assert.IsType<Size>(result);
        Assert.Equal(expectedValue, size.Value);
        Assert.Equal(expectedUnit, size.Unit);
    }

    [Theory]
    [InlineData("100")]             // only one part
    [InlineData("100 cm extra")]    // three parts
    [InlineData("abc cm")]          // value not numeric
    [InlineData("100 yd")]          // unknown unit
    [InlineData("0 cm")]            // not positive
    public void ConvertFrom_InvalidInput_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input));
    }
}
