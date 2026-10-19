using KernelAbstractions.Application.Services.CurrentUserService.Models;
using KernelAbstractions.Patterns.Results;
using System.Linq.Expressions;

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
    /// <param name="predicate">An optional filter to apply to the user entity.</param>
    /// <param name="cancellation">Cancellation token.</param>
    Task<Result<TUser>> GetCurrentUserFromDbAsync<TUser>(Expression<Func<TUser, bool>>? predicate = null, CancellationToken cancellation = default)
        where TUser : class;
}