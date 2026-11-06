using KernelAbstractions.Patterns.Results;
using System.Text.Json;

namespace KernelAbstractions.Domain.ValueObjects.JsonConverters;

/// <summary>
/// A <see cref="ValueObjectJsonConverter{T}"/> for <see cref="Money"/> that reads and
/// writes the canonical form <c>{ "amount": 100, "currency": "IRR" }</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Read:</b> Accepts a JSON object with <c>amount</c> (number) and <c>currency</c>
/// (string) properties. Validation goes through
/// <see cref="Money.Create(decimal, string)"/>.
/// </para>
/// <para>
/// <b>Write:</b> Emits the same object shape.
/// </para>
/// </remarks>
public sealed class MoneyJsonConverter : ValueObjectJsonConverter<Money>
{
    /// <inheritdoc />
    protected override Result<Money> Parse(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return Result<Money>.Failure(
                new Error(
                    "Kernel.Json.Money.INVALID_TYPE",
                    "Money must be a JSON object with 'amount' and 'currency' properties.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("amount", out JsonElement amountElement)
            || !amountElement.TryGetDecimal(out decimal amount))
        {
            return Result<Money>.Failure(
                new Error(
                    "Kernel.Json.Money.INVALID_AMOUNT",
                    "Money.amount is missing or not a number.",
                    ErrorType.ValidationError));
        }

        if (!element.TryGetProperty("currency", out JsonElement currencyElement)
            || currencyElement.ValueKind != JsonValueKind.String)
        {
            return Result<Money>.Failure(
                new Error(
                    "Kernel.Json.Money.INVALID_CURRENCY",
                    "Money.currency is missing or not a string.",
                    ErrorType.ValidationError));
        }

        return Money.Create(amount, currencyElement.GetString() ?? string.Empty);
    }

    /// <inheritdoc />
    protected override void Format(Utf8JsonWriter writer, Money value)
    {
        writer.WriteStartObject();
        writer.WriteNumber("amount", value.Amount);
        writer.WriteString("currency", value.Currency.Value);
        writer.WriteEndObject();
    }
}
