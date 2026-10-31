using KernelAbstractions.Patterns.Results;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace KernelAbstractions.AspNetCore.Models;

/// <summary>
/// Represents the serialized form of a <see cref="Result"/> that is returned
/// from a Web API endpoint to a front-end client <strong>on success</strong>.
/// </summary>
/// <remarks>
/// <para>
/// On failure, the Web API returns a standard <c>ProblemDetails</c> payload
/// (RFC 7807) instead of an <see cref="ApiResponse"/>. This split keeps the
/// success contract rich and front-end friendly while letting errors follow
/// the ASP.NET Core standard, which is understood by Swagger, Postman, and
/// other tooling out of the box.
/// </para>
/// <para>
/// Because this type is only used for successful outcomes, <see cref="Error"/>
/// is always <see langword="null"/> on instances produced by the Web API.
/// The property is kept on the type so that any consumer that stores a
/// heterogeneous list of responses (e.g. a result filter) still compiles.
/// </para>
/// </remarks>
/// <seealso cref="ApiResponse{T}"/>
/// <seealso cref="Result"/>
/// <seealso cref="Error"/>
public sealed record ApiResponse
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    /// <remarks>
    /// When this property is <see langword="false"/>, <see cref="Error"/> is
    /// guaranteed to be non-null. This lets the compiler reason about
    /// <see cref="Error"/> in both branches of an <c>if (response.IsSuccess)</c>.
    /// </remarks>
    [JsonPropertyName("success")]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is the logical negation of <see cref="IsSuccess"/> and is
    /// intentionally excluded from the JSON payload; clients should rely on
    /// <see cref="IsSuccess"/> instead.
    /// When this property is <see langword="true"/>, <see cref="Error"/> is
    /// guaranteed to be non-null.
    /// </remarks>
    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error describing the failure, or <see langword="null"/> on success.
    /// </summary>
    /// <remarks>
    /// Always <see langword="null"/> for responses produced by the Web API on
    /// success. Retained for compatibility with generic result handlers that
    /// also process the domain <see cref="Error"/> type.
    /// </remarks>
    [JsonPropertyName("error")]
    public Error? Error { get; }

    /// <summary>
    /// Gets the correlation identifier for the request that produced this response,
    /// or <see langword="null"/> if not available.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the UTC timestamp at which this response was produced.
    /// </summary>
    /// <remarks>
    /// Set once by the constructor and intentionally not exposed as
    /// <see langword="init"/>, so it cannot be altered via a <c>with</c>
    /// expression.
    /// </remarks>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; }

    private ApiResponse(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
        Timestamp = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a successful response with <see cref="Error"/> set to <see langword="null"/>.
    /// </summary>
    public static ApiResponse Success()
        => new(true, null);

    /// <summary>
    /// Creates a failed response carrying the specified <paramref name="error"/>.
    /// </summary>
    /// <remarks>
    /// This factory exists for completeness and for callers that want to build
    /// an <see cref="ApiResponse"/> directly. The Web API layer, however,
    /// <strong>does not</strong> use it: failures are mapped to
    /// <c>ProblemDetails</c> instead. Use this factory only when an
    /// <see cref="ApiResponse"/> envelope is required for both outcomes
    /// (e.g. a custom endpoint that intentionally deviates from the standard).
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static ApiResponse Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, error);
    }
}

/// <summary>
/// Represents the serialized form of a <see cref="Result{T}"/> that is returned
/// from a Web API endpoint to a front-end client <strong>on success</strong>.
/// </summary>
/// <typeparam name="T">The type of the value produced on success.</typeparam>
/// <remarks>
/// See <see cref="ApiResponse"/> for the design rationale. This type mirrors
/// the same principles for the generic case.
/// </remarks>
/// <seealso cref="ApiResponse"/>
/// <seealso cref="Result{T}"/>
/// <seealso cref="Error"/>
public sealed record ApiResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    /// <remarks>
    /// When this property is <see langword="true"/>, <see cref="Value"/> is guaranteed
    /// to be non-null for reference-type <typeparamref name="T"/>.
    /// When this property is <see langword="false"/>, <see cref="Error"/> is
    /// guaranteed to be non-null.
    /// </remarks>
    [JsonPropertyName("success")]
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is the logical negation of <see cref="IsSuccess"/> and is
    /// intentionally excluded from the JSON payload; clients should rely on
    /// <see cref="IsSuccess"/> instead.
    /// When this property is <see langword="true"/>, <see cref="Error"/> is
    /// guaranteed to be non-null.
    /// </remarks>
    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the value produced by a successful operation.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Value { get; }

    /// <summary>
    /// Gets the error describing the failure, or <see langword="null"/> on success.
    /// </summary>
    /// <remarks>
    /// Always <see langword="null"/> for responses produced by the Web API on
    /// success. See <see cref="ApiResponse.Error"/> for the rationale.
    /// </remarks>
    [JsonPropertyName("error")]
    public Error? Error { get; }

    /// <summary>
    /// Gets the correlation identifier for the request that produced this response.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the UTC timestamp at which this response was produced.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; }

    private ApiResponse(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Timestamp = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a successful response carrying the specified <paramref name="value"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/> and <typeparamref name="T"/>
    /// is a reference type.
    /// </exception>
    public static ApiResponse<T> Success(T value)
    {
        if (value is null && default(T) is null)
        {
            throw new ArgumentNullException(
                nameof(value),
                $"ApiResponse<{typeof(T).Name}>.Success requires a non-null value. " +
                "Use ApiResponse.Success() for void operations, or a nullable type " +
                "(e.g. ApiResponse<int?>) when null is a valid success value.");
        }

        return new(true, value, null);
    }

    /// <summary>
    /// Creates a failed response carrying the specified <paramref name="error"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="ApiResponse.Failure"/> for the rationale; the Web API
    /// layer maps failures to <c>ProblemDetails</c> instead.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static ApiResponse<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, default, error);
    }
}