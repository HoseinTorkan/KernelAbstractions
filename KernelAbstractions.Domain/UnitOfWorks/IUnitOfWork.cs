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
/// infrastructure layer (e.g., using Entity Framework Core's DbContext).
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
    /// This method is expected to commit all changes as a single transaction.
    /// If any change fails, the entire operation is rolled back.
    /// </remarks>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}