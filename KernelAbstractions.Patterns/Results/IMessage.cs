namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Defines a contract for messages returned as part of a result.
/// A message represents the outcome of an operation, whether successful or failed.
/// </summary>
/// <remarks>
/// This interface is used by both successful and failed results, allowing a single
/// mechanism to carry feedback to the caller. The <see cref="MessageType"/> property
/// indicates the nature of the message (success, validation error, not found, etc.).
/// </remarks>
/// <example>
/// <code>
/// var success = new Message("SUCCESS", "Operation completed successfully.", MessageType.Success);
/// var failure = new Message("PRODUCT_NOT_FOUND", "Product was not found.", MessageType.NotFound);
/// </code>
/// </example>
/// <seealso cref="Message"/>
/// <seealso cref="MessageType"/>
/// <seealso cref="IResult"/>
public interface IMessage
{
    /// <summary>
    /// Gets the unique code identifying this message.
    /// </summary>
    /// <remarks>
    /// The code is intended for programmatic identification and should be
    /// stable across versions. Examples: "PRODUCT_NOT_FOUND", "INVALID_EMAIL".
    /// </remarks>
    string Code { get; }

    /// <summary>
    /// Gets the human-readable text of this message.
    /// </summary>
    /// <remarks>
    /// The text is intended for display to users or for logging.
    /// It should be concise and informative.
    /// </remarks>
    string Text { get; }
    
    /// <summary>
    /// Gets the type of this message, indicating its nature
    /// (e.g., success, validation error, not found).
    /// </summary>
    MessageType MessageType { get; }
}