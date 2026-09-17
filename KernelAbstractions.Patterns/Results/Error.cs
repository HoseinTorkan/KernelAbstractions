namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Represents an error that describes the failure of an operation.
/// An error always contains a code for programmatic identification,
/// a human-readable text, and a type that indicates its nature.
/// </summary>
/// <remarks>
/// Errors are used in conjunction with <see cref="Result"/> and <see cref="Result{T}"/>
/// to describe why an operation failed. They replace the need for throwing exceptions
/// in expected failure scenarios.
/// <para>
/// The <see cref="None"/> static member represents the absence of an error and is
/// used internally by successful results.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var notFound = new Error("PRODUCT_NOT_FOUND", "Product was not found.", ErrorType.NotFound);
/// var validation = new Error("INVALID_EMAIL", "Email format is invalid.", ErrorType.ValidationError);
/// </code>
/// </example>
/// <seealso cref="IError"/>
/// <seealso cref="ErrorType"/>
/// <seealso cref="Result"/>
/// <seealso cref="Result{T}"/>
public sealed class Error(string code, string text, ErrorType errorType) : IError
{
    /// <summary>
    /// Represents the absence of an error.
    /// Used by successful results to indicate that no error occurred.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <inheritdoc />
    public string Code { get; } = code;

    /// <inheritdoc />
    public string Text { get; } = text;

    /// <inheritdoc />
    public ErrorType ErrorType { get; } = errorType;
}