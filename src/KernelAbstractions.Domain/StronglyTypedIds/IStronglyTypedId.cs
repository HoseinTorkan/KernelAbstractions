using KernelAbstractions.Domain.Entities;

namespace KernelAbstractions.Domain.StronglyTypedIds;

/// <summary>
/// Defines a contract for strongly-typed identifiers.
/// A strongly-typed identifier wraps a primitive value (such as
/// <see cref="Guid"/> or <see cref="long"/>) into a dedicated type to provide
/// compile-time type safety and prevent accidental misuse of identifiers
/// across different entities.
/// </summary>
/// <typeparam name="TSelf">
/// The self-referencing type of the identifier itself (e.g., ProductId, OrderId).
/// Enables the Curiously Recurring Template Pattern (CRTP) to distinguish between
/// different identifiers at compile time.
/// Must be a value type to keep identifiers allocation-free and cheap to compare.
/// </typeparam>
/// <typeparam name="TValue">
/// The underlying primitive type used to store the identifier value.
/// Must be a value type (struct) that implements
/// <see cref="IEquatable{T}"/>, such as <see cref="Guid"/> or
/// <see cref="long"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This interface defines only the minimal requirement: exposing the underlying
/// value. Factory methods such as New, From, and Create are intentionally
/// excluded and must be defined by each identifier type individually.
/// </para>
/// <para>
/// Implementations are expected to provide value equality based on
/// <see cref="Value"/>; deriving from a <c>record struct</c> or a shared
/// base type is the recommended way to achieve this.
/// </para>
/// <para>
/// Because <typeparamref name="TSelf"/> is constrained to <c>struct</c>,
/// identifiers are allocation-free and benefit from value semantics.
/// Note that this also means <c>default(TSelf)</c> is a valid value; if an
/// all-zero identifier must be rejected, the identifier type itself should
/// enforce that in its constructor or factory methods.
/// </para>
/// </remarks>
/// <seealso cref="IEntity{TId}"/>
public interface IStronglyTypedId<TSelf, TValue>
    where TSelf : struct, IStronglyTypedId<TSelf, TValue>
    where TValue : struct, IEquatable<TValue>
{
    /// <summary>
    /// Gets the underlying primitive value of the strongly-typed identifier.
    /// </summary>
    TValue Value { get; }
}
