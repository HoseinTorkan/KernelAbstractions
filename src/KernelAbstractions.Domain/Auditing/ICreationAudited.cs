using KernelAbstractions.Domain.StronglyTypedIds;

namespace KernelAbstractions.Domain.Auditing;

/// <summary>
/// Defines a contract for entities that require tracking of their creation.
/// Captures both the moment of creation and the identity of the user who
/// created the entity.
/// </summary>
/// <typeparam name="TUserId">
/// The type of the identifier of the user who created the entity.
/// Must be a value type (e.g., <see cref="Guid"/>, <see cref="long"/>,
/// or a strongly-typed identifier implementing
/// <see cref="IStronglyTypedId{TSelf, TValue}"/>).
/// </typeparam>
/// <remarks>
/// <para>
/// This interface is separated from modification and deletion auditing to
/// comply with the Interface Segregation Principle. Entities that are
/// immutable after creation (such as audit logs or immutable snapshots)
/// only need this interface.
/// </para>
/// <para>
/// <see cref="CreatedAt"/> is expressed as a <see cref="DateTimeOffset"/> so
/// that the instant is unambiguous regardless of the storage or display
/// time zone, and so that comparisons are always correct.
/// Implementations should persist it in UTC (offset <c>+00:00</c>) and let
/// the presentation layer convert it as needed.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class AuditLog : IGuidEntity&lt;AuditLogId&gt;, ICreationAudited&lt;UserId&gt;
/// {
///     public AuditLogId Id { get; private set; }
///     public DateTimeOffset CreatedAt { get; private set; }
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
    /// Gets the moment when this entity was created.
    /// </summary>
    /// <remarks>
    /// Set exactly once at instantiation and never modified afterwards.
    /// Implementations should persist this value in UTC.
    /// </remarks>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who created this entity.
    /// </summary>
    /// <remarks>
    /// Set exactly once at instantiation and never modified afterwards.
    /// </remarks>
    TUserId CreatedBy { get; }
}
