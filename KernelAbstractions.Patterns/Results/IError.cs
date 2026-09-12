namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Defines a contract for errors that describe the failure of an operation.
/// An error always contains a code for programmatic identification,
/// a human-readable text, and a type that indicates its nature.
/// </summary>
/// <remarks>
/// Errors are used in conjunction with <see cref="IResult"/> to describe why
/// an operation failed. They replace the need for throwing exceptions in
/// expected failure scenarios.
/// </remarks>
/// <example>
/// <code>
/// var notFound = new Error("PRODUCT_NOT_FOUND", "Product was not found.", ErrorType.NotFound);
/// var validation = new Error("INVALID_EMAIL", "Email format is invalid.", ErrorType.ValidationError);
/// </code>
/// </example>
/// <seealso cref="Error"/>
/// <seealso cref="ErrorType"/>
/// <seealso cref="IResult"/>
public interface IError
{
    /// <summary>
    /// Gets the unique code identifying this error.
    /// </summary>
    /// <remarks>
    /// The code is intended for programmatic identification and should be
    /// stable across versions. Examples: "PRODUCT_NOT_FOUND", "INVALID_EMAIL".
    /// </remarks>
    string Code { get; }

    /// <summary>
    /// Gets the human-readable text of this error.
    /// </summary>
    /// <remarks>
    /// The text is intended for display to users or for logging.
    /// It should be concise and informative.
    /// </remarks>
    string Text { get; }

    /// <summary>
    /// Gets the type of this error, indicating its nature
    /// (e.g., validation error, not found, conflict).
    /// </summary>
    ErrorType ErrorType { get; }
}