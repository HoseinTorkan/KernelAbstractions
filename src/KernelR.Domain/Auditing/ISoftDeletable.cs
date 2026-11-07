using KernelR.Domain.StronglyTypedIds;

namespace KernelR.Domain.Auditing;

/// <summary>
/// Defines a contract for entities that support soft deletion.
/// A soft-deleted entity is marked as deleted instead of being physically removed.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who performed the soft deletion.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing
/// <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// <para>
/// This interface is separated from creation and modification auditing to
/// comply with the Interface Segregation Principle. Entities that do not require
/// soft deletion (such as immutable logs) do not need this interface.
/// </para>
/// <para>
/// <see cref="DeletedAt"/> and <see cref="DeletedBy"/> are nullable and are
/// expected to be set (and cleared) together: an entity is either "not deleted"
/// (both are <see langword="null"/>) or "soft-deleted" (both are non-null).
/// <see cref="IsDeleted"/> is derived from <see cref="DeletedAt"/>, so it is
/// always consistent with the audit fields.
/// </para>
/// <para>
/// Implementations must ensure that soft-deleted entities are excluded from
/// normal queries (for example, via a global query filter).
/// </para>
/// <para>
/// <see cref="DeletedAt"/> is expressed as a <see cref="DateTimeOffset"/> so that
/// the instant is unambiguous regardless of the storage or display time zone.
/// Implementations should persist it in UTC (offset <c>+00:00</c>).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class Product : IGuidEntity&lt;ProductId&gt;, ISoftDeletable&lt;UserId&gt;
/// {
///     public ProductId Id { get; private set; }
///     public DateTimeOffset? DeletedAt { get; private set; }
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
    /// Gets the moment when this entity was soft-deleted.
    /// Returns <see langword="null"/> if the entity has not been deleted.
    /// </summary>
    /// <remarks>
    /// When this property is non-null, <see cref="DeletedBy"/> is guaranteed to be
    /// non-null as well. Implementations should persist this value in UTC.
    /// </remarks>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who soft-deleted this entity.
    /// Returns <see langword="null"/> if the entity has not been deleted.
    /// </summary>
    /// <remarks>
    /// When this property is non-null, <see cref="DeletedAt"/> is guaranteed to be
    /// non-null as well.
    /// </remarks>
    TUserId? DeletedBy { get; }

    /// <summary>
    /// Gets a value indicating whether this entity has been soft-deleted.
    /// </summary>
    /// <remarks>
    /// Derived from <see cref="DeletedAt"/>; an entity is considered deleted
    /// if and only if <see cref="DeletedAt"/> is non-null.
    /// </remarks>
    bool IsDeleted => DeletedAt is not null;
}
