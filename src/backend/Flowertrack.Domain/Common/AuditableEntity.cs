namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for entities that need audit tracking.
/// Provides standard audit fields for creation and update tracking.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    protected AuditableEntity(TId id) : base(id)
    {
    }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// Sets the audit information for entity creation.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the entity.</param>
    protected void SetCreatedAudit(Guid userId)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = userId;
    }

    /// <summary>
    /// Sets the audit information for entity update.
    /// </summary>
    /// <param name="userId">The identifier of the user updating the entity.</param>
    protected void SetUpdatedAudit(Guid userId)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = userId;
    }
}
