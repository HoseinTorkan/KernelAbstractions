using KernelAbstractions.Patterns.Results;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A generic <see cref="JsonConverter{T}"/> base class for KernelAbstractions value
/// objects whose JSON shape is <b>not</b> a plain string (e.g. an object with multiple
/// properties). Each derived converter implements <see cref="Parse"/> and
/// <see cref="Format"/> to define its own JSON shape.
/// </summary>
/// <typeparam name="T">The value object type. Must be a value type (struct).</typeparam>
/// <remarks>
/// <para>
/// The base class handles the mechanics that are identical for every non-string value
/// object: reading a <see cref="JsonElement"/>, converting a domain <see cref="Error"/>
/// into a <see cref="JsonException"/>, and writing through a
/// <see cref="Utf8JsonWriter"/>.
/// </para>
/// <para>
/// <b>String-shaped value objects</b> (e.g. <c>PhoneNumber</c>, <c>Email</c>) should
/// derive from <see cref="ValueObjectStringJsonConverter{T}"/> instead, which avoids
/// the cost of materializing a <see cref="JsonDocument"/> for every read.
/// </para>
/// </remarks>
public abstract class ValueObjectJsonConverter<T> : JsonConverter<T>
    where T : struct
{
    /// <summary>
    /// Parses a JSON element into the value object.
    /// </summary>
    /// <param name="element">The raw JSON element.</param>
    /// <returns>
    /// A successful result carrying the value object, or a failure carrying the
    /// domain error to report.
    /// </returns>
    protected abstract Result<T> Parse(JsonElement element);

    /// <summary>
    /// Writes the value object to the given writer.
    /// </summary>
    /// <param name="writer">The target writer.</param>
    /// <param name="value">The value object to write.</param>
    protected abstract void Format(Utf8JsonWriter writer, T value);

    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        Result<T> result = Parse(doc.RootElement);

        return result.IsFailure
            ? throw new JsonException($"{result.Error.Code}: {result.Error.Text}")
            : result.Value;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Format(writer, value);
    }
}
