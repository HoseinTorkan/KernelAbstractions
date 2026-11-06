using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="TypeConverter"/> that binds a string of the form
/// <c>"&lt;value&gt; &lt;unit&gt;"</c> to a <see cref="Size"/> value object.
/// </summary>
/// <remarks>
/// <para>
/// <b>Format:</b> <c>"&lt;value&gt; &lt;unit&gt;"</c>, where:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <c>value</c> is a decimal number in the invariant culture (dot as decimal
///     separator, no thousands separator).
///   </description></item>
///   <item><description>
///     <c>unit</c> is a <see cref="SizeUnit"/> name or short form (case-insensitive):
///     <c>Millimeter</c>/<c>mm</c>, <c>Centimeter</c>/<c>cm</c>, <c>Meter</c>/<c>m</c>,
///     <c>Inch</c>/<c>in</c>, <c>Foot</c>/<c>ft</c>.
///   </description></item>
///   <item><description>Whitespace is required between the two parts; multiple spaces are tolerated.</description></item>
/// </list>
/// <para>
/// <b>Examples:</b> <c>"100 cm"</c>, <c>"5.5 in"</c>, <c>"2 m"</c>, <c>"10 Millimeter"</c>.
/// </para>
/// <para>
/// <b>Invalid input</b> is reported by throwing a <see cref="FormatException"/> whose
/// message contains a JSON-serialized domain <see cref="Error"/> (PascalCase, matching
/// <c>KernelModelStateFilter</c>'s contract).
/// </para>
/// </remarks>
public sealed class SizeTypeConverter : TypeConverter
{
    private static readonly JsonSerializerOptions ErrorJsonOptions = new()
    {
        PropertyNamingPolicy = null,           // PascalCase
        PropertyNameCaseInsensitive = false,
        WriteIndented = false,
    };

    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string);

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            string[] parts = stringValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw BuildFormatException(
                    "Kernel.ValueObjects.Size.INVALID_FORMAT",
                    "Size must be formatted as '<value> <unit>', e.g. '100 cm'.");
            }

            if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                throw BuildFormatException(
                    "Kernel.ValueObjects.Size.INVALID_FORMAT",
                    "Size value is not a valid number.");
            }

            if (!TryParseUnit(parts[1], out SizeUnit unit))
            {
                throw BuildFormatException(
                    "Kernel.ValueObjects.Size.INVALID_UNIT",
                    $"Size unit '{parts[1]}' is not recognized. " +
                    "Use mm, cm, m, in, ft (or Millimeter, Centimeter, Meter, Inch, Foot).");
            }

            Result<Size> result = Size.Create(amount, unit);

            if (result.IsFailure)
            {
                throw BuildFormatException(result.Error);
            }

            return result.Value;
        }

        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Parses a unit name, accepting both full names and short forms.
    /// </summary>
    /// <param name="text">The unit token from the input string.</param>
    /// <param name="unit">The parsed unit when successful; otherwise <c>default</c>.</param>
    /// <returns><see langword="true"/> when a recognized unit was parsed.</returns>
    private static bool TryParseUnit(string text, out SizeUnit unit)
    {
        // Short forms first (case-insensitive).
        switch (text.ToLowerInvariant())
        {
            case "mm": unit = SizeUnit.Millimeter; return true;
            case "cm": unit = SizeUnit.Centimeter; return true;
            case "m": unit = SizeUnit.Meter; return true;
            case "in": unit = SizeUnit.Inch; return true;
            case "ft": unit = SizeUnit.Foot; return true;
        }

        // Full names (case-insensitive).
        if (Enum.TryParse(text, ignoreCase: true, out unit) && Enum.IsDefined(unit))
        {
            return true;
        }

        unit = default;
        return false;
    }

    private static FormatException BuildFormatException(string code, string text)
        => BuildFormatException(new Error(code, text, ErrorType.ValidationError));

    private static FormatException BuildFormatException(Error error)
        => new(JsonSerializer.Serialize(error, ErrorJsonOptions));
}
