namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Represents a message that carries the outcome of an operation,
/// whether successful or failed.
/// </summary>
/// <remarks>
/// A message always contains a code for programmatic identification,
/// a human-readable text, and a type that indicates its nature.
/// </remarks>
/// <example>
/// <code>
/// var success = new Message("SUCCESS", "Operation completed successfully.", MessageType.Success);
/// var failure = new Message("PRODUCT_NOT_FOUND", "Product was not found.", MessageType.NotFound);
/// </code>
/// </example>
/// <seealso cref="IMessage"/>
/// <seealso cref="MessageType"/>
public sealed class Message : IMessage
{
    /// <inheritdoc />
    public string Code { get; }

    /// <inheritdoc />
    public string Text { get; }

    /// <inheritdoc />
    public MessageType MessageType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="code">The unique code identifying this message.</param>
    /// <param name="text">The human-readable text of this message.</param>
    /// <param name="messageType">The type of this message.</param>
    public Message(string code, string text, MessageType messageType)
    {
        Code = code;
        Text = text;
        MessageType = messageType;
    }
}