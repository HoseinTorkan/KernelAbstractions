using KernelAbstractions.Application.Services.AuthTokenService.Models;
using KernelAbstractions.Patterns.Results;
using System.Security.Claims;

namespace KernelAbstractions.Application.Services.AuthTokenService;

/// <summary>
/// Contract for generating and validating authentication tokens
/// (access tokens and refresh tokens) used in passwordless
/// and multi-factor authentication flows.
/// </summary>
/// <remarks>
/// Implementations are JWT-provider-agnostic. The access token is expected
/// to be short-lived and self-contained, while the refresh token is a
/// long-lived opaque value persisted server-side for revocation support.
/// </remarks>
public interface IAuthTokenService
{
    /// <summary>
    /// Generates a new access token and refresh token pair for the given claims.
    /// </summary>
    /// <param name="claims">The claims to embed in the access token.</param>
    /// <returns>
    /// A <see cref="TokenPair"/> containing the access token, the refresh token,
    /// and their respective expiration timestamps.
    /// </returns>
    TokenPair GenerateTokens(IEnumerable<Claim> claims);

    /// <summary>
    /// Validates the specified access token and returns its claims if valid.
    /// </summary>
    /// <param name="accessToken">The access token to validate.</param>
    /// <returns>
    /// A <see cref="ClaimsPrincipal"/> built from the token's claims on success;
    /// otherwise, a failure describing the reason.
    /// </returns>
    Result<ClaimsPrincipal> ValidateAccessToken(string accessToken);
}