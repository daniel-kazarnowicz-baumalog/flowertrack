namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for entities with audit information
/// </summary>
public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    protected AuditableEntity(TId id) : base(id)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; protected set; }

    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; protected set; }

    /// <summary>
    /// Sets creation timestamp
    /// </summary>
    protected void SetCreatedAt(DateTimeOffset timestamp)
    {
        CreatedAt = timestamp;
        UpdatedAt = timestamp;
    }

    /// <summary>
    /// Updates the modification timestamp
    /// </summary>
    protected void SetUpdatedAt(DateTimeOffset timestamp)
    {
        UpdatedAt = timestamp;
    }
}
