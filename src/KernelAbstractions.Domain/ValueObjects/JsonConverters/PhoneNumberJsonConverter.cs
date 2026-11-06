using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> that binds a JSON
/// string to a <see cref="PhoneNumber"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, a phone number appears as a plain JSON string such as
/// <c>"09121234567"</c>. Validation goes through <see cref="PhoneNumber.Create(string)"/>.
/// </remarks>
public sealed class PhoneNumberJsonConverter : ValueObjectStringJsonConverter<PhoneNumber>
{
    /// <inheritdoc />
    protected override Result<PhoneNumber> Create(string value) => PhoneNumber.Create(value);
}
