using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an organization's API key is generated or regenerated.
/// </summary>
public sealed class OrganizationApiKeyGeneratedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationApiKeyGeneratedEvent"/> class.
    /// </summary>
    /// <param name="organizationId">The unique identifier of the organization.</param>
    /// <param name="isRegeneration">Indicates whether this is a regeneration of an existing key.</param>
    public OrganizationApiKeyGeneratedEvent(Guid organizationId, bool isRegeneration)
    {
        OrganizationId = organizationId;
        IsRegeneration = isRegeneration;
    }

    /// <summary>
    /// Gets the unique identifier of the organization.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets a value indicating whether this is a regeneration of an existing key.
    /// </summary>
    public bool IsRegeneration { get; }
}
