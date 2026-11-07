using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a valid human name (first name, last name, or full name).
/// </summary>
/// <remarks>
/// <para>
/// This value object is intended <b>only for human names</b>. It accepts Unicode letters,
/// the zero-width non-joiner (<c>U+200C</c>, used in Persian names such as
/// <c>فاطمه‌زهرا</c>), apostrophes, and hyphens. Digits and other symbols are rejected.
/// </para>
/// <para>
/// For non-person names (product, category, brand, etc.), do <b>not</b> use this type.
/// Those concepts have their own validation rules that differ by domain; model them
/// explicitly when the need arises.
/// </para>
/// </remarks>
[JsonConverter(typeof(PersonNameJsonConverter))]
[TypeConverter(typeof(PersonNameTypeConverter))]
public readonly partial record struct PersonName
{
    /// <summary>
    /// Matches a sequence of Unicode letters (with optional apostrophes, hyphens, and
    /// zero-width non-joiners), separated by single whitespace or zero-width non-joiner
    /// characters.
    /// </summary>
    [GeneratedRegex(@"^[\p{L}\u200C'\-]+([\s\u200C][\p{L}\u200C'\-]+)*$")]
    private static partial Regex AllowedPattern();

    /// <summary>
    /// Gets the trimmed name value.
    /// </summary>
    public string Value { get; }

    private PersonName(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="PersonName"/> from the given string.
    /// </summary>
    /// <param name="value">The raw name string.</param>
    /// <returns>
    /// A successful result carrying the trimmed name, or a failure when the input is
    /// empty, too short, too long, or contains disallowed characters.
    /// </returns>
    public static Result<PersonName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PersonName>.Failure(
                new Error("Kernel.ValueObjects.PersonName.EMPTY",
                          "Person name cannot be empty.",
                          ErrorType.ValidationError));
        }

        string trimmed = value.Trim();

        if (trimmed.Length < 2)
        {
            return Result<PersonName>.Failure(
                new Error("Kernel.ValueObjects.PersonName.TOO_SHORT",
                          "Person name is too short.",
                          ErrorType.ValidationError));
        }

        if (trimmed.Length > 100)
        {
            return Result<PersonName>.Failure(
                new Error("Kernel.ValueObjects.PersonName.TOO_LONG",
                          "Person name is too long.",
                          ErrorType.ValidationError));
        }

        if (!AllowedPattern().IsMatch(trimmed))
        {
            return Result<PersonName>.Failure(
                new Error("Kernel.ValueObjects.PersonName.INVALID_CHARACTERS",
                          "Person name contains invalid characters.",
                          ErrorType.ValidationError));
        }

        return Result<PersonName>.Success(new PersonName(trimmed));
    }

    /// <summary>
    /// Returns the name as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="PersonName"/> to its string value.
    /// </summary>
    public static implicit operator string(PersonName name) => name.Value;
}
