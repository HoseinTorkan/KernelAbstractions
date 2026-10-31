using KernelAbstractions.AspNetCore.Models;
using KernelAbstractions.Patterns.Results;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KernelAbstractions.AspNetCore.Extensions;

/// <summary>
/// Converts domain <see cref="Result"/> instances to their presentation-layer
/// <see cref="ApiResponse"/> counterparts.
/// </summary>
/// <remarks>
/// <para>
/// The conversion always produces a non-null <see cref="ApiResponse.TraceId"/>.
/// If the caller does not supply one, the current <see cref="Activity.Id"/>
/// is used, falling back to a fresh GUID so the response is always correlatable.
/// </para>
/// <para>
/// These extensions are intentionally <see langword="public"/>: they are the
/// building blocks for
/// <see cref="ControllerEnvelopeExtensions.EnvelopeResult(ControllerBase, Result)"/>
/// and <see cref="MinimalApiEnvelopeExtensions.ToHttpResult(Result, string)"/>, and
/// consumers may also need them directly in middleware, result filters, Blazor
/// components, or gRPC services that do not have access to a <c>ControllerBase</c>.
/// </para>
/// </remarks>
public static class ResultConversionExtensions
{
    /// <summary>
    /// Converts a domain <see cref="Result"/> to an <see cref="ApiResponse"/>.
    /// </summary>
    /// <param name="result">The domain result to convert. Must not be <see langword="null"/>.</param>
    /// <param name="traceId">
    /// The correlation identifier for the request. If <see langword="null"/>,
    /// the current <see cref="Activity.Id"/> is used, or a fresh GUID as a last resort.
    /// </param>
    /// <returns>
    /// A successful or failed <see cref="ApiResponse"/> with a non-null
    /// <see cref="ApiResponse.TraceId"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static ApiResponse ToApiResponse(this Result result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        traceId ??= ResolveTraceId();

        if (result.IsFailure)
            return ApiResponse.Failure(result.Error) with { TraceId = traceId };

        return ApiResponse.Success() with { TraceId = traceId };
    }

    /// <summary>
    /// Converts a domain <see cref="Result{T}"/> to an <see cref="ApiResponse{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value produced on success.</typeparam>
    /// <param name="result">The domain result to convert. Must not be <see langword="null"/>.</param>
    /// <param name="traceId">
    /// The correlation identifier for the request. If <see langword="null"/>,
    /// the current <see cref="Activity.Id"/> is used, or a fresh GUID as a last resort.
    /// </param>
    /// <returns>
    /// A successful or failed <see cref="ApiResponse{T}"/> with a non-null
    /// <see cref="ApiResponse{T}.TraceId"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="result"/> is <see langword="null"/>.
    /// </exception>
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        traceId ??= ResolveTraceId();

        if (result.IsSuccess)
            return ApiResponse<T>.Success(result.Value) with { TraceId = traceId };

        return ApiResponse<T>.Failure(result.Error) with { TraceId = traceId };
    }

    /// <summary>
    /// Resolves a trace identifier for the current request, preferring the
    /// active <see cref="Activity"/>'s id and falling back to a new GUID.
    /// </summary>
    /// <remarks>
    /// <see langword="internal"/> because it is an implementation detail;
    /// consumers should rely on <see cref="ToApiResponse(Result, string?)"/>
    /// or the higher-level extensions instead.
    /// </remarks>
    internal static string ResolveTraceId()
        => Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
}