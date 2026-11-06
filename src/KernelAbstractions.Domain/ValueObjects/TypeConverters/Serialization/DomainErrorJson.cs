using KernelAbstractions.Patterns.Results;
using System.Text.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure (intentional)
namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;
#pragma warning restore IDE0130

/// <summary>
/// The shared serialization contract for domain <see cref="Patterns.Results.Error"/>
/// payloads that cross from <c>TypeConverter</c> to the Web layer's
/// <c>KernelModelStateFilter</c>.
/// </summary>
/// <remarks>
/// <para>
/// Both the Domain-layer <c>ValueObjectTypeConverter&lt;T&gt;</c> and the Web-layer
/// <c>KernelModelStateFilter</c> use this same instance, so the two sides can never
/// drift apart.
/// </para>
/// <para>
/// <b>PascalCase</b> matches the property names on <see cref="Patterns.Results.Error"/>
/// (<c>Code</c>, <c>Text</c>, <c>ErrorType</c>).
/// </para>
/// <para>
/// <b>Independence from consumer settings:</b> Options are built explicitly and are
/// intentionally <b>not</b> <see cref="JsonSerializerOptions.Default"/>, so consumer
/// applications cannot affect this pipeline by customizing global JSON settings.
/// </para>
/// </remarks>
public static class DomainErrorJson
{
    /// <summary>
    /// The serializer options used by all KernelAbstractions packages when transferring
    /// <see cref="Error"/> payloads across layer boundaries.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = null,           // PascalCase
        PropertyNameCaseInsensitive = false,
        WriteIndented = false,
    };
}
