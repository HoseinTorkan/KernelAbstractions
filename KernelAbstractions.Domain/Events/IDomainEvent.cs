using KernelAbstractions.Domain.Entities;

namespace KernelAbstractions.Domain.Events;

/// <summary>
/// Defines a contract for domain events in Domain-Driven Design.
/// A domain event represents something meaningful that happened in the domain.
/// </summary>
/// <remarks>
/// Domain events are typically raised by aggregate roots and dispatched after
/// the aggregate has been successfully persisted. All timestamps are expected
/// to be stored in UTC.
/// </remarks>
/// <example>
/// <code>
/// public sealed record OrderCreatedEvent(OrderId OrderId, UserId CustomerId) : IDomainEvent
/// {
///     public DateTime OccurredOn { get; } = DateTime.UtcNow;
/// }
/// </code>
/// </example>
/// <seealso cref="IAggregateRoot{TId}"/>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when this event occurred (in UTC).
    /// </summary>
    DateTime OccurredOn { get; }
}