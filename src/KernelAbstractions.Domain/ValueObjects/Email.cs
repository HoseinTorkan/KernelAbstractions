using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a valid email address.
/// </summary>
/// <remarks>
/// <para>
/// The value is normalized to <b>lowercase</b> before storage. Email addresses are
/// case-insensitive in practice for the domain part, and treating the whole address
/// as lowercase avoids duplicate registrations that differ only in case.
/// </para>
/// <para>
/// Validation uses a practical subset of RFC 5322, sufficient for the vast majority of
/// real-world addresses. It is intentionally not a full RFC implementation: fully
/// RFC-compliant addresses can be arbitrarily complex, and the only authoritative
/// validation for an email is delivery.
/// </para>
/// </remarks>
[JsonConverter(typeof(EmailJsonConverter))]
[TypeConverter(typeof(EmailTypeConverter))]
public readonly partial record struct Email
{
    /// <summary>
    /// Maximum length of an email address per RFC 5321 (SMTP path limit).
    /// </summary>
    private const int MaxLength = 254;

    /// <summary>
    /// Matches a practical subset of RFC 5322 email addresses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Local part: letters, digits, and a small set of common symbols
    /// (<c>. _ % + -</c>). Must not start or end with a dot, and must not contain
    /// consecutive dots.
    /// </para>
    /// <para>
    /// Domain part: one or more dot-separated labels of letters, digits, and hyphens.
    /// Labels must not start or end with a hyphen. The final label (TLD) must be at
    /// least two letters.
    /// </para>
    /// </remarks>
    [GeneratedRegex(
        @"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
        @"@" +
        @"([A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,}$")]
    private static partial Regex AllowedPattern();

    /// <summary>
    /// Gets the normalized (lowercase) email address.
    /// </summary>
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="Email"/> from the given string.
    /// </summary>
    /// <param name="value">The raw email address.</param>
    /// <returns>
    /// A successful result carrying the normalized email, or a failure when the input
    /// is empty or does not match the expected format.
    /// </returns>
    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure(
                new Error("Kernel.ValueObjects.Email.EMPTY",
                          "Email cannot be empty.",
                          ErrorType.ValidationError));
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
        {
            return Result<Email>.Failure(
                new Error("Kernel.ValueObjects.Email.TOO_LONG",
                          "Email is too long.",
                          ErrorType.ValidationError));
        }

        if (!AllowedPattern().IsMatch(normalized))
        {
            return Result<Email>.Failure(
                new Error("Kernel.ValueObjects.Email.INVALID_FORMAT",
                          "Email format is invalid.",
                          ErrorType.ValidationError));
        }

        return Result<Email>.Success(new Email(normalized));
    }

    /// <summary>
    /// Returns the normalized email address as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts an <see cref="Email"/> to its normalized string value.
    /// </summary>
    public static implicit operator string(Email email) => email.Value;
}
