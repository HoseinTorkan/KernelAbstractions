using KernelAbstractions.Domain.Enums;
using KernelAbstractions.Patterns.Results;
using System.Text.Json;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="ValueObjectJsonConverter{T}"/> for <see cref="DataSize"/> that reads and
/// writes the canonical form <c>{ "value": 1.5, "unit": "Gigabyte" }</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Read:</b> Accepts a JSON object with <c>value</c> (number) and <c>unit</c> (string)
/// properties. The unit is parsed case-insensitively against <see cref="DataSizeUnit"/>.
/// </para>
/// <para>
/// <b>Write:</b> Emits the same object shape, with the unit as its full name.
/// </para>
/// </remarks>
public sealed class DataSizeJsonConverter : ValueObjectJsonConverter<DataSize>
{
    /// <inheritdoc />
    protected override Result<DataSize> Parse(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return Result<DataSize>.Failure(
                new Error(
                    "Kernel.Json.DataSize.INVALID_TYPE",
                    "DataSize must be a JSON object with 'value' and 'unit' properties.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("value", out JsonElement valueElement)
            || !valueElement.TryGetDecimal(out decimal value))
        {
            return Result<DataSize>.Failure(
                new Error(
                    "Kernel.Json.DataSize.INVALID_VALUE",
                    "DataSize.value is missing or not a number.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("unit", out JsonElement unitElement)
            || unitElement.ValueKind != JsonValueKind.String)
        {
            return Result<DataSize>.Failure(
                new Error(
                    "Kernel.Json.DataSize.INVALID_UNIT",
                    "DataSize.unit is missing or not a string.",
                    ErrorType.ValidationError));
        }

        string unitText = unitElement.GetString() ?? string.Empty;

        if (!Enum.TryParse(unitText, ignoreCase: true, out DataSizeUnit unit) || !Enum.IsDefined(unit))
        {
            return Result<DataSize>.Failure(
                new Error(
                    "Kernel.Json.DataSize.INVALID_UNIT",
                    $"DataSize.unit '{unitText}' is not recognized.",
                    ErrorType.ValidationError));
        }

        return DataSize.Create(value, unit);
    }

    /// <inheritdoc />
    protected override void Format(Utf8JsonWriter writer, DataSize value)
    {
        writer.WriteStartObject();
        writer.WriteNumber("value", value.Value);
        writer.WriteString("unit", value.Unit.ToString());
        writer.WriteEndObject();
    }
}
