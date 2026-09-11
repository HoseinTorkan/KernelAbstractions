using KernelAbstractions.Domain.Auditing;
using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Auditig;

/// <summary>
/// Defines a contract for entities that require tracking of their creation.
/// Captures both the timestamp of creation and the identity of the user who created the entity.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who created the entity.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// This interface is separated from modification and deletion auditing
/// to comply with the Interface Segregation Principle. Entities that are immutable
/// after creation (such as audit logs or immutable snapshots) only need this interface.
/// All timestamps are expected to be stored in UTC.
/// </remarks>
/// <example>
/// <code>
/// public class AuditLog : IGuidEntity&lt;AuditLogId&gt;, ICreationAudited&lt;UserId&gt;
/// {
///     public AuditLogId Id { get; private set; }
///     public DateTime CreatedAt { get; private set; }
///     public UserId CreatedBy { get; private set; }
/// }
/// </code>
/// </example>
/// <seealso cref="IModificationAudited{TUserId}"/>
/// <seealso cref="ISoftDeletable{TUserId}"/>
public interface ICreationAudited<TUserId>
    where TUserId : struct
{
    /// <summary>
    /// Gets the date and time when this entity was created (in UTC).
    /// </summary>
    /// <remarks>
    /// Set exactly once at instantiation and never modified afterwards.
    /// </remarks>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who created this entity.
    /// </summary>
    /// <remarks>
    /// Set exactly once at instantiation and never modified afterwards.
    /// </remarks>
    TUserId CreatedBy { get; }
}