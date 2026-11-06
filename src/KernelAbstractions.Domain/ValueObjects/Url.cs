using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a valid absolute HTTP or HTTPS URL.
/// </summary>
/// <remarks>
/// <para>
/// This value object guarantees that:
/// </para>
/// <list type="bullet">
///   <item><description>The value is a syntactically valid absolute URI.</description></item>
///   <item><description>The URI scheme is either <c>http</c> or <c>https</c>.</description></item>
///   <item><description>
///     The stored value is the <b>normalized</b> form produced by
///     <see cref="Uri.AbsoluteUri"/>: percent-encoding is applied to unsafe characters,
///     and the scheme and host are lowercased. This guarantees that two URLs differing
///     only in casing or unescaped characters compare equal.
///   </description></item>
/// </list>
/// <para>
/// <b>What this type does not check:</b> Whether the URL points to a specific kind of
/// resource (image, HTML page, API endpoint, etc.). The content type of a URL is only
/// known after a request and is a concern of the application layer, not of this value
/// object. Use property names (e.g. <c>AvatarUrl</c>, <c>WebsiteUrl</c>) to express
/// intent.
/// </para>
/// </remarks>
[JsonConverter(typeof(UrlJsonConverter))]
[TypeConverter(typeof(UrlTypeConverter))]
public readonly record struct Url
{
    /// <summary>
    /// Gets the normalized absolute URL.
    /// </summary>
    public string Value { get; }

    private Url(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="Url"/> from the given string.
    /// </summary>
    /// <param name="value">The raw URL. Leading and trailing whitespace is trimmed.</param>
    /// <returns>
    /// A successful result carrying the normalized URL, or a failure with one of:
    /// <list type="bullet">
    ///   <item><description><c>Kernel.ValueObjects.Url.EMPTY</c> when the input is null or whitespace.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.Url.INVALID_FORMAT</c> when the input is not a valid absolute URI.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.Url.INVALID_SCHEME</c> when the scheme is not <c>http</c> or <c>https</c>.</description></item>
    /// </list>
    /// </returns>
    public static Result<Url> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Url>.Failure(
                new Error("Kernel.ValueObjects.Url.EMPTY",
                          "URL cannot be empty.",
                          ErrorType.ValidationError));
        }

        string trimmed = value.Trim();

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? uri))
        {
            return Result<Url>.Failure(
                new Error("Kernel.ValueObjects.Url.INVALID_FORMAT",
                          "URL is not a valid absolute URI.",
                          ErrorType.ValidationError));
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            return Result<Url>.Failure(
                new Error("Kernel.ValueObjects.Url.INVALID_SCHEME",
                          "URL must use http or https.",
                          ErrorType.ValidationError));
        }

        // Store the normalized form, not the raw input.
        return Result<Url>.Success(new Url(uri.AbsoluteUri));
    }

    /// <summary>
    /// Returns the normalized URL as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="Url"/> to its normalized string value.
    /// </summary>
    public static implicit operator string(Url url) => url.Value;
}
