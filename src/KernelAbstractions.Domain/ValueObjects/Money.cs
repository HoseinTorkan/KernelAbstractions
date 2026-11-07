using KernelR.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KernelR.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with its associated currency.
/// </summary>
/// <remarks>
/// <para>
/// This value object guarantees the following invariants:
/// </para>
/// <list type="bullet">
///   <item><description>
///     The <see cref="Amount"/> is always rounded to two decimal places using
///     <see cref="MidpointRounding.AwayFromZero"/>.
///   </description></item>
///   <item><description>
///     The <see cref="Currency"/> is always a valid, uppercase ISO 4217 code
///     (see <see cref="CurrencyCode"/>).
///   </description></item>
///   <item><description>
///     The <see cref="Amount"/> may be negative. Negative amounts represent debts,
///     balances, reversals, or withdrawals.
///   </description></item>
/// </list>
/// <para>
/// <b>Error model:</b> The <see cref="Create"/> factory returns <see cref="Result{T}"/>
/// for recoverable validation failures. Arithmetic operators, by contrast, throw
/// <see cref="InvalidOperationException"/> on currency mismatch, because mixing
/// currencies is a programming error rather than a recoverable condition.
/// </para>
/// <para>
/// <b>Currency validation:</b> The currency string is validated by
/// <see cref="CurrencyCode.Create(string)"/>. If the currency is invalid, the resulting
/// failure carries the error from <see cref="CurrencyCode"/> (codes
/// <c>Kernel.ValueObjects.CurrencyCode.EMPTY</c> or
/// <c>Kernel.ValueObjects.CurrencyCode.INVALID</c>).
/// </para>
/// </remarks>
[JsonConverter(typeof(MoneyJsonConverter))]
[TypeConverter(typeof(MoneyTypeConverter))]
public readonly record struct Money
{
    /// <summary>
    /// Gets the monetary amount, rounded to two decimal places.
    /// </summary>
    /// <remarks>
    /// The value may be negative.
    /// </remarks>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the validated ISO 4217 currency code.
    /// </summary>
    public CurrencyCode Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> struct.
    /// </summary>
    /// <param name="amount">The amount, which will be rounded to two decimal places.</param>
    /// <param name="currency">An already-validated currency code.</param>
    private Money(decimal amount, CurrencyCode currency)
    {
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency;
    }

    /// <summary>
    /// Creates a new <see cref="Money"/> from an amount and a currency code.
    /// </summary>
    /// <param name="amount">
    /// The monetary amount. May be negative. Values with more than two decimal places
    /// are rounded using <see cref="MidpointRounding.AwayFromZero"/>.
    /// </param>
    /// <param name="currency">
    /// A 3-letter ISO 4217 currency code (e.g. <c>IRR</c>, <c>USD</c>).
    /// Case-insensitive; validated through <see cref="CurrencyCode.Create(string)"/>.
    /// </param>
    /// <returns>
    /// A successful result carrying the new <see cref="Money"/>, or a failure if the
    /// currency code is invalid.
    /// </returns>
    public static Result<Money> Create(decimal amount, string currency)
    {
        Result<CurrencyCode> codeResult = CurrencyCode.Create(currency);

        return codeResult.IsFailure
            ? Result<Money>.Failure(codeResult.Error)
            : Result<Money>.Success(new Money(amount, codeResult.Value));
    }

    /// <summary>
    /// Creates a zero amount in the given currency.
    /// </summary>
    /// <param name="currency">A 3-letter ISO 4217 currency code.</param>
    /// <returns>
    /// A successful result carrying <c>0.00</c> in the given currency, or a failure
    /// when the currency code is invalid.
    /// </returns>
    public static Result<Money> Zero(string currency) => Create(0.00m, currency);

    #region Arithmetic Operators

    /// <summary>
    /// Adds two monetary amounts of the same currency.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>A new <see cref="Money"/> carrying the sum, in the currency of <paramref name="left"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="left"/> and <paramref name="right"/> have different currencies.
    /// </exception>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts one monetary amount from another of the same currency.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>
    /// A new <see cref="Money"/> carrying the difference, which may be negative.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="left"/> and <paramref name="right"/> have different currencies.
    /// </exception>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies a monetary amount by a scalar factor.
    /// </summary>
    /// <param name="money">The amount to multiply.</param>
    /// <param name="multiplier">The scalar factor. May be negative.</param>
    /// <returns>A new <see cref="Money"/> carrying the product, rounded to two decimal places.</returns>
    public static Money operator *(Money money, decimal multiplier)
        => new(money.Amount * multiplier, money.Currency);

    /// <summary>
    /// Divides a monetary amount by a scalar divisor.
    /// </summary>
    /// <param name="money">The amount to divide.</param>
    /// <param name="divisor">The scalar divisor. Must not be zero.</param>
    /// <returns>A new <see cref="Money"/> carrying the quotient, rounded to two decimal places.</returns>
    /// <exception cref="DivideByZeroException">
    /// Thrown when <paramref name="divisor"/> is zero.
    /// </exception>
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor is 0m)
        {
            throw new DivideByZeroException("Cannot divide money by zero.");
        }

        return new Money(money.Amount / divisor, money.Currency);
    }

    /// <summary>
    /// Negates a monetary amount.
    /// </summary>
    /// <param name="money">The amount to negate.</param>
    /// <returns>A new <see cref="Money"/> with the sign of <see cref="Amount"/> inverted.</returns>
    public static Money operator -(Money money)
        => new(-money.Amount, money.Currency);

    #endregion

    #region Comparison Operators (Money vs Money)

    /// <summary>
    /// Determines whether the left amount is greater than the right amount.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the operands have different currencies.
    /// </exception>
    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Determines whether the left amount is less than the right amount.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the operands have different currencies.
    /// </exception>
    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Determines whether the left amount is greater than or equal to the right amount.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the operands have different currencies.
    /// </exception>
    public static bool operator >=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    /// <summary>
    /// Determines whether the left amount is less than or equal to the right amount.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the operands have different currencies.
    /// </exception>
    public static bool operator <=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    #endregion

    #region Comparison Operators (Money vs decimal)

    /// <summary>
    /// Determines whether the amount is greater than the given scalar.
    /// </summary>
    /// <remarks>Currency is ignored on the scalar side; the comparison is purely numeric.</remarks>
    public static bool operator >(Money left, decimal right) => left.Amount > right;

    /// <summary>
    /// Determines whether the amount is less than the given scalar.
    /// </summary>
    /// <remarks>Currency is ignored on the scalar side; the comparison is purely numeric.</remarks>
    public static bool operator <(Money left, decimal right) => left.Amount < right;

    /// <summary>
    /// Determines whether the amount is greater than or equal to the given scalar.
    /// </summary>
    /// <remarks>Currency is ignored on the scalar side; the comparison is purely numeric.</remarks>
    public static bool operator >=(Money left, decimal right) => left.Amount >= right;

    /// <summary>
    /// Determines whether the amount is less than or equal to the given scalar.
    /// </summary>
    /// <remarks>Currency is ignored on the scalar side; the comparison is purely numeric.</remarks>
    public static bool operator <=(Money left, decimal right) => left.Amount <= right;

    #endregion

    /// <summary>
    /// Throws when the two operands have different currencies.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the currencies do not match.
    /// </exception>
    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot operate on amounts with different currencies: " +
                $"'{left.Currency}' vs '{right.Currency}'.");
        }
    }

    /// <summary>
    /// Returns a human-readable representation such as <c>1,234.56 IRR</c>.
    /// </summary>
    /// <returns>A string with two decimal places and the currency code.</returns>
    public override string ToString() => $"{Amount:N2} {Currency}";
}
