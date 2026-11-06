using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string to a
/// <see cref="CurrencyCode"/>.
/// </summary>
public sealed class CurrencyCodeTypeConverter : ValueObjectTypeConverter<CurrencyCode>
{
    /// <inheritdoc />
    protected override Result<CurrencyCode> Create(string value) => CurrencyCode.Create(value);
}
