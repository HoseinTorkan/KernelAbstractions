using KernelAbstractions.Domain.Auditig;
using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Auditing;

/// <summary>
/// Defines a contract for entities that support soft deletion.
/// A soft-deleted entity is marked as deleted instead of being physically removed.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who performed the soft deletion.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// This interface is separated from creation and modification auditing
/// to comply with the Interface Segregation Principle. Entities that do not require
/// soft deletion (such as immutable logs) do not need this interface.
/// Implementations must ensure that soft-deleted entities are excluded from normal queries.
/// All timestamps are expected to be stored in UTC.
/// </remarks>
/// <example>
/// <code>
/// public class Product : IGuidEntity&lt;ProductId&gt;, ISoftDeletable&lt;UserId&gt;
/// {
///     public ProductId Id { get; private set; }
///     public bool IsDeleted { get; private set; }
///     public DateTime? DeletedAt { get; private set; }
///     public UserId? DeletedBy { get; private set; }
/// }
/// </code>
/// </example>
/// <seealso cref="ICreationAudited{TUserId}"/>
/// <seealso cref="IModificationAudited{TUserId}"/>
public interface ISoftDeletable<TUserId>
    where TUserId : struct
{
    /// <summary>
    /// Gets a value indicating whether this entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the date and time when this entity was soft-deleted (in UTC).
    /// Returns <c>null</c> if the entity has not been deleted.
    /// </summary>
    DateTime? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who soft-deleted this entity.
    /// Returns <c>null</c> if the entity has not been deleted.
    /// </summary>
    TUserId? DeletedBy { get; }
}