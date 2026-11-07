using KernelR.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a valid Iranian national code (کد ملی).
/// </summary>
/// <remarks>
/// <para>
/// The national code is a 10-digit number with a built-in checksum. Validation
/// rejects codes whose digits are all identical (e.g. <c>1111111111</c>), which are
/// formally invalid even though their checksum matches.
/// </para>
/// <para>
/// Input is normalized by removing spaces and dashes before validation.
/// </para>
/// </remarks>
[JsonConverter(typeof(NationalCodeJsonConverter))]
[TypeConverter(typeof(NationalCodeTypeConverter))]
public readonly partial record struct NationalCode
{
    /// <summary>
    /// Matches a sequence of 10 digits.
    /// </summary>
    [GeneratedRegex(@"^\d{10}$")]
    private static partial Regex AllowedPattern();

    /// <summary>
    /// Matches characters treated as separators during normalization.
    /// </summary>
    [GeneratedRegex(@"[\s\-]")]
    private static partial Regex Separators();

    /// <summary>
    /// Gets the normalized national code (10 digits).
    /// </summary>
    public string Value { get; }

    private NationalCode(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="NationalCode"/> from the given string.
    /// </summary>
    /// <param name="value">The raw national code (e.g. <c>"0012345678"</c>, <c>"001-2345678"</c>).</param>
    /// <returns>
    /// A successful result carrying the normalized code, or a failure when the input
    /// is empty, not 10 digits, all-identical, or has an invalid checksum.
    /// </returns>
    public static Result<NationalCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<NationalCode>.Failure(
                new Error("Kernel.ValueObjects.NationalCode.EMPTY",
                          "National code cannot be empty.",
                          ErrorType.ValidationError));
        }

        string normalized = Separators().Replace(value, string.Empty);

        if (!AllowedPattern().IsMatch(normalized))
        {
            return Result<NationalCode>.Failure(
                new Error("Kernel.ValueObjects.NationalCode.INVALID_FORMAT",
                          "National code must be exactly 10 digits.",
                          ErrorType.ValidationError));
        }

        // Reject codes where all digits are identical; these are formally invalid.
        if (normalized.All(c => c == normalized[0]))
        {
            return Result<NationalCode>.Failure(
                new Error("Kernel.ValueObjects.NationalCode.INVALID_CHECKSUM",
                          "National code is invalid.",
                          ErrorType.ValidationError));
        }

        // Standard Iranian national code checksum.
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (normalized[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int checkDigit = normalized[9] - '0';

        bool isValid = remainder < 2
            ? checkDigit == remainder
            : checkDigit == 11 - remainder;

        return !isValid
            ? Result<NationalCode>.Failure(
                new Error("Kernel.ValueObjects.NationalCode.INVALID_CHECKSUM",
                          "National code is invalid.",
                          ErrorType.ValidationError))
            : Result<NationalCode>.Success(new NationalCode(normalized));
    }

    /// <summary>
    /// Returns the normalized national code as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="NationalCode"/> to its normalized string value.
    /// </summary>
    public static implicit operator string(NationalCode code) => code.Value;
}
