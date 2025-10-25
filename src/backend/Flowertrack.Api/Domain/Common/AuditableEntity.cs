namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for entities that require audit tracking
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public abstract class AuditableEntity<TId> where TId : struct
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// Date and time when the entity was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Identifier of the user who created the entity
    /// </summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>
    /// Date and time when the entity was last updated (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Identifier of the user who last updated the entity
    /// </summary>
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// Collection of domain events raised by this entity
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events raised by this entity
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Sets the created audit information
    /// </summary>
    protected void SetCreatedAudit(Guid? createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Sets the updated audit information
    /// </summary>
    protected void SetUpdatedAudit(Guid? updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
