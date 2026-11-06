using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class CurrencyCodeTests
{
    [Theory]
    [InlineData("IRR")]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("AED")]
    [InlineData("IRT")] // Non-ISO extension for Iranian Toman
    public void Create_ValidCurrency_ReturnsSuccess(string input)
    {
        var result = CurrencyCode.Create(input);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("irr", "IRR")]
    [InlineData("usd", "USD")]
    [InlineData("  IRR  ", "IRR")]
    public void Create_LowerCaseOrWhitespace_ReturnsNormalizedUppercase(string input, string expected)
    {
        var result = CurrencyCode.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyInput_ReturnsEmptyErrorCode(string input)
    {
        var result = CurrencyCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.CurrencyCode.EMPTY", result.Error.Code);
    }

    [Theory]
    [InlineData("XYZ")] // structurally valid but unassigned
    [InlineData("IR")]  // too short
    [InlineData("IRRR")] // too long
    [InlineData("IR1")] // contains a digit
    [InlineData("IR-")] // contains a symbol
    public void Create_InvalidCurrency_ReturnsInvalidErrorCode(string input)
    {
        var result = CurrencyCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.CurrencyCode.INVALID", result.Error.Code);
    }

    [Fact]
    public void Equality_SameNormalizedValue_ReturnsTrue()
    {
        var first = CurrencyCode.Create("irr").Value;
        var second = CurrencyCode.Create("IRR").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var code = CurrencyCode.Create("usd").Value;

        var text = code.ToString();

        Assert.Equal("USD", text);
    }
}
