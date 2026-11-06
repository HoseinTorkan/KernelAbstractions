using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class PersonNameTests
{
    [Theory]
    [InlineData("علی")]
    [InlineData("فاطمه‌زهرا")]
    [InlineData("O'Brien")]
    [InlineData("Jean-Pierre")]
    public void Create_ValidName_ReturnsSuccess(string input)
    {
        // Act
        var result = PersonName.Create(input);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ع")]           // تک‌حرفی
    [InlineData("علی123")]      // عدد داره
    public void Create_InvalidName_ReturnsFailure(string input)
    {
        // Act
        var result = PersonName.Create(input);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_Empty_ReturnsEmptyErrorCode()
    {
        // Act
        var result = PersonName.Create("");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.PersonName.EMPTY", result.Error.Code);
    }
}
