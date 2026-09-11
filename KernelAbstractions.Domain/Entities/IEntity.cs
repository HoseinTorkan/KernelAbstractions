using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Entities;

/// <summary>
/// Defines a contract for entities that have a unique identifier.
/// An entity is an object that is defined by its identity rather than its attributes.
/// </summary>
/// <typeparam name="TId">
/// The type of the entity's identifier. Must be a value type (struct).
/// Common choices include <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing <see cref="IStronglyTypedId{TSelf, TValue}"/>.
/// </typeparam>
/// <remarks>
/// This interface uses a minimal constraint (<c>struct</c>) to provide flexibility
/// across different persistence technologies and legacy systems.
/// While any value type is permitted, it is strongly recommended to use a
/// strongly-typed identifier (such as <c>ProductId</c> or <c>OrderId</c>) to avoid
/// primitive obsession and improve type safety.
/// </remarks>
/// <example>
/// Using a strongly-typed identifier (recommended):
/// <code>
/// public class Product : IEntity&lt;ProductId&gt;
/// {
///     public ProductId Id { get; private set; }
/// }
/// </code>
/// Using a raw primitive (acceptable in legacy scenarios):
/// <code>
/// public class LegacyEntity : IEntity&lt;int&gt;
/// {
///     public int Id { get; private set; }
/// }
/// </code>
/// </example>
/// <seealso cref="IAggregateRoot{TId}"/>
/// <seealso cref="IStronglyTypedId{TSelf, TValue}"/>
public interface IEntity<TId>
    where TId : struct
{
    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    /// <value>
    /// The identifier value that uniquely identifies this entity.
    /// </value>
    /// <remarks>
    /// The identifier is expected to be immutable after the entity is created.
    /// Implementations should set this property only during construction.
    /// </remarks>
    TId Id { get; }
}