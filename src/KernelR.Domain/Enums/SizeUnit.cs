namespace KernelR.Domain.Enums;

/// <summary>
/// Units of measurement supported by <see cref="ValueObjects.Size"/>.
/// </summary>
/// <remarks>
/// <para>
/// Metric units (<see cref="Millimeter"/>, <see cref="Centimeter"/>, <see cref="Meter"/>)
/// cover dimensions from small components to large surfaces. Imperial units
/// (<see cref="Inch"/>, <see cref="Foot"/>) cover screen sizes, pipes, and furniture.
/// </para>
/// <para>
/// Additional units can be added later, but each addition requires extending
/// <c>Size.ToMillimeterFactor</c> and, optionally, adding a dedicated conversion method.
/// </para>
/// </remarks>
public enum SizeUnit
{
    /// <summary>Millimeters (mm).</summary>
    Millimeter = 1,

    /// <summary>Centimeters (cm).</summary>
    Centimeter = 2,

    /// <summary>Meters (m).</summary>
    Meter = 3,

    /// <summary>Inches (in).</summary>
    Inch = 4,

    /// <summary>Feet (ft).</summary>
    Foot = 5,
}
