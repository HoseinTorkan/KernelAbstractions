namespace KernelAbstractions.Application.Services.OtpService.Models;

/// <summary>
/// Details of a generated One-Time Password.
/// </summary>
public sealed class OtpDetails
{
    /// <summary>Gets the generated OTP code.</summary>
    public required string Code { get; init; }

    /// <summary>Gets the UTC timestamp when the OTP expires.</summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>Gets the maximum allowed verification attempts before invalidation.</summary>
    public required int MaxAttempts { get; init; }
}