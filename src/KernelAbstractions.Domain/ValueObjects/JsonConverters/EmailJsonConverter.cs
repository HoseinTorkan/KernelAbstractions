using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> that binds a JSON
/// string to an <see cref="Email"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, an email appears as a plain JSON string such as
/// <c>"user@example.com"</c>. Validation goes through <see cref="Email.Create(string)"/>.
/// </remarks>
public sealed class EmailJsonConverter : ValueObjectStringJsonConverter<Email>
{
    /// <inheritdoc />
    protected override Result<Email> Create(string value) => Email.Create(value);
}
