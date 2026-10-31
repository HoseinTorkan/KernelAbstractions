using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Auditing;

/// <summary>
/// Defines a contract for entities that require tracking of their last modification.
/// Captures both the moment of the most recent update and the identity of the
/// user who performed it.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who modified the entity.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing
/// <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// <para>
/// This interface is separated from creation and deletion auditing to comply
/// with the Interface Segregation Principle. Entities that are never modified
/// after creation do not need this interface.
/// </para>
/// <para>
/// Both properties are nullable because an entity may not have been modified yet.
/// They are expected to be set (and cleared) together: an entity is either
/// "never modified" (both are <see langword="null"/>) or "modified at least once"
/// (both are non-null).
/// </para>
/// <para>
/// <see cref="UpdatedAt"/> is expressed as a <see cref="DateTimeOffset"/> so that
/// the instant is unambiguous regardless of the storage or display time zone.
/// Implementations should persist it in UTC (offset <c>+00:00</c>).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class User : IGuidEntity&lt;UserId&gt;, IModificationAudited&lt;UserId&gt;
/// {
///     public UserId Id { get; private set; }
///     public DateTimeOffset? UpdatedAt { get; private set; }
///     public UserId? UpdatedBy { get; private set; }
/// }
/// </code>
/// </example>
/// <seealso cref="ICreationAudited{TUserId}"/>
/// <seealso cref="ISoftDeletable{TUserId}"/>
public interface IModificationAudited<TUserId>
    where TUserId : struct
{
    /// <summary>
    /// Gets the moment when this entity was last updated.
    /// Returns <see langword="null"/> if the entity has never been modified.
    /// </summary>
    /// <remarks>
    /// When this property is non-null, <see cref="UpdatedBy"/> is guaranteed to be
    /// non-null as well. Implementations should persist this value in UTC.
    /// </remarks>
    DateTimeOffset? UpdatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who last updated this entity.
    /// Returns <see langword="null"/> if the entity has never been modified.
    /// </summary>
    /// <remarks>
    /// When this property is non-null, <see cref="UpdatedAt"/> is guaranteed to be
    /// non-null as well.
    /// </remarks>
    TUserId? UpdatedBy { get; }
}
