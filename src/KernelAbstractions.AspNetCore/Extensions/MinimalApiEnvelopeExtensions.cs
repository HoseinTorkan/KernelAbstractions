using KernelAbstractions.AspNetCore.Models;
using KernelAbstractions.Patterns.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KernelAbstractions.AspNetCore.Extensions;

/// <summary>
/// Converts domain <see cref="Result"/> instances into <see cref="IResult"/>
/// values suitable for Minimal API endpoints.
/// </summary>
/// <remarks>
/// <para>
/// Mirrors <see cref="ControllerEnvelopeExtensions"/>: success becomes an
/// <see cref="ApiResponse{T}"/>, failure becomes a standard
/// <see cref="ProblemDetails"/> with an HTTP status derived from
/// <see cref="ErrorType"/>.
/// </para>
/// <para>
/// The <see cref="System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute"/>
/// annotations on <see cref="Result.IsSuccess"/> and <see cref="Result.IsFailure"/>
/// let the compiler prove that <see cref="Result{T}.Value"/> is non-null in the
/// success branch and that <see cref="Result{T}.Error"/> is non-null in the
/// failure branch, so no null-forgiving operator is required in either case.
/// </para>
/// </remarks>
public static class MinimalApiEnvelopeExtensions
{
    /// <summary>
    /// Converts a <see cref="Result"/> to an <see cref="IResult"/>: <c>200 OK</c>
    /// with an <see cref="ApiResponse"/> on success, or a <see cref="ProblemDetails"/>
    /// on failure.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IResult ToHttpResult(this Result result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            ApiResponse response = ApiResponse.Success()
                with
            { TraceId = traceId ?? ResultConversionExtensions.ResolveTraceId() };

            return Results.Ok(response);
        }

        return BuildProblemDetails(result.Error, traceId);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IResult"/>: <c>200 OK</c>
    /// with an <see cref="ApiResponse{T}"/> on success, or a <see cref="ProblemDetails"/>
    /// on failure.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static IResult ToHttpResult<T>(this Result<T> result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            ApiResponse<T> response = ApiResponse<T>.Success(result.Value)
                with
            { TraceId = traceId ?? ResultConversionExtensions.ResolveTraceId() };

            return Results.Ok(response);
        }

        return BuildProblemDetails(result.Error, traceId);
    }

    /// <summary>
    /// Builds an <see cref="IResult"/> that serializes a <see cref="ProblemDetails"/>
    /// payload for the given domain <paramref name="error"/>.
    /// </summary>
    private static IResult BuildProblemDetails(Error error, string? traceId)
    {
        int statusCode = (int)error.ErrorType.ToHttpStatusCode();

        return Results.Problem(
            type: $"https://httpstatuses.io/{statusCode}",
            title: error.Code,
            detail: error.Text,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = traceId ?? ResultConversionExtensions.ResolveTraceId(),
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["errorType"] = error.ErrorType.ToString()
            });
    }
}
