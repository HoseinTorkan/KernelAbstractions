using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using System.Globalization;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class DataSizeTypeConverterTests
{
    private readonly DataSizeTypeConverter _converter = new();

    [Theory]
    [InlineData("1.5 GB", 1.5, DataSizeUnit.Gigabyte)]
    [InlineData("1.5 Gigabyte", 1.5, DataSizeUnit.Gigabyte)]
    [InlineData("500 MB", 500, DataSizeUnit.Megabyte)]
    [InlineData("2 TB", 2, DataSizeUnit.Terabyte)]
    [InlineData("100 KB", 100, DataSizeUnit.Kilobyte)]
    [InlineData("10 B", 10, DataSizeUnit.Byte)]
    public void ConvertFrom_ValidInput_ReturnsDataSize(string input, decimal expectedValue, DataSizeUnit expectedUnit)
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        var size = Assert.IsType<DataSize>(result);
        Assert.Equal(expectedValue, size.Value);
        Assert.Equal(expectedUnit, size.Unit);
    }

    [Theory]
    [InlineData("1.5")]             // only one part
    [InlineData("1.5 GB extra")]    // three parts
    [InlineData("abc GB")]          // value not numeric
    [InlineData("1.5 XB")]          // unknown unit
    [InlineData("-1 GB")]           // negative
    public void ConvertFrom_InvalidInput_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(
            () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input));
    }
}
