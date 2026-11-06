using KernelAbstractions.Domain.ValueObjects;

namespace KernelAbstractions.Domain.Tests.ValueObjects;

public class UrlTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://example.com/path")]
    [InlineData("https://example.com/path?query=1")]
    [InlineData("https://sub.domain.example.com")]
    [InlineData("https://example.com:8080/path")]
    public void Create_ValidUrl_ReturnsSuccess(string input)
    {
        var result = Url.Create(input);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("HTTPS://Example.COM/Path", "https://example.com/Path")]
    [InlineData("http://example.com/a b", "http://example.com/a%20b")]
    public void Create_NormalizesToAbsoluteUri(string input, string expected)
    {
        var result = Url.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyInput_ReturnsEmptyErrorCode(string input)
    {
        var result = Url.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Url.EMPTY", result.Error.Code);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("example.com")]
    [InlineData("/relative/path")]
    [InlineData("not a url")]
    public void Create_InvalidFormat_ReturnsInvalidFormatErrorCode(string input)
    {
        var result = Url.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Url.INVALID_FORMAT", result.Error.Code);
    }

    [Theory]
    [InlineData("ftp://example.com")]
    [InlineData("file:///C:/path")]
    [InlineData("mailto:user@example.com")]
    [InlineData("ssh://example.com")]
    public void Create_InvalidScheme_ReturnsInvalidSchemeErrorCode(string input)
    {
        var result = Url.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("Kernel.ValueObjects.Url.INVALID_SCHEME", result.Error.Code);
    }

    [Fact]
    public void Equality_SameNormalizedValue_ReturnsTrue()
    {
        var first = Url.Create("HTTPS://Example.COM/Path").Value;
        var second = Url.Create("https://example.com/Path").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var url = Url.Create("HTTPS://Example.COM").Value;

        var text = url.ToString();

        Assert.Equal("https://example.com/", text);
    }
}
