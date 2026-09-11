using KernelAbstractions.Domain.Auditig;
using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Auditing;

/// <summary>
/// Defines a contract for entities that require tracking of their last modification.
/// Captures both the timestamp of the most recent update and the identity of the user who performed it.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who modified the entity.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// This interface is separated from creation and deletion auditing
/// to comply with the Interface Segregation Principle. Entities that are never modified
/// after creation do not need this interface.
/// Both properties are nullable because an entity may not have been modified yet.
/// All timestamps are expected to be stored in UTC.
/// </remarks>
/// <example>
/// <code>
/// public class User : IGuidEntity&lt;UserId&gt;, IModificationAudited&lt;UserId&gt;
/// {
///     public UserId Id { get; private set; }
///     public DateTime? UpdatedAt { get; private set; }
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
    /// Gets the date and time when this entity was last updated (in UTC).
    /// Returns <c>null</c> if never modified.
    /// </summary>
    DateTime? UpdatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who last updated this entity.
    /// Returns <c>null</c> if never modified.
    /// </summary>
    TUserId? UpdatedBy { get; }
}