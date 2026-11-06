using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("09121234567", "09121234567")]
    [InlineData("+989121234567", "09121234567")]
    [InlineData("00989121234567", "09121234567")]
    [InlineData("989121234567", "09121234567")]
    [InlineData("0912-123-4567", "09121234567")]
    [InlineData("0912 123 4567", "09121234567")]
    [InlineData("(0912)1234567", "09121234567")]
    [InlineData("+98 912 123 4567", "09121234567")]
    public void Create_ValidInput_NormalizesToStandardFormat(string input, string expected)
    {
        var result = PhoneNumber.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyInput_ReturnsEmptyErrorCode(string input)
    {
        var result = PhoneNumber.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.PhoneNumber.EMPTY", result.Error.Code);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("09121234")]
    [InlineData("091212345678")]
    [InlineData("19121234567")]
    [InlineData("08121234567")]
    [InlineData("abc")]
    public void Create_InvalidFormat_ReturnsInvalidFormatErrorCode(string input)
    {
        var result = PhoneNumber.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.PhoneNumber.INVALID_FORMAT", result.Error.Code);
    }

    [Fact]
    public void Equality_SameNormalizedValue_ReturnsTrue()
    {
        var first = PhoneNumber.Create("+989121234567").Value;
        var second = PhoneNumber.Create("0912-123-4567").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var phone = PhoneNumber.Create("+989121234567").Value;

        var text = phone.ToString();

        Assert.Equal("09121234567", text);
    }
}
