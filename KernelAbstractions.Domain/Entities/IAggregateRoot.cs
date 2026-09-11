using KernelAbstractions.Domain.Events;

namespace KernelAbstractions.Domain.Entities;

/// <summary>
/// Defines a contract for aggregate roots in Domain-Driven Design.
/// An aggregate root is the entry point for accessing and modifying a cluster
/// of related domain objects (the aggregate).
/// </summary>
/// <typeparam name="TId">
/// The type of the aggregate root's identifier.
/// Must be a value type (struct), such as <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing <see cref="IStronglyTypedId{TSelf, TValue}"/>.
/// </typeparam>
/// <remarks>
/// Only aggregate roots should be referenced directly by repositories.
/// All other entities within the aggregate must be accessed through the aggregate root.
/// </remarks>
public interface IAggregateRoot<TId> : IEntity<TId>
    where TId : struct
{
    /// <summary>
    /// Gets the collection of domain events raised by this aggregate root.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to be dispatched after the aggregate is persisted.
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from this aggregate root.
    /// </summary>
    void ClearDomainEvents();
}