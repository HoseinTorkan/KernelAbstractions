using KernelAbstractions.Domain.Entities;

namespace KernelAbstractions.Domain.Repositories;

/// <summary>
/// Defines a generic contract for repositories that manage the persistence
/// of aggregate roots with a unique identifier.
/// </summary>
/// <typeparam name="TId">
/// The type of the aggregate root's identifier. Must be a value type (struct).
/// </typeparam>
/// <typeparam name="TAggregateRoot">
/// The type of the aggregate root managed by this repository.
/// Must implement <see cref="IAggregateRoot{TId}"/>.
/// </typeparam>
/// <remarks>
/// In Domain-Driven Design, only aggregate roots are allowed to have repositories.
/// Other entities within an aggregate must be accessed through their aggregate root.
/// This interface is intentionally generic and only covers the most common operations.
/// More specific queries should be defined in dedicated repository interfaces
/// (e.g., <c>IOrderRepository</c>) in the domain layer.
/// </remarks>
/// <seealso cref="IAggregateRoot{TId}"/>
public interface IRepository<TId, TAggregateRoot>
    where TId : struct
    where TAggregateRoot : IAggregateRoot<TId>
{
    /// <summary>
    /// Checks whether an aggregate root with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if an aggregate root with the specified identifier exists; otherwise, <c>false</c>.</returns>
    Task<bool> AnyAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the aggregate root with the specified identifier, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The identifier of the aggregate root to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// The aggregate root with the specified identifier, or <c>null</c> if no such aggregate root exists.
    /// </returns>
    Task<TAggregateRoot?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new aggregate root to the repository.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate root to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of new aggregate roots to the repository.
    /// </summary>
    /// <param name="aggregateRoots">The aggregate roots to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddRangeAsync(IEnumerable<TAggregateRoot> aggregateRoots, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing aggregate root as modified.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate root to update.</param>
    void Update(TAggregateRoot aggregateRoot);

    /// <summary>
    /// Marks an existing aggregate root for deletion.
    /// </summary>
    /// <param name="aggregateRoot">The aggregate root to delete.</param>
    void Delete(TAggregateRoot aggregateRoot);

    /// <summary>
    /// Marks a collection of existing aggregate roots for deletion.
    /// </summary>
    /// <param name="aggregateRoots">The aggregate roots to delete.</param>
    void DeleteRange(IEnumerable<TAggregateRoot> aggregateRoots);
}