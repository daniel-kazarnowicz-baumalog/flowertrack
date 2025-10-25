namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for entities with audit fields
/// </summary>
/// <typeparam name="TId">Type of the entity identifier</typeparam>
public abstract class AuditableEntity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Unique identifier
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// When the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Who created the entity
    /// </summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>
    /// When the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; protected set; }

    /// <summary>
    /// Domain events raised by this entity
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Add a domain event
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear all domain events
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Set audit information for creation
    /// </summary>
    protected void SetCreatedAudit(string? createdBy = null)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Set audit information for update
    /// </summary>
    protected void SetUpdatedAudit(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
