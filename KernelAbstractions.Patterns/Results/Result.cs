namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// A result is either successful or failed, and always carries a message.
/// </summary>
/// <remarks>
/// This class provides a functional alternative to throwing exceptions for
/// expected failures. It is designed to be used across all layers of the application.
/// </remarks>
/// <example>
/// <code>
/// var success = Result.Success(Message.Success("Operation completed successfully."));
/// var failure = Result.Failure(Message.NotFound("Product was not found."));
/// </code>
/// </example>
/// <seealso cref="Result{T}"/>
/// <seealso cref="IResult"/>
public class Result : IResult
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public IMessage Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="message">The message associated with the result.</param>
    protected Result(bool isSuccess, IMessage message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    /// <summary>
    /// Creates a successful result with the specified message.
    /// </summary>
    /// <param name="message">The message associated with the success.</param>
    /// <returns>A new successful <see cref="Result"/>.</returns>
    public static Result Success(IMessage message)
        => new(true, message);

    /// <summary>
    /// Creates a failed result with the specified message.
    /// </summary>
    /// <param name="message">The message describing the failure.</param>
    /// <returns>A new failed <see cref="Result"/>.</returns>
    public static Result Failure(IMessage message)
        => new(false, message);
}

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// A result is either successful (with a value) or failed (without a value).
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
/// <remarks>
/// <see cref="Value"/> is expected to be accessed only when
/// <see cref="IResult.IsSuccess"/> is <c>true</c>. On a failed result,
/// <see cref="Value"/> returns <c>default</c>.
/// </remarks>
/// <example>
/// <code>
/// Result&lt;Product&gt; result = await _repository.GetProductAsync(id);
/// if (result.IsSuccess)
/// {
///     Console.WriteLine(result.Value!.Name);
/// }
/// </code>
/// </example>
/// <seealso cref="Result"/>
/// <seealso cref="IResult{T}"/>
public sealed class Result<T> : IResult<T>
{
    private readonly T? _value;

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public IMessage Message { get; }

    /// <inheritdoc />
    public T? Value => IsSuccess ? _value : default;

    private Result(T? value, bool isSuccess, IMessage message)
    {
        _value = value;
        IsSuccess = isSuccess;
        Message = message;
    }

    /// <summary>
    /// Creates a successful result with the specified value and message.
    /// </summary>
    /// <param name="value">The value produced by the successful operation.</param>
    /// <param name="message">The message associated with the success.</param>
    /// <returns>A new successful <see cref="Result{T}"/>.</returns>
    public static Result<T> Success(T value, IMessage message)
        => new(value, true, message);

    /// <summary>
    /// Creates a failed result with the specified message.
    /// </summary>
    /// <param name="message">The message describing the failure.</param>
    /// <returns>A new failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Failure(IMessage message)
        => new(default, false, message);
}