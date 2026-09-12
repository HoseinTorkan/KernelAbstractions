namespace KernelAbstractions.Patterns.Results;

/// <summary>
/// Defines a contract for the outcome of an operation that does not return a value.
/// A result is either successful or failed, and always carries an error on failure.
/// </summary>
/// <remarks>
/// This interface provides a functional alternative to throwing exceptions for
/// expected failures. It is designed to be used across all layers of the application.
/// </remarks>
/// <seealso cref="IResult{T}"/>
/// <seealso cref="IError"/>
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
    /// Gets the error associated with this result.
    /// Returns <c>null</c> when the operation is successful.
    /// </summary>
    IError? Error { get; }
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
/// is <c>true</c>. On a failed result, <see cref="Value"/> returns <c>default</c>.
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
/// <seealso cref="IError"/>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value produced by a successful operation.
    /// Returns <c>null</c> (or <c>default</c>) if the operation failed.
    /// </summary>
    T? Value { get; }
}