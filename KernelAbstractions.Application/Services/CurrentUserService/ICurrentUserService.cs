using KernelAbstractions.Application.Services.CurrentUserService.Models;
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
    Result<CurrentUser> GetCurrentUserFromClaims();

    /// <summary>
    /// Loads the full user entity from the database.
    /// </summary>
    /// <typeparam name="TUser">The type of the user entity.</typeparam>
    Task<Result<TUser?>> GetCurrentUserFromDbAsync<TUser>(CancellationToken cancellation = default)
        where TUser : class;
}