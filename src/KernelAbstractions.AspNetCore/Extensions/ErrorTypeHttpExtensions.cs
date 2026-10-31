using KernelAbstractions.Patterns.Results;
using System.Net;

namespace KernelAbstractions.AspNetCore.Extensions;

/// <summary>
/// Maps <see cref="ErrorType"/> values to HTTP status codes.
/// </summary>
/// <remarks>
/// <para>
/// Kept outside the <c>KernelAbstractions.Patterns.Results</c> package so the
/// domain layer remains free of HTTP dependencies. The grouping of
/// <see cref="ErrorType"/> values is designed to make this mapping stable
/// and predictable.
/// </para>
/// <para>
/// <see cref="ErrorType.Duplicate"/> and <see cref="ErrorType.Conflict"/> both
/// map to <c>409 Conflict</c> per RFC 7231. Clients that need to distinguish
/// them should inspect <see cref="Error.Code"/> (e.g. <c>PRODUCT_DUPLICATE</c>
/// vs <c>CONCURRENT_UPDATE</c>).
/// </para>
/// </remarks>
public static class ErrorTypeHttpExtensions
{
    /// <summary>
    /// Gets the HTTP status code that best represents the given <paramref name="errorType"/>.
    /// </summary>
    public static HttpStatusCode ToHttpStatusCode(this ErrorType errorType) => errorType switch
    {
        // Validation → 400 Bad Request
        ErrorType.ValidationError => HttpStatusCode.BadRequest,
        ErrorType.InvalidInput => HttpStatusCode.BadRequest,
        ErrorType.RequiredField => HttpStatusCode.BadRequest,
        ErrorType.InvalidFormat => HttpStatusCode.BadRequest,

        // Resource → 404 Not Found / 409 Conflict
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Duplicate => HttpStatusCode.Conflict,
        ErrorType.Conflict => HttpStatusCode.Conflict,

        // Auth → 401 / 403 / 429
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,
        ErrorType.InvalidToken => HttpStatusCode.Unauthorized,
        ErrorType.TooManyAttempts => HttpStatusCode.TooManyRequests,

        // Business rules → 422 Unprocessable Entity
        ErrorType.BusinessRuleViolation => HttpStatusCode.UnprocessableEntity,
        ErrorType.InsufficientStock => HttpStatusCode.UnprocessableEntity,
        ErrorType.InvalidStatus => HttpStatusCode.UnprocessableEntity,
        ErrorType.MinimumAmount => HttpStatusCode.UnprocessableEntity,
        ErrorType.MaximumAmount => HttpStatusCode.UnprocessableEntity,

        // Infrastructure → 500 / 502
        ErrorType.Infrastructure => HttpStatusCode.InternalServerError,
        ErrorType.DatabaseError => HttpStatusCode.InternalServerError,
        ErrorType.NetworkError => HttpStatusCode.BadGateway,
        ErrorType.ExternalServiceError => HttpStatusCode.BadGateway,
        ErrorType.FileError => HttpStatusCode.InternalServerError,

        // Transient → 408 / 409
        // 408 Request Timeout (not 504 Gateway Timeout): callers should retry.
        // Server-side timeouts are represented by NetworkError / ExternalServiceError.
        ErrorType.Timeout => HttpStatusCode.RequestTimeout,
        ErrorType.Concurrency => HttpStatusCode.Conflict,

        // General → 500
        ErrorType.UnknownError => HttpStatusCode.InternalServerError,

        // Fallback for any ErrorType not explicitly mapped.
        // Unknown errors are treated as server errors so they are not
        // mistaken for client mistakes.
        _ => HttpStatusCode.InternalServerError
    };
}