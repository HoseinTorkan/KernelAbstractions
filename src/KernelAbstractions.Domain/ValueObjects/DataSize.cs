using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a quantity of digital information (storage capacity, file size,
/// memory usage) with a unit, convertible between bytes, KB, MB, GB, and TB.
/// </summary>
/// <remarks>
/// <para>
/// This value object guarantees the following invariants:
/// </para>
/// <list type="bullet">
///   <item><description>The <see cref="Value"/> is always non-negative (zero is allowed).</description></item>
///   <item><description>
///     The <see cref="Value"/> never exceeds <c>1,000,000</c> in the unit supplied to
///     <see cref="Create"/>. Conversions may produce values above this bound, because
///     the bound applies to the input, not to derived values.
///   </description></item>
///   <item><description>
///     The <see cref="Value"/> is always rounded to two decimal places using
///     <see cref="MidpointRounding.AwayFromZero"/>.
///   </description></item>
/// </list>
/// <para>
/// <b>Binary units:</b> This type uses base-1024 prefixes (1 KB = 1024 B), matching
/// what operating systems and file managers display. See <see cref="DataSizeUnit"/>
/// for the rationale.
/// </para>
/// <para>
/// <b>Zero is valid</b> because empty files and unused storage are meaningful values.
/// Use <see cref="Create"/> with any non-negative amount.
/// </para>
/// </remarks>
[JsonConverter(typeof(DataSizeJsonConverter))]
[TypeConverter(typeof(DataSizeTypeConverter))]
public readonly record struct DataSize
{
    /// <summary>
    /// The maximum permitted value in the unit supplied to <see cref="Create"/>.
    /// </summary>
    private const decimal MaxValue = 1_000_000m;

    /// <summary>
    /// The number of decimal places retained after rounding.
    /// </summary>
    private const int Precision = 2;

    /// <summary>
    /// Gets the numeric value, rounded to two decimal places.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Gets the unit of measurement.
    /// </summary>
    public DataSizeUnit Unit { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSize"/> struct.
    /// </summary>
    /// <param name="value">The numeric value. The caller must ensure it is valid.</param>
    /// <param name="unit">The unit of measurement.</param>
    private DataSize(decimal value, DataSizeUnit unit)
    {
        Value = Math.Round(value, Precision, MidpointRounding.AwayFromZero);
        Unit = unit;
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> from a numeric value and a unit.
    /// </summary>
    /// <param name="value">
    /// The numeric value. Must be non-negative and no greater than <c>1,000,000</c>.
    /// </param>
    /// <param name="unit">The unit of measurement. Must be a defined <see cref="DataSizeUnit"/>.</param>
    /// <returns>
    /// A successful result carrying the new <see cref="DataSize"/>, or a failure with one of:
    /// <list type="bullet">
    ///   <item><description><c>Kernel.ValueObjects.DataSize.INVALID_VALUE</c> when <paramref name="value"/> is negative.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.DataSize.TOO_LARGE</c> when <paramref name="value"/> exceeds <c>1,000,000</c>.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.DataSize.INVALID_UNIT</c> when <paramref name="unit"/> is not a defined value.</description></item>
    /// </list>
    /// </returns>
    public static Result<DataSize> Create(decimal value, DataSizeUnit unit)
    {
        if (value < 0)
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.INVALID_VALUE",
                          "Data size cannot be negative.",
                          ErrorType.ValidationError));
        }

        if (value > MaxValue)
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.TOO_LARGE",
                          $"Data size value must not exceed {MaxValue}.",
                          ErrorType.ValidationError));
        }

        if (!Enum.IsDefined(unit))
        {
            return Result<DataSize>.Failure(
                new Error("Kernel.ValueObjects.DataSize.INVALID_UNIT",
                          "Data size unit is not defined.",
                          ErrorType.ValidationError));
        }

        return Result<DataSize>.Success(new DataSize(value, unit));
    }

    /// <summary>
    /// Creates a zero amount in the given unit.
    /// </summary>
    /// <param name="unit">The unit of measurement.</param>
    public static Result<DataSize> Zero(DataSizeUnit unit) => Create(0m, unit);

    /// <summary>
    /// Converts this data size to bytes.
    /// </summary>
    public DataSize ToByte() => Convert(DataSizeUnit.Byte);

    /// <summary>
    /// Converts this data size to kilobytes.
    /// </summary>
    public DataSize ToKilobyte() => Convert(DataSizeUnit.Kilobyte);

    /// <summary>
    /// Converts this data size to megabytes.
    /// </summary>
    public DataSize ToMegabyte() => Convert(DataSizeUnit.Megabyte);

    /// <summary>
    /// Converts this data size to gigabytes.
    /// </summary>
    public DataSize ToGigabyte() => Convert(DataSizeUnit.Gigabyte);

    /// <summary>
    /// Converts this data size to terabytes.
    /// </summary>
    public DataSize ToTerabyte() => Convert(DataSizeUnit.Terabyte);

    /// <summary>
    /// Converts this data size to the given target unit.
    /// </summary>
    /// <param name="target">The target unit.</param>
    /// <returns>
    /// A new <see cref="DataSize"/> expressed in <paramref name="target"/>, or <c>this</c>
    /// if already in that unit.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="target"/> is not a supported unit.
    /// </exception>
    private DataSize Convert(DataSizeUnit target)
    {
        if (Unit == target)
        {
            return this;
        }

        decimal inBytes = Value * ToByteFactor(Unit);
        decimal result = inBytes / ToByteFactor(target);

        return new DataSize(result, target);
    }

    /// <summary>
    /// Returns the multiplicative factor that converts one unit into bytes.
    /// </summary>
    /// <param name="unit">The source unit.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="unit"/> is not a supported unit.
    /// </exception>
    private static decimal ToByteFactor(DataSizeUnit unit) => unit switch
    {
        DataSizeUnit.Byte => 1m,
        DataSizeUnit.Kilobyte => 1024m,
        DataSizeUnit.Megabyte => 1_048_576m,           // 1024^2
        DataSizeUnit.Gigabyte => 1_073_741_824m,       // 1024^3
        DataSizeUnit.Terabyte => 1_099_511_627_776m,   // 1024^4
        _ => throw new InvalidOperationException($"Unsupported data size unit: {unit}"),
    };

    /// <summary>
    /// Returns a human-readable representation such as <c>1.5 Gigabyte</c>.
    /// </summary>
    public override string ToString() => $"{Value} {Unit}";

    /// <summary>
    /// Implicitly converts a <see cref="DataSize"/> to its string representation.
    /// </summary>
    public static implicit operator string(DataSize size) => size.ToString();
}
