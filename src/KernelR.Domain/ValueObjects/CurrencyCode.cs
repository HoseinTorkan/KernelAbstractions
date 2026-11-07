using KernelR.Patterns.Results;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KernelR.Domain.ValueObjects;

/// <summary>
/// Represents a valid ISO 4217 currency code.
/// </summary>
/// <remarks>
/// <para>
/// The value is always a 3-letter uppercase code. Validation checks the input against
/// the ISO 4217 currency list, so structurally valid but unassigned codes
/// (e.g. <c>XYZ</c>) are rejected.
/// </para>
/// <para>
/// <b>Iranian Toman:</b> In addition to the official ISO 4217 codes, this type also
/// accepts <c>IRT</c> (Iranian Toman), which is widely used in Iranian applications
/// but is not part of the official ISO 4217 list (only <c>IRR</c> is).
/// </para>
/// <para>
/// <b>Maintenance:</b> The list of accepted codes is maintained internally and must
/// be updated when ISO 4217 changes (which is rare). Consumers cannot extend the list,
/// ensuring that an invalid code can never enter the domain model.
/// </para>
/// </remarks>
[JsonConverter(typeof(CurrencyCodeJsonConverter))]
[TypeConverter(typeof(CurrencyCodeTypeConverter))]
public readonly record struct CurrencyCode
{
    /// <summary>
    /// The set of accepted currency codes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Source: ISO 4217 (active codes) plus <c>IRT</c> for Iranian Toman.
    /// </para>
    /// <para>
    /// Fund codes, precious metals, and other non-currency ISO codes (e.g. <c>XAU</c>,
    /// <c>XDR</c>) are intentionally excluded; this type models spendable currencies.
    /// </para>
    /// </remarks>
    private static readonly HashSet<string> ValidCodes = new(StringComparer.Ordinal)
    {
        // ─── ISO 4217 active currencies ───
        "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN",
        "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BOV",
        "BRL", "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHE", "CHF",
        "CHW", "CLF", "CLP", "CNY", "COP", "COU", "CRC", "CUC", "CUP", "CVE",
        "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD",
        "FKP", "GBP", "GEL", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD",
        "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "INR", "IQD", "IRR", "ISK",
        "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KMF", "KPW", "KRW", "KWD",
        "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL",
        "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN",
        "MXV", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR",
        "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD",
        "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLE",
        "SOS", "SRD", "SSP", "STN", "SVC", "SYP", "SZL", "THB", "TJS", "TMT",
        "TND", "TOP", "TRY", "TTD", "TWD", "TZS", "UAH", "UGX", "USD", "USN",
        "UYI", "UYU", "UYW", "UZS", "VED", "VES", "VND", "VUV", "WST", "XAF",
        "XCD", "XOF", "XPF", "YER", "ZAR", "ZMW", "ZWL",

        // ─── Non-ISO extension: Iranian Toman ───
        "IRT"
    };

    /// <summary>
    /// Gets the uppercase 3-letter currency code.
    /// </summary>
    public string Value { get; }

    private CurrencyCode(string value) => Value = value;

    /// <summary>
    /// Creates a new <see cref="CurrencyCode"/> from the given string.
    /// </summary>
    /// <param name="value">
    /// The raw currency code. Case-insensitive; surrounding whitespace is trimmed.
    /// </param>
    /// <returns>
    /// A successful result carrying the normalized code, or a failure with one of:
    /// <list type="bullet">
    ///   <item><description><c>Kernel.ValueObjects.CurrencyCode.EMPTY</c> when the input is null or whitespace.</description></item>
    ///   <item><description><c>Kernel.ValueObjects.CurrencyCode.INVALID</c> when the input is not a recognized ISO 4217 code.</description></item>
    /// </list>
    /// </returns>
    public static Result<CurrencyCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<CurrencyCode>.Failure(
                new Error("Kernel.ValueObjects.CurrencyCode.EMPTY",
                          "Currency code cannot be empty.",
                          ErrorType.ValidationError));
        }

        string normalized = value.Trim().ToUpperInvariant();

        if (!ValidCodes.Contains(normalized))
        {
            return Result<CurrencyCode>.Failure(
                new Error("Kernel.ValueObjects.CurrencyCode.INVALID",
                          "Currency code is not a valid ISO 4217 code.",
                          ErrorType.ValidationError));
        }

        return Result<CurrencyCode>.Success(new CurrencyCode(normalized));
    }

    /// <summary>
    /// Returns the currency code as a string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="CurrencyCode"/> to its string value.
    /// </summary>
    public static implicit operator string(CurrencyCode code) => code.Value;
}
