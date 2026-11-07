using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects;

/// <summary>
/// Represents a physical size (length, width, height, or diameter) with a unit,
/// convertible between millimeters, centimeters, meters, inches, and feet.
/// </summary>
/// <remarks>
/// <para>
/// This value object guarantees the following invariants:
/// </para>
/// <list type="bullet">
///   <item><description>The <see cref="Value"/> is always strictly positive.</description></item>
///   <item><description>
///     The <see cref="Value"/> never exceeds <c>10,000</c> in the unit supplied to
///     <see cref="Create"/>. Conversions may produce values above this bound, because
///     the bound applies to the input, not to derived values.
///   </description></item>
///   <item><description>
///     The <see cref="Value"/> is always rounded to two decimal places using
///     <see cref="MidpointRounding.AwayFromZero"/>.
///   </description></item>
/// </list>
/// <para>
/// <b>Not for apparel:</b> This type models a <b>measurable quantity</b>. It is not
/// suitable for clothing or shoe sizes, which are labels rather than measurements.
/// </para>
/// </remarks>
[JsonConverter(typeof(SizeJsonConverter))]
[TypeConverter(typeof(SizeTypeConverter))]
public readonly record struct Size
{
    private const decimal MaxValue = 10_000m;
    private const int Precision = 2;

    /// <summary>
    /// Gets the numeric value, rounded to two decimal places.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Gets the unit of measurement.
    /// </summary>
    public SizeUnit Unit { get; }

    private Size(decimal value, SizeUnit unit)
    {
        Value = Math.Round(value, Precision, MidpointRounding.AwayFromZero);
        Unit = unit;
    }

    /// <summary>
    /// Creates a new <see cref="Size"/> from a numeric value and a unit.
    /// </summary>
    /// <param name="value">The numeric value. Must be strictly positive and no greater than <c>10,000</c>.</param>
    /// <param name="unit">The unit of measurement. Must be a defined <see cref="SizeUnit"/>.</param>
    /// <returns>
    /// A successful result carrying the new <see cref="Size"/>, or a failure with one of:
    /// <list type="bullet">
    ///   <item><description><c>Kernel.ValueObjects.Size.INVALID_VALUE</c> when <paramref name="value"/> is not positive.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.Size.TOO_LARGE</c> when <paramref name="value"/> exceeds <c>10,000</c>.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.Size.INVALID_UNIT</c> when <paramref name="unit"/> is not a defined value.</description></item>
    /// </list>
    /// </returns>
    public static Result<Size> Create(decimal value, SizeUnit unit)
    {
        if (value <= 0)
        {
            return Result<Size>.Failure(
                new Error("Kernel.ValueObjects.Size.INVALID_VALUE",
                          "Size value must be greater than zero.",
                          ErrorType.ValidationError));
        }

        if (value > MaxValue)
        {
            return Result<Size>.Failure(
                new Error("Kernel.ValueObjects.Size.TOO_LARGE",
                          $"Size value must not exceed {MaxValue}.",
                          ErrorType.ValidationError));
        }

        if (!Enum.IsDefined(unit))
        {
            return Result<Size>.Failure(
                new Error("Kernel.ValueObjects.Size.INVALID_UNIT",
                          "Size unit is not defined.",
                          ErrorType.ValidationError));
        }

        return Result<Size>.Success(new Size(value, unit));
    }

    /// <summary>
    /// Converts this size to millimeters.
    /// </summary>
    public Size ToMillimeter() => Convert(SizeUnit.Millimeter);

    /// <summary>
    /// Converts this size to centimeters.
    /// </summary>
    public Size ToCentimeter() => Convert(SizeUnit.Centimeter);

    /// <summary>
    /// Converts this size to meters.
    /// </summary>
    public Size ToMeter() => Convert(SizeUnit.Meter);

    /// <summary>
    /// Converts this size to inches.
    /// </summary>
    public Size ToInch() => Convert(SizeUnit.Inch);

    /// <summary>
    /// Converts this size to feet.
    /// </summary>
    public Size ToFoot() => Convert(SizeUnit.Foot);

    /// <summary>
    /// Converts this size to the given target unit.
    /// </summary>
    /// <param name="target">The target unit.</param>
    /// <returns>
    /// A new <see cref="Size"/> expressed in <paramref name="target"/>, or <c>this</c>
    /// if already in that unit.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="target"/> is not a supported unit.
    /// </exception>
    private Size Convert(SizeUnit target)
    {
        if (Unit == target)
        {
            return this;
        }

        decimal inMillimeters = Value * ToMillimeterFactor(Unit);
        decimal result = inMillimeters / ToMillimeterFactor(target);

        return new Size(result, target);
    }

    /// <summary>
    /// Returns the multiplicative factor that converts one unit into millimeters.
    /// </summary>
    /// <param name="unit">The source unit.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="unit"/> is not a supported unit.
    /// </exception>
    private static decimal ToMillimeterFactor(SizeUnit unit) => unit switch
    {
        SizeUnit.Millimeter => 1m,
        SizeUnit.Centimeter => 10m,
        SizeUnit.Meter => 1000m,
        SizeUnit.Inch => 25.4m,
        SizeUnit.Foot => 304.8m,
        _ => throw new InvalidOperationException($"Unsupported size unit: {unit}"),
    };

    /// <summary>
    /// Returns a human-readable representation such as <c>5.5 Centimeter</c>.
    /// </summary>
    public override string ToString() => $"{Value} {Unit}";

    /// <summary>
    /// Implicitly converts a <see cref="Size"/> to its string representation.
    /// </summary>
    public static implicit operator string(Size size) => size.ToString();
}
