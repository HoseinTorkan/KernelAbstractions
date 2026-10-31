using KernelAbstractions.Domain.Repositories;

namespace KernelAbstractions.Domain.UnitOfWorks;

/// <summary>
/// Defines a contract for the Unit of Work pattern, which coordinates the
/// persistence of changes made to aggregate roots within a single transaction.
/// </summary>
/// <remarks>
/// <para>
/// The Unit of Work pattern maintains a list of objects affected by a business
/// transaction and coordinates the writing out of changes. It ensures that all
/// changes are committed together or none at all, preserving data consistency.
/// </para>
/// <para>
/// In Domain-Driven Design, a Unit of Work typically spans a single application
/// use case (command). Changes made to multiple aggregate roots within that use
/// case are committed atomically.
/// </para>
/// <para>
/// The implementation of this interface is expected to be provided by the
/// infrastructure layer (e.g., using Entity Framework Core's
/// <c>DbContext</c>), and is typically registered with a scoped lifetime so
/// that all repositories share the same unit of work.
/// </para>
/// <para>
/// Domain events raised by aggregate roots are not dispatched by this method;
/// dispatch belongs to the persistence layer (for example, an EF Core
/// interceptor or a domain-event dispatcher invoked after
/// <see cref="SaveChangesAsync"/>).
/// </para>
/// </remarks>
/// <seealso cref="IRepository{TId, TAggregateRoot}"/>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes made within the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// The number of state entries written to the underlying data store.
    /// </returns>
    /// <remarks>
    /// In most implementations (e.g. EF Core), this method implicitly wraps the
    /// pending changes in a single transaction and commits them atomically.
    /// If the underlying store does not support implicit transactions, the
    /// implementation must ensure that all changes are applied atomically.
    /// </remarks>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
