using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a valid Iranian mobile phone number.
/// </summary>
/// <remarks>
/// <para>
/// This value object guarantees that the underlying value is a normalized
/// Iranian mobile number in the format <c>09XXXXXXXXX</c>.
/// </para>
/// <para>
/// Input is normalized before validation:
/// </para>
/// <list type="bullet">
///   <item><description>Whitespace, dashes, and parentheses are removed.</description></item>
///   <item><description>Country-code prefixes <c>+98</c>, <c>0098</c>, and <c>98</c> are replaced with a leading <c>0</c>.</description></item>
/// </list>
/// <para>
/// <b>Integration with ASP.NET Core:</b> This type implements <see cref="IParsable{TSelf}"/>
/// so that Minimal API endpoints can bind it directly from query strings, route values,
/// or form data without additional attributes:
/// </para>
/// <code>
/// app.MapGet("/api/phone", (PhoneNumber phone) => Results.Ok(phone.Value));
/// </code>
/// <para>
/// For JSON bodies, <see cref="PhoneNumberJsonConverter"/> is used automatically because
/// the type is decorated with <see cref="JsonConverterAttribute"/>. For MVC model binding
/// from query strings, <see cref="PhoneNumberTypeConverter"/> is used via
/// <see cref="TypeConverterAttribute"/>.
/// </para>
/// </remarks>
[JsonConverter(typeof(PhoneNumberJsonConverter))]
[TypeConverter(typeof(PhoneNumberTypeConverter))]
public readonly partial record struct PhoneNumber : IParsable<PhoneNumber>
{
    /// <summary>
    /// Matches a normalized Iranian mobile number: a leading <c>0</c>, a <c>9</c>, and nine digits.
    /// </summary>
    [GeneratedRegex(@"^09\d{9}$")]
    private static partial Regex AllowedPattern();

    /// <summary>
    /// Matches characters treated as separators during normalization.
    /// </summary>
    [GeneratedRegex(@"[\s\-\(\)]")]
    private static partial Regex Separators();

    /// <summary>
    /// Matches a country-code prefix at the start of the input.
    /// </summary>
    /// <remarks>
    /// Longer alternatives are listed first so that the engine prefers <c>+98</c> and
    /// <c>0098</c> over a bare <c>98</c>, preventing partial matches such as
    /// treating the <c>98</c> in <c>0098...</c> as a country code after separators
    /// have been removed.
    /// </remarks>
    [GeneratedRegex(@"^(\+98|0098|98)")]
    private static partial Regex CountryCodePrefix();

    /// <summary>
    /// Gets the normalized phone number in the format <c>09XXXXXXXXX</c>.
    /// </summary>
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="PhoneNumber"/> from the given string.
    /// </summary>
    /// <param name="value">
    /// The raw phone number string (e.g. <c>"+989121234567"</c>,
    /// <c>"0912-123-4567"</c>, <c>"00989121234567"</c>).
    /// </param>
    /// <returns>
    /// A successful result carrying the normalized number, or a failure
    /// with <c>Kernel.ValueObjects.PhoneNumber.EMPTY</c> when the input is empty and
    /// <c>Kernel.ValueObjects.PhoneNumber.INVALID_FORMAT</c> when the input does not match
    /// the expected pattern.
    /// </returns>
    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PhoneNumber>.Failure(
                new Error("Kernel.ValueObjects.PhoneNumber.EMPTY",
                          "Phone number cannot be empty.",
                          ErrorType.ValidationError));
        }

        // Normalize only once: the normalized value is reused for validation and storage.
        string normalized = Normalize(value);

        return !AllowedPattern().IsMatch(normalized)
            ? Result<PhoneNumber>.Failure(
                new Error("Kernel.ValueObjects.PhoneNumber.INVALID_FORMAT",
                          "Phone number format is invalid.",
                          ErrorType.ValidationError))
            : Result<PhoneNumber>.Success(new PhoneNumber(normalized));
    }

    /// <summary>
    /// Normalizes the input string by removing separators and replacing
    /// country-code prefixes with the local leading zero.
    /// </summary>
    /// <param name="value">The raw phone number string.</param>
    /// <returns>The normalized string, not yet validated.</returns>
    private static string Normalize(string value)
    {
        // Remove separators first so that the country-code prefix is adjacent to the digits.
        string withoutSeparators = Separators().Replace(value, string.Empty);

        // Replace any known country-code prefix with a single leading zero.
        return CountryCodePrefix().Replace(withoutSeparators, "0");
    }

    /// <summary>
    /// Parses a string into a <see cref="PhoneNumber"/>, throwing on failure.
    /// </summary>
    /// <param name="s">The raw phone number string to parse.</param>
    /// <param name="provider">An optional format provider; currently ignored.</param>
    /// <returns>A <see cref="PhoneNumber"/> carrying the normalized value.</returns>
    /// <exception cref="FormatException">
    /// <paramref name="s"/> is <see langword="null"/>, empty, whitespace, or does not
    /// match the expected Iranian mobile number format.
    /// </exception>
    /// <remarks>
    /// This method satisfies <see cref="IParsable{TSelf}"/> and is used by ASP.NET Core
    /// Minimal APIs to bind a <see cref="PhoneNumber"/> parameter from a query string,
    /// route value, or form field. The <paramref name="provider"/> argument is part of the
    /// contract but is not needed because phone numbers are culture-invariant.
    /// </remarks>
    public static PhoneNumber Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out PhoneNumber result))
        {
            return result;
        }

        throw new FormatException($"'{s}' is not a valid PhoneNumber.");
    }

    /// <summary>
    /// Tries to parse a string into a <see cref="PhoneNumber"/>.
    /// </summary>
    /// <param name="s">The raw phone number string to parse.</param>
    /// <param name="provider">An optional format provider; currently ignored.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, contains the normalized
    /// <see cref="PhoneNumber"/>; otherwise, <see langword="default"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="s"/> is a valid Iranian mobile number;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method satisfies <see cref="IParsable{TSelf}"/>. When it returns
    /// <see langword="false"/>, ASP.NET Core adds a binding error to
    /// <c>ModelState</c>, which is then converted to a standardized
    /// <c>ProblemDetails</c> response by <c>KernelModelStateFilter</c>.
    /// </remarks>
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out PhoneNumber result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        Result<PhoneNumber> createResult = Create(s);
        if (createResult.IsFailure)
        {
            return false;
        }

        result = createResult.Value;
        return true;
    }

    /// <summary>
    /// Returns the normalized phone number as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="PhoneNumber"/> to its normalized string value.
    /// </summary>
    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
