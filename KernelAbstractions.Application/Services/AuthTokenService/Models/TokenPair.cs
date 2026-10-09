namespace KernelAbstractions.Application.Services.AuthTokenService.Models;

/// <summary>
/// Represents a pair of access and refresh tokens issued to a user.
/// </summary>
public sealed class TokenPair
{
    /// <summary>Gets the short-lived access token.</summary>
    public required string AccessToken { get; init; }

    /// <summary>Gets the UTC timestamp when the access token expires.</summary>
    public required DateTime AccessTokenExpiresAt { get; init; }

    /// <summary>Gets the long-lived refresh token.</summary>
    public required string RefreshToken { get; init; }

    /// <summary>Gets the UTC timestamp when the refresh token expires.</summary>
    public required DateTime RefreshTokenExpiresAt { get; init; }
}