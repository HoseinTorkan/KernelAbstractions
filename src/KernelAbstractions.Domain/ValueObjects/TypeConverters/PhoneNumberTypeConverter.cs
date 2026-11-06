using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string to a
/// <see cref="PhoneNumber"/>.
/// </summary>
public sealed class PhoneNumberTypeConverter : ValueObjectTypeConverter<PhoneNumber>
{
    /// <inheritdoc />
    protected override Result<PhoneNumber> Create(string value) => PhoneNumber.Create(value);
}
