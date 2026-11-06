using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("user_name@example.com")]
    [InlineData("user-name@example.com")]
    [InlineData("user123@example.com")]
    public void Create_ValidEmail_ReturnsSuccess(string input)
    {
        var result = Email.Create(input);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_UpperCase_ReturnsLowercase()
    {
        var result = Email.Create("User@Example.COM");

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyInput_ReturnsEmptyErrorCode(string input)
    {
        var result = Email.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Email.EMPTY", result.Error.Code);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abc@")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@example")]
    [InlineData("user@@example.com")]
    [InlineData("user..name@example.com")]
    [InlineData(".user@example.com")]
    [InlineData("user.@example.com")]
    [InlineData("user@example..com")]
    [InlineData("user@.example.com")]
    [InlineData("user@example.c")]
    public void Create_InvalidFormat_ReturnsInvalidFormatErrorCode(string input)
    {
        var result = Email.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Email.INVALID_FORMAT", result.Error.Code);
    }

    [Fact]
    public void Create_TooLong_ReturnsTooLongErrorCode()
    {
        var input = new string('a', 250) + "@example.com";

        var result = Email.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Email.TOO_LONG", result.Error.Code);
    }

    [Fact]
    public void Equality_SameNormalizedValue_ReturnsTrue()
    {
        var first = Email.Create("User@Example.com").Value;
        var second = Email.Create("user@example.com").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var email = Email.Create("User@Example.COM").Value;

        var text = email.ToString();

        Assert.Equal("user@example.com", text);
    }
}
