using KernelAbstractions.Patterns.Results;
using System.ComponentModel;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="TypeConverter"/> that binds a string to a
/// <see cref="PersonName"/>.
/// </summary>
public sealed class PersonNameTypeConverter : ValueObjectTypeConverter<PersonName>
{
    /// <inheritdoc />
    protected override Result<PersonName> Create(string value) => PersonName.Create(value);
}
