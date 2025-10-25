namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for entities with audit tracking capabilities
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public abstract class AuditableEntity<TId> where TId : notnull
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets when the entity was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets who created the entity
    /// </summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>
    /// Gets or sets when the entity was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; protected set; }

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
    /// <param name="domainEvent">The domain event to add</param>
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
    /// Sets the audit information when the entity is created
    /// </summary>
    /// <param name="createdBy">The user who created the entity</param>
    protected void SetCreatedAudit(string? createdBy = null)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Sets the audit information when the entity is updated
    /// </summary>
    /// <param name="updatedBy">The user who updated the entity</param>
    protected void SetUpdatedAudit(string? updatedBy = null)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }
}
