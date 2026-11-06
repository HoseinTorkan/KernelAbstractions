using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class DataSizeTests
{
    [Theory]
    [InlineData(0, DataSizeUnit.Byte)]
    [InlineData(1.5, DataSizeUnit.Gigabyte)]
    [InlineData(500, DataSizeUnit.Megabyte)]
    public void Create_ValidSize_ReturnsSuccess(decimal value, DataSizeUnit unit)
    {
        var result = DataSize.Create(value, unit);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_NegativeValue_ReturnsInvalidValueErrorCode()
    {
        var result = DataSize.Create(-1, DataSizeUnit.Byte);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.DataSize.INVALID_VALUE", result.Error.Code);
    }

    [Fact]
    public void Create_TooLargeValue_ReturnsTooLargeErrorCode()
    {
        var result = DataSize.Create(1_000_001, DataSizeUnit.Megabyte);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.DataSize.TOO_LARGE", result.Error.Code);
    }

    [Theory]
    [InlineData(1.555, DataSizeUnit.Gigabyte, 1.56)]
    [InlineData(1.554, DataSizeUnit.Gigabyte, 1.55)]
    public void Create_RoundsToTwoDecimalPlaces(decimal value, DataSizeUnit unit, decimal expected)
    {
        var result = DataSize.Create(value, unit);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Fact]
    public void Zero_ReturnsZeroInUnit()
    {
        var result = DataSize.Zero(DataSizeUnit.Byte);

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Value.Value);
        Assert.Equal(DataSizeUnit.Byte, result.Value.Unit);
    }

    [Fact]
    public void ToByte_FromKilobyte_ReturnsCorrectValue()
    {
        var size = DataSize.Create(1, DataSizeUnit.Kilobyte).Value;

        var result = size.ToByte();

        Assert.Equal(1024m, result.Value);
        Assert.Equal(DataSizeUnit.Byte, result.Unit);
    }

    [Fact]
    public void ToKilobyte_FromMegabyte_ReturnsCorrectValue()
    {
        var size = DataSize.Create(1, DataSizeUnit.Megabyte).Value;

        var result = size.ToKilobyte();

        Assert.Equal(1024m, result.Value);
        Assert.Equal(DataSizeUnit.Kilobyte, result.Unit);
    }

    [Fact]
    public void ToMegabyte_FromGigabyte_ReturnsCorrectValue()
    {
        var size = DataSize.Create(1, DataSizeUnit.Gigabyte).Value;

        var result = size.ToMegabyte();

        Assert.Equal(1024m, result.Value);
        Assert.Equal(DataSizeUnit.Megabyte, result.Unit);
    }

    [Fact]
    public void ToGigabyte_FromTerabyte_ReturnsCorrectValue()
    {
        var size = DataSize.Create(1, DataSizeUnit.Terabyte).Value;

        var result = size.ToGigabyte();

        Assert.Equal(1024m, result.Value);
        Assert.Equal(DataSizeUnit.Gigabyte, result.Unit);
    }

    [Fact]
    public void ToSameUnit_ReturnsSameInstance()
    {
        var size = DataSize.Create(5, DataSizeUnit.Megabyte).Value;

        var result = size.ToMegabyte();

        Assert.Equal(size, result);
    }

    [Fact]
    public void Equality_SameValueAndUnit_ReturnsTrue()
    {
        var first = DataSize.Create(1.5m, DataSizeUnit.Gigabyte).Value;
        var second = DataSize.Create(1.5m, DataSizeUnit.Gigabyte).Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Equality_DifferentUnit_ReturnsFalse()
    {
        var first = DataSize.Create(1, DataSizeUnit.Megabyte).Value;
        var second = DataSize.Create(1, DataSizeUnit.Gigabyte).Value;

        Assert.NotEqual(first, second);
        Assert.False(first == second);
    }

    [Fact]
    public void ToString_ReturnsValueAndUnit()
    {
        var size = DataSize.Create(1.5m, DataSizeUnit.Gigabyte).Value;

        var text = size.ToString();

        Assert.Equal("1.5 Gigabyte", text);
    }
}
