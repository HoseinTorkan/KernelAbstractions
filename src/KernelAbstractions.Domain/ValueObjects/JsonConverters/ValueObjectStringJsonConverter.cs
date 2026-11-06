using KernelAbstractions.Patterns.Results;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A generic <see cref="JsonConverter{T}"/> for value objects that appear as plain
/// JSON strings on the wire.
/// </summary>
/// <typeparam name="T">The value object type. Must be a value type (struct).</typeparam>
/// <remarks>
/// <para>
/// Concrete converters only need to derive from this class and supply the
/// <c>Create</c> factory:
/// </para>
/// <code>
/// public sealed class PhoneNumberJsonConverter : ValueObjectStringJsonConverter&lt;PhoneNumber&gt;
/// {
///     protected override Result&lt;PhoneNumber&gt; Create(string value) => PhoneNumber.Create(value);
/// }
/// </code>
/// <para>
/// On the wire, the value object appears as a plain JSON string, e.g.
/// <c>"09121234567"</c> or <c>"user@example.com"</c>. There is no wrapper object.
/// </para>
/// <para>
/// <b>Performance:</b> Unlike <see cref="ValueObjectJsonConverter{T}"/>, this base
/// reads the string directly from <see cref="Utf8JsonReader"/> without materializing a
/// <see cref="JsonDocument"/>. This is the common case for single-value objects, so the
/// allocation saved matters.
/// </para>
/// <para>
/// <b>Validation:</b> Invalid input produces a <see cref="JsonException"/> carrying the
/// domain error code and message. The value object itself is never constructed
/// bypassing <c>Create</c>.
/// </para>
/// </remarks>
public abstract class ValueObjectStringJsonConverter<T> : JsonConverter<T>
    where T : struct
{
    /// <summary>
    /// Creates the value object from a string.
    /// </summary>
    /// <param name="value">The raw string input.</param>
    /// <returns>
    /// A successful result carrying the value object, or a failure carrying the
    /// domain error to report.
    /// </returns>
    protected abstract Result<T> Create(string value);

    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"{typeof(T).Name} must be a JSON string.");
        }

        string raw = reader.GetString() ?? string.Empty;
        Result<T> result = Create(raw);

        return result.IsFailure
            ? throw new JsonException($"{result.Error.Code}: {result.Error.Text}")
            : result.Value;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
