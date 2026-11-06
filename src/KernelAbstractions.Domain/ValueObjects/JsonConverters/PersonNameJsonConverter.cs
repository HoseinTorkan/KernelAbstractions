using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> that binds a JSON
/// string to a <see cref="PersonName"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, a person name appears as a plain JSON string such as <c>"علی"</c>.
/// Validation goes through <see cref="PersonName.Create(string)"/>.
/// </remarks>
public sealed class PersonNameJsonConverter : ValueObjectStringJsonConverter<PersonName>
{
    /// <inheritdoc />
    protected override Result<PersonName> Create(string value) => PersonName.Create(value);
}
