namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for entities that require audit information
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    protected void SetCreatedAudit(Guid userId)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = userId;
    }

    protected void SetUpdatedAudit(Guid userId)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }
}
