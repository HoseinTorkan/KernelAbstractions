using KernelAbstractions.Application.Services.CurrentUserService.Models;
using KernelAbstractions.Domain.Entities;
using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Application.Services.CurrentUserService;

/// <summary>
/// Provides access to the currently authenticated user's identity.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the currently authenticated user from JWT claims (no database access).
    /// </summary>
    /// <returns>
    /// The current user built from the token's claims, or a failure if the
    /// request is not authenticated or required claims are missing.
    /// </returns>
    Result<CurrentUser> GetCurrentUserFromClaims();

    /// <summary>
    /// Loads the full user entity from the database for the current user.
    /// </summary>
    /// <typeparam name="TUser">
    /// The concrete user entity type. Must implement
    /// <see cref="ICanBeCurrentUser"/>; this prevents other entities from
    /// being passed by mistake.
    /// </typeparam>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>
    /// The current user entity, or a failure if the user is not found.
    /// </returns>
    Task<Result<TUser>> GetCurrentUserFromDbAsync<TUser>(
        CancellationToken cancellation = default)
        where TUser : class, ICanBeCurrentUser;
}
