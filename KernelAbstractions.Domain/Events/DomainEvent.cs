namespace KernelAbstractions.Domain.Events;

/// <summary>
/// Provides a base implementation of <see cref="IDomainEvent"/> that
/// supplies a UTC timestamp (<see cref="OccurredOn"/>) automatically.
/// </summary>
/// <remarks>
/// Inherit from this record to avoid repeating the boilerplate of
/// capturing the occurrence time in every domain event.
/// </remarks>
/// <example>
/// <code>
/// public sealed record UserCreatedEvent(
///     UserId UserId,
///     PhoneNumber PhoneNumber) : DomainEvent;
/// </code>
/// </example>
/// <seealso cref="IDomainEvent"/>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}