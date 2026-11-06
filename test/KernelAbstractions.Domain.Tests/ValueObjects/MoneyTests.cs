using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(100, "IRR")]
    [InlineData(0, "USD")]
    [InlineData(-50, "EUR")]
    public void Create_ValidAmount_ReturnsSuccess(decimal amount, string currency)
    {
        var result = Money.Create(amount, currency);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(100.556, "IRR", 100.56)]
    [InlineData(100.554, "IRR", 100.55)]
    [InlineData(100.555, "IRR", 100.56)] // MidpointRounding.AwayFromZero
    public void Create_RoundsToTwoDecimalPlaces(decimal amount, string currency, decimal expectedAmount)
    {
        var result = Money.Create(amount, currency);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedAmount, result.Value.Amount);
    }

    [Fact]
    public void Create_InvalidCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, "XYZ");

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.CurrencyCode.INVALID", result.Error.Code);
    }

    [Fact]
    public void Create_EmptyCurrency_ReturnsFailure()
    {
        var result = Money.Create(100, "");

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.CurrencyCode.EMPTY", result.Error.Code);
    }

    [Fact]
    public void Zero_ReturnsZeroAmount()
    {
        var result = Money.Zero("IRR");

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Value.Amount);
    }

    [Fact]
    public void Addition_SameCurrency_ReturnsSum()
    {
        var a = Money.Create(100, "IRR").Value;
        var b = Money.Create(50, "IRR").Value;

        var sum = a + b;

        Assert.Equal(150m, sum.Amount);
        Assert.Equal("IRR", sum.Currency.Value);
    }

    [Fact]
    public void Addition_DifferentCurrency_ThrowsInvalidOperationException()
    {
        var a = Money.Create(100, "IRR").Value;
        var b = Money.Create(50, "USD").Value;

        Assert.Throws<InvalidOperationException>(() => _ = a + b);
    }

    [Fact]
    public void Subtraction_ReturnsDifference()
    {
        var a = Money.Create(100, "IRR").Value;
        var b = Money.Create(30, "IRR").Value;

        var diff = a - b;

        Assert.Equal(70m, diff.Amount);
    }

    [Fact]
    public void Subtraction_ResultMayBeNegative()
    {
        var a = Money.Create(30, "IRR").Value;
        var b = Money.Create(100, "IRR").Value;

        var diff = a - b;

        Assert.Equal(-70m, diff.Amount);
    }

    [Fact]
    public void Multiplication_ReturnsProduct()
    {
        var money = Money.Create(100, "IRR").Value;

        var result = money * 2.5m;

        Assert.Equal(250m, result.Amount);
    }

    [Fact]
    public void Division_ReturnsQuotient()
    {
        var money = Money.Create(100, "IRR").Value;

        var result = money / 4m;

        Assert.Equal(25m, result.Amount);
    }

    [Fact]
    public void Division_ByZero_ThrowsDivideByZeroException()
    {
        var money = Money.Create(100, "IRR").Value;

        Assert.Throws<DivideByZeroException>(() => _ = money / 0m);
    }

    [Fact]
    public void UnaryMinus_NegatesAmount()
    {
        var money = Money.Create(100, "IRR").Value;

        var negated = -money;

        Assert.Equal(-100m, negated.Amount);
    }

    [Fact]
    public void Comparison_WorksForSameCurrency()
    {
        var a = Money.Create(100, "IRR").Value;
        var b = Money.Create(50, "IRR").Value;

        Assert.True(a > b);
        Assert.True(b < a);
        Assert.True(a >= b);
        Assert.True(b <= a);
    }

    [Fact]
    public void Comparison_DifferentCurrency_ThrowsInvalidOperationException()
    {
        var a = Money.Create(100, "IRR").Value;
        var b = Money.Create(50, "USD").Value;

        Assert.Throws<InvalidOperationException>(() => _ = a > b);
    }

    [Fact]
    public void ToString_ReturnsFormattedValue()
    {
        var money = Money.Create(1234.56m, "IRR").Value;

        var text = money.ToString();

        Assert.Equal("1,234.56 IRR", text);
    }
}
