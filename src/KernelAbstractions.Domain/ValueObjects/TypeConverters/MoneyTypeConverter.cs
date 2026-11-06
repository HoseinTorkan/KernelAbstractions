using KernelAbstractions.Patterns.Results;
using System.Globalization;

namespace KernelAbstractions.Domain.ValueObjects.TypeConverters;

/// <summary>
/// A <see cref="System.ComponentModel.TypeConverter"/> that binds a string of the form
/// <c>"&lt;amount&gt; &lt;currency&gt;"</c> to a <see cref="Money"/> value object.
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see cref="ValueObjectTypeConverter{T}"/> to reuse its
/// <see cref="System.Text.Json.JsonSerializerOptions"/> instance and its
/// <see cref="FormatException"/> construction contract.
/// </para>
/// <para>
/// <b>Examples:</b> <c>"100 IRR"</c>, <c>"-50.25 USD"</c>.
/// </para>
/// </remarks>
public sealed class MoneyTypeConverter : ValueObjectTypeConverter<Money>
{
    /// <inheritdoc />
    protected override Result<Money> Create(string value)
    {
        string[] parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            return Result<Money>.Failure(
                new Error("Kernel.ValueObjects.Money.INVALID_FORMAT",
                          "Money must be formatted as '<amount> <currency>', e.g. '100 IRR'.",
                          ErrorType.ValidationError));
        }

        if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
        {
            return Result<Money>.Failure(
                new Error("Kernel.ValueObjects.Money.INVALID_FORMAT",
                          "Money amount is not a valid number.",
                          ErrorType.ValidationError));
        }

        return Money.Create(amount, parts[1]);
    }
}
