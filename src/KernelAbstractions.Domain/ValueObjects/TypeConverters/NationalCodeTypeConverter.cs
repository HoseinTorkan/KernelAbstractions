using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string to a
/// <see cref="NationalCode"/>.
/// </summary>
public sealed class NationalCodeTypeConverter : ValueObjectTypeConverter<NationalCode>
{
    /// <inheritdoc />
    protected override Result<NationalCode> Create(string value) => NationalCode.Create(value);
}
