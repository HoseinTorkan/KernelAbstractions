using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Patterns.Results;
using System.Globalization;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string of the form
/// <c>"&lt;value&gt; &lt;unit&gt;"</c> to a <see cref="DataSize"/> value object.
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see cref="ValueObjectTypeConverter{T}"/> to reuse its
/// <see cref="System.Text.Json.JsonSerializerOptions"/> instance and its
/// <see cref="FormatException"/> construction contract.
/// </para>
/// <para>
/// <b>Format:</b> <c>"&lt;value&gt; &lt;unit&gt;"</c>, where:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <c>value</c> is a decimal number in the invariant culture.
///   </description></item>
///   <item><description>
///     <c>unit</c> is a <see cref="DataSizeUnit"/> name or short form (case-insensitive):
///     <c>Byte</c>/<c>B</c>, <c>Kilobyte</c>/<c>KB</c>, <c>Megabyte</c>/<c>MB</c>,
///     <c>Gigabyte</c>/<c>GB</c>, <c>Terabyte</c>/<c>TB</c>.
///   </description></item>
///   <item><description>Whitespace is required between the two parts.</description></item>
/// </list>
/// <para>
/// <b>Examples:</b> <c>"1.5 GB"</c>, <c>"500 MB"</c>, <c>"0 Byte"</c>, <c>"2 TB"</c>.
/// </para>
/// </remarks>
public sealed class DataSizeTypeConverter : ValueObjectTypeConverter<DataSize>
{
    /// <inheritdoc />
    protected override Result<DataSize> Create(string value)
    {
        string[] parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.INVALID_FORMAT",
                          "Data size must be formatted as '<value> <unit>', e.g. '1.5 GB'.",
                          ErrorType.ValidationError));
        }

        if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.INVALID_FORMAT",
                          "Data size value is not a valid number.",
                          ErrorType.ValidationError));
        }

        if (!TryParseUnit(parts[1], out DataSizeUnit unit))
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.INVALID_UNIT",
                          $"Data size unit '{parts[1]}' is not recognized. " +
                          "Use B, KB, MB, GB, TB (or Byte, Kilobyte, Megabyte, Gigabyte, Terabyte).",
                          ErrorType.ValidationError));
        }

        return DataSize.Create(amount, unit);
    }

    /// <summary>
    /// Parses a unit name, accepting both full names and short forms.
    /// </summary>
    /// <param name="text">The unit token from the input string.</param>
    /// <param name="unit">The parsed unit when successful; otherwise <c>default</c>.</param>
    /// <returns><see langword="true"/> when a recognized unit was parsed.</returns>
    private static bool TryParseUnit(string text, out DataSizeUnit unit)
    {
        // Short forms first (case-insensitive).
        switch (text.ToUpperInvariant())
        {
            case "B": unit = DataSizeUnit.Byte; return true;
            case "KB": unit = DataSizeUnit.Kilobyte; return true;
            case "MB": unit = DataSizeUnit.Megabyte; return true;
            case "GB": unit = DataSizeUnit.Gigabyte; return true;
            case "TB": unit = DataSizeUnit.Terabyte; return true;
        }

        // Full names (case-insensitive).
        if (Enum.TryParse(text, ignoreCase: true, out unit) && Enum.IsDefined(unit))
        {
            return true;
        }

        unit = default;
        return false;
    }
}
