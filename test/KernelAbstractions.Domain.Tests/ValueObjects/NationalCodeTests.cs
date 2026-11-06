using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class NationalCodeTests
{
    [Theory]
    [InlineData("0499370899")] // valid: checksum = 9 (11 - (266 % 11) = 11 - 2 = 9)
    [InlineData("0012345679")] // valid: checksum = 9 (11 - (112 % 11) = 11 - 2 = 9)
    public void Create_ValidNationalCode_ReturnsSuccess(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithSeparators_ReturnsNormalizedValue()
    {
        var result = NationalCode.Create("049-937-0899");

        Assert.True(result.IsSuccess);
        Assert.Equal("0499370899", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyInput_ReturnsEmptyErrorCode(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.NationalCode.EMPTY", result.Error.Code);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    [InlineData("12345abcde")]
    public void Create_InvalidFormat_ReturnsInvalidFormatErrorCode(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.NationalCode.INVALID_FORMAT", result.Error.Code);
    }

    [Theory]
    [InlineData("1111111111")]
    [InlineData("0000000000")]
    [InlineData("9999999999")]
    public void Create_AllIdenticalDigits_ReturnsInvalidChecksumErrorCode(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.NationalCode.INVALID_CHECKSUM", result.Error.Code);
    }

    [Theory]
    [InlineData("0012345678")] // checksum should be 9, not 8
    [InlineData("1234567890")] // checksum should be 6, not 0
    public void Create_InvalidChecksum_ReturnsInvalidChecksumErrorCode(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.NationalCode.INVALID_CHECKSUM", result.Error.Code);
    }

    [Fact]
    public void Equality_SameNormalizedValue_ReturnsTrue()
    {
        var first = NationalCode.Create("049-937-0899").Value;
        var second = NationalCode.Create("0499370899").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var code = NationalCode.Create("049-937-0899").Value;

        var text = code.ToString();

        Assert.Equal("0499370899", text);
    }
}
