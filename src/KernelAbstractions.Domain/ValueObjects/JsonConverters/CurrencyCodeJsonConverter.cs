using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> that binds a JSON
/// string to a <see cref="CurrencyCode"/> and writes it back as a string.
/// </summary>
/// <remarks>
/// On the wire, a currency code appears as a plain JSON string such as <c>"IRR"</c>.
/// Validation goes through <see cref="CurrencyCode.Create(string)"/>.
/// </remarks>
public sealed class CurrencyCodeJsonConverter : ValueObjectStringJsonConverter<CurrencyCode>
{
    /// <inheritdoc />
    protected override Result<CurrencyCode> Create(string value) => CurrencyCode.Create(value);
}
