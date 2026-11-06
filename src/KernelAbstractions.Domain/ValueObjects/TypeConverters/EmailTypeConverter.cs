using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string to an
/// <see cref="Email"/>.
/// </summary>
public sealed class EmailTypeConverter : ValueObjectTypeConverter<Email>
{
    /// <inheritdoc />
    protected override Result<Email> Create(string value) => Email.Create(value);
}
