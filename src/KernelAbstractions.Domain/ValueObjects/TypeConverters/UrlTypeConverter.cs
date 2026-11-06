using KernelAbstractions.Patterns.Results;
using System.ComponentModel;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="TypeConverter"/> that binds a string to a
/// <see cref="Url"/>.
/// </summary>
public sealed class UrlTypeConverter : ValueObjectTypeConverter<Url>
{
    /// <inheritdoc />
    protected override Result<Url> Create(string value) => Url.Create(value);
}
