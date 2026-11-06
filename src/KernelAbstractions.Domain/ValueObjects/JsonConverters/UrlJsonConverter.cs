using KernelAbstractions.Patterns.Results;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> that binds a JSON
/// string to a <see cref="Url"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, a URL appears as a plain JSON string such as
/// <c>"https://example.com/path"</c>. Validation goes through
/// <see cref="Url.Create(string)"/>.
/// </remarks>
public sealed class UrlJsonConverter : ValueObjectStringJsonConverter<Url>
{
    /// <inheritdoc />
    protected override Result<Url> Create(string value) => Url.Create(value);
}
