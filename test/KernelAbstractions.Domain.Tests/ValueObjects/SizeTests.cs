using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class SizeTests
{
    [Theory]
    [InlineData(100, SizeUnit.Centimeter)]
    [InlineData(0.1, SizeUnit.Millimeter)]
    [InlineData(500, SizeUnit.Inch)]
    public void Create_ValidSize_ReturnsSuccess(decimal value, SizeUnit unit)
    {
        var result = Size.Create(value, unit);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_NonPositiveValue_ReturnsInvalidValueErrorCode(decimal value)
    {
        var result = Size.Create(value, SizeUnit.Centimeter);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Size.INVALID_VALUE", result.Error.Code);
    }

    [Fact]
    public void Create_TooLargeValue_ReturnsTooLargeErrorCode()
    {
        var result = Size.Create(10_001, SizeUnit.Centimeter);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Size.TOO_LARGE", result.Error.Code);
    }

    [Theory]
    [InlineData(5.555, SizeUnit.Centimeter, 5.56)]
    [InlineData(5.554, SizeUnit.Centimeter, 5.55)]
    public void Create_RoundsToTwoDecimalPlaces(decimal value, SizeUnit unit, decimal expected)
    {
        var result = Size.Create(value, unit);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Fact]
    public void ToMillimeter_FromCentimeter_ReturnsCorrectValue()
    {
        var size = Size.Create(5, SizeUnit.Centimeter).Value;

        var result = size.ToMillimeter();

        Assert.Equal(50m, result.Value);
        Assert.Equal(SizeUnit.Millimeter, result.Unit);
    }

    [Fact]
    public void ToMillimeter_FromInch_ReturnsCorrectValue()
    {
        var size = Size.Create(1, SizeUnit.Inch).Value;

        var result = size.ToMillimeter();

        Assert.Equal(25.4m, result.Value);
        Assert.Equal(SizeUnit.Millimeter, result.Unit);
    }

    [Fact]
    public void ToCentimeter_FromMillimeter_ReturnsCorrectValue()
    {
        var size = Size.Create(50, SizeUnit.Millimeter).Value;

        var result = size.ToCentimeter();

        Assert.Equal(5m, result.Value);
        Assert.Equal(SizeUnit.Centimeter, result.Unit);
    }

    [Fact]
    public void ToInch_FromCentimeter_ReturnsCorrectValue()
    {
        var size = Size.Create(2.54m, SizeUnit.Centimeter).Value;

        var result = size.ToInch();

        Assert.Equal(1m, result.Value);
        Assert.Equal(SizeUnit.Inch, result.Unit);
    }

    [Fact]
    public void ToMeter_FromCentimeter_ReturnsCorrectValue()
    {
        var size = Size.Create(100, SizeUnit.Centimeter).Value;

        var result = size.ToMeter();

        Assert.Equal(1m, result.Value);
        Assert.Equal(SizeUnit.Meter, result.Unit);
    }

    [Fact]
    public void ToFoot_FromInch_ReturnsCorrectValue()
    {
        var size = Size.Create(12, SizeUnit.Inch).Value;

        var result = size.ToFoot();

        Assert.Equal(1m, result.Value);
        Assert.Equal(SizeUnit.Foot, result.Unit);
    }

    [Fact]
    public void ToSameUnit_ReturnsSameInstance()
    {
        var size = Size.Create(5, SizeUnit.Centimeter).Value;

        var result = size.ToCentimeter();

        Assert.Equal(size, result);
    }

    [Fact]
    public void Equality_SameValueAndUnit_ReturnsTrue()
    {
        var first = Size.Create(5, SizeUnit.Centimeter).Value;
        var second = Size.Create(5, SizeUnit.Centimeter).Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Equality_DifferentUnit_ReturnsFalse()
    {
        var first = Size.Create(5, SizeUnit.Centimeter).Value;
        var second = Size.Create(5, SizeUnit.Millimeter).Value;

        Assert.NotEqual(first, second);
        Assert.False(first == second);
    }

    [Fact]
    public void ToString_ReturnsValueAndUnit()
    {
        var size = Size.Create(5.5m, SizeUnit.Centimeter).Value;

        var text = size.ToString();

        Assert.Equal("5.5 Centimeter", text);
    }
}
