using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Patterns.Results;
using System.Text.Json;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="ValueObjectJsonConverter{T}"/> for <see cref="Size"/> that reads and
/// writes the canonical form <c>{ "value": 5, "unit": "Centimeter" }</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Read:</b> Accepts a JSON object with <c>value</c> (number) and <c>unit</c> (string)
/// properties. The unit is parsed case-insensitively against <see cref="SizeUnit"/>.
/// </para>
/// <para>
/// <b>Write:</b> Emits the same object shape, with the unit as its full name
/// (e.g. <c>"Centimeter"</c>, not <c>"cm"</c>).
/// </para>
/// </remarks>
public sealed class SizeJsonConverter : ValueObjectJsonConverter<Size>
{
    /// <inheritdoc />
    protected override Result<Size> Parse(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return Result<Size>.Failure(
                new Error(
                    "Kernel.Json.Size.INVALID_TYPE",
                    "Size must be a JSON object with 'value' and 'unit' properties.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("value", out JsonElement valueElement)
            || !valueElement.TryGetDecimal(out decimal value))
        {
            return Result<Size>.Failure(
                new Error(
                    "Kernel.Json.Size.INVALID_VALUE",
                    "Size.value is missing or not a number.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("unit", out JsonElement unitElement)
            || unitElement.ValueKind != JsonValueKind.String)
        {
            return Result<Size>.Failure(
                new Error(
                    "Kernel.Json.Size.INVALID_UNIT",
                    "Size.unit is missing or not a string.",
                    ErrorType.ValidationError));
        }

        string unitText = unitElement.GetString() ?? string.Empty;

        if (!Enum.TryParse(unitText, ignoreCase: true, out SizeUnit unit) || !Enum.IsDefined(unit))
        {
            return Result<Size>.Failure(
                new Error(
                    "Kernel.Json.Size.INVALID_UNIT",
                    $"Size.unit '{unitText}' is not recognized.",
                    ErrorType.ValidationError));
        }

        return Size.Create(value, unit);
    }

    /// <inheritdoc />
    protected override void Format(Utf8JsonWriter writer, Size value)
    {
        writer.WriteStartObject();
        writer.WriteNumber("value", value.Value);
        writer.WriteString("unit", value.Unit.ToString());
        writer.WriteEndObject();
    }
}
