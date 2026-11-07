using KernelAbstractions.Patterns.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KernelR.AspNetCore.ErrorHandling;

/// <summary>
/// Handles unhandled exceptions and converts them into a standardized
/// <see cref="ProblemDetails"/> response.
/// </summary>
/// <remarks>
/// <para>
/// This handler is registered with <c>AddExceptionHandler&lt;KernelRExceptionHandler&gt;()</c>
/// and runs via the standard <c>UseExceptionHandler()</c> middleware. It acts as the
/// final safety net for all unhandled exceptions.
/// </para>
/// <para>
/// <b>Exception categories:</b>
/// </para>
/// <list type="bullet">
///   <item><description>
///     <see cref="JsonException"/> produced by KernelR JSON converters is treated as a
///     <b>validation failure</b> (400) rather than a server error, because it represents
///     malformed or invalid input, not a bug. The exception message is expected to follow
///     the <c>"&lt;code&gt;: &lt;text&gt;"</c> contract produced by
///     <c>BaseValueObjectJsonConverter&lt;T&gt;</c>.
///   </description></item>
///   <item><description>
///     <see cref="BadHttpRequestException"/> raised by ASP.NET Core Minimal API binding
///     (e.g. a failed query-string or body binding) is treated as a <b>validation
///     failure</b> (400). If its <see cref="Exception.InnerException"/> is a
///     <see cref="JsonException"/> produced by a KernelR converter, the original error
///     code and message are preserved; otherwise a generic
///     <c>KernelR.Validation.InvalidInput</c> error is produced.
///   </description></item>
///   <item><description>
///     All other exceptions are reported as <b>500</b> with a generic message, so that
///     internal details (stack traces, SQL, file paths) are never exposed to clients.
///   </description></item>
/// </list>
/// </remarks>
public sealed class KernelRExceptionHandler : IExceptionHandler
{
    private const string UnexpectedErrorMessage =
        "An unexpected error occurred while processing your request.";

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Error error = exception switch
        {
            JsonException jsonException => BuildJsonError(jsonException.Message),

            BadHttpRequestException badRequest => BuildBadRequestError(badRequest),

            _ => new Error(
                "KernelR.Infrastructure.UnhandledException",
                UnexpectedErrorMessage,
                ErrorType.Infrastructure)
        };

        int statusCode = error.ErrorType.ToHttpStatusCode().ToInt32();

        ProblemDetails problemDetails = new()
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = error.Code,
            Detail = error.Text,
            Status = statusCode,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
        problemDetails.Extensions["errorType"] = error.ErrorType.ToString();

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private static Error BuildBadRequestError(BadHttpRequestException exception)
    {
        if (exception.InnerException is JsonException inner)
        {
            return BuildJsonError(inner.Message);
        }

        return new Error(
            "KernelR.Validation.InvalidInput",
            exception.Message,
            ErrorType.ValidationError);
    }

    private static Error BuildJsonError(string message)
    {
        int separatorIndex = message.IndexOf(": ", StringComparison.Ordinal);

        return separatorIndex > 0
            ? new Error(
                message[..separatorIndex],
                message[(separatorIndex + 2)..],
                ErrorType.ValidationError)
            : new Error(
                "KernelR.Validation.InvalidJson",
                "The request body is not valid JSON or does not match the expected shape.",
                ErrorType.ValidationError);
    }
}
