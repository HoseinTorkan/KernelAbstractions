using KernelAbstractions.Application.Services.CurrentUserService.Models;
using KernelAbstractions.Patterns.Results;

namespace KernelAbstractions.Application.Services.CurrentUserService;

/// <summary>
/// Provides access to the currently authenticated user's identity.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the currently authenticated user.
    /// </summary>
    Result<CurrentUser> Get();
}