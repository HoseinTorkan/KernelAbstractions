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
public sealed class Error : IError
{
    /// <inheritdoc />
    public string Code { get; }

    /// <inheritdoc />
    public string Text { get; }

    /// <inheritdoc />
    public ErrorType ErrorType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="code">The unique code identifying this error.</param>
    /// <param name="text">The human-readable text of this error.</param>
    /// <param name="errorType">The type of this error.</param>
    public Error(string code, string text, ErrorType errorType)
    {
        Code = code;
        Text = text;
        ErrorType = errorType;
    }
}