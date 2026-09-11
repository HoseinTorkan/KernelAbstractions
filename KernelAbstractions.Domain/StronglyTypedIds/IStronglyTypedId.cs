using KernelAbstractions.Domain.Entities;

namespace KernelAbstractions.Domain.StronglyTypedIds;

/// <summary>
/// Defines a contract for strongly-typed identifiers.
/// A strongly-typed identifier wraps a primitive value (such as <see cref="Guid"/> or <see cref="long"/>)
/// into a dedicated type to provide compile-time type safety and prevent accidental misuse
/// of identifiers across different entities.
/// </summary>
/// <typeparam name="TSelf">
/// The self-referencing type of the identifier itself (e.g., ProductId, OrderId).
/// Enables the Curiously Recurring Template Pattern (CRTP) to distinguish between
/// different identifiers at compile time.
/// </typeparam>
/// <typeparam name="TValue">
/// The underlying primitive type used to store the identifier value.
/// Must be a value type (struct), such as <see cref="Guid"/> or <see cref="long"/>.
/// </typeparam>
/// <remarks>
/// This interface defines only the minimal requirement: exposing the underlying value.
/// Factory methods such as New, From, and Create are intentionally excluded and must be
/// defined by each identifier type individually.
/// </remarks>
/// <seealso cref="IEntity{TId}"/>
public interface IStronglyTypedId<TSelf, TValue>
    where TSelf : IStronglyTypedId<TSelf, TValue>
    where TValue : struct
{
    /// <summary>
    /// Gets the underlying primitive value of the strongly-typed identifier.
    /// </summary>
    TValue Value { get; }
}