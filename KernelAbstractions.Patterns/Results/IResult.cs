namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Defines a contract for the outcome of an operation that does not return a value.
/// A result is either successful or failed, and always carries a message.
/// </summary>
/// <remarks>
/// This interface provides a functional alternative to throwing exceptions for
/// expected failures. It is designed to be used across all layers of the application.
/// </remarks>
/// <seealso cref="IResult{T}"/>
/// <seealso cref="IMessage"/>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the message associated with this result.
    /// </summary>
    /// <remarks>
    /// The message is always present, regardless of success or failure.
    /// For successful results, it typically contains a confirmation message.
    /// For failed results, it contains the error description.
    /// </remarks>
    IMessage Message { get; }
}

/// <summary>
/// Defines a contract for the outcome of an operation that returns a value.
/// A result is either successful (with a value) or failed (without a value).
/// </summary>
/// <typeparam name="T">
/// The type of the value returned on success.
/// This type parameter is covariant, allowing a more derived type to be used
/// where a less derived type is expected.
/// </typeparam>
/// <remarks>
/// <see cref="Value"/> is expected to be accessed only when <see cref="IResult.IsSuccess"/>
/// is <c>true</c>. Implementations should throw or return <c>default</c> when accessed
/// on a failed result, depending on the chosen strategy.
/// </remarks>
/// <example>
/// <code>
/// IResult&lt;Product&gt; result = await _repository.GetProductAsync(id);
/// if (result.IsSuccess)
/// {
///     Console.WriteLine(result.Value.Name);
/// }
/// </code>
/// </example>
/// <seealso cref="IResult"/>
/// <seealso cref="IMessage"/>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value produced by a successful operation.
    /// Returns <c>null</c> (or <c>default</c>) if the operation failed.
    /// </summary>
    T? Value { get; }
}