using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> that binds a JSON
/// string to a <see cref="NationalCode"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, a national code appears as a plain JSON string such as
/// <c>"0012345678"</c>. Validation goes through
/// <see cref="NationalCode.Create(string)"/>.
/// </remarks>
public sealed class NationalCodeJsonConverter : ValueObjectStringJsonConverter<NationalCode>
{
    /// <inheritdoc />
    protected override Result<NationalCode> Create(string value) => NationalCode.Create(value);
}
