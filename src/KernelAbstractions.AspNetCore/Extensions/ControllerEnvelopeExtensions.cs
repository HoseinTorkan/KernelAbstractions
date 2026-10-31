using KernelAbstractions.AspNetCore.Models;
using KernelAbstractions.Patterns.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KernelAbstractions.AspNetCore.Extensions;

/// <summary>
/// Converts domain <see cref="Result"/> instances into <see cref="IActionResult"/>
/// values for MVC controllers.
/// </summary>
/// <remarks>
/// <para>
/// On success, the result is wrapped in an <see cref="ApiResponse{T}"/> envelope
/// with HTTP <c>200 OK</c>. On failure, the result is mapped to a standard
/// <see cref="ProblemDetails"/> payload with an HTTP status derived from
/// <see cref="ErrorType"/>, so that clients and tooling (Swagger, Postman)
/// observe the RFC 7807 shape they expect.
/// </para>
/// <para>
/// The <see cref="System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute"/>
/// annotations on <see cref="Result.IsSuccess"/> and <see cref="Result.IsFailure"/>
/// let the compiler prove that <see cref="Result{T}.Value"/> is non-null in the
/// success branch and that <see cref="Result{T}.Error"/> is non-null in the
/// failure branch, so no null-forgiving operator is required.
/// </para>
/// <para>
/// This split keeps the success contract rich and predictable for the
/// front-end, while letting failures benefit from ASP.NET Core's standard
/// error handling (including the automatic <c>traceId</c> extension).
/// </para>
/// </remarks>
public static class ControllerEnvelopeExtensions
{
    /// <summary>
    /// Wraps a generic <see cref="Result{T}"/> in an <see cref="ApiResponse{T}"/>
    /// on success, or a <see cref="ProblemDetails"/> on failure.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="controller"/> or <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IActionResult EnvelopeResult<T>(this ControllerBase controller, Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsFailure)
        {
            return BuildProblemDetails(controller, result.Error);
        }

        ApiResponse<T> response = result.ToApiResponse(controller.HttpContext?.TraceIdentifier);
        return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
    }

    /// <summary>
    /// Wraps a non-generic <see cref="Result"/> in an <see cref="ApiResponse"/>
    /// on success, or a <see cref="ProblemDetails"/> on failure.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="controller"/> or <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IActionResult EnvelopeResult(this ControllerBase controller, Result result)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsFailure)
        {
            return BuildProblemDetails(controller, result.Error);
        }

        ApiResponse response = result.ToApiResponse(controller.HttpContext?.TraceIdentifier);
        return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
    }

    /// <summary>
    /// Builds a <see cref="ProblemDetails"/>-shaped <see cref="ObjectResult"/>
    /// from a domain <see cref="Error"/>.
    /// </summary>
    /// <remarks>
    /// Returns the concrete <see cref="ObjectResult"/> rather than
    /// <see cref="IActionResult"/> so the compiler can inline the call and
    /// avoid the virtual dispatch that CA1859 flags. Callers upcast to
    /// <see cref="IActionResult"/> implicitly.
    /// </remarks>
    private static ObjectResult BuildProblemDetails(ControllerBase controller, Error error)
    {
        int statusCode = (int)error.ErrorType.ToHttpStatusCode();
        string traceId = controller.HttpContext?.TraceIdentifier ?? ResultConversionExtensions.ResolveTraceId();

        ProblemDetails problem = new()
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = error.Code,
            Detail = error.Text,
            Status = statusCode,
            Instance = controller.HttpContext?.Request.Path
        };

        problem.Extensions["traceId"] = traceId;
        problem.Extensions["timestamp"] = DateTimeOffset.UtcNow;
        problem.Extensions["errorType"] = error.ErrorType.ToString();

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
