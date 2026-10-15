using System.Security.Claims;

namespace KernelAbstractions.Application.Services.CurrentUserService.Models;

/// <summary>
/// Represents the currently authenticated user's identity.
/// </summary>
public sealed class CurrentUser
{
    /// <summary>Gets the user's unique identifier.</summary>
    public required Guid UserId { get; init; }

    /// <summary>Gets the user's role name.</summary>
    public required string Role { get; init; }

    /// <summary>Gets the user's phone number, if available.</summary>
    public required string PhoneNumber { get; init; }

    /// <summary>Gets all claims associated with the user.</summary>
    public required IEnumerable<Claim> Claims { get; init; }
}