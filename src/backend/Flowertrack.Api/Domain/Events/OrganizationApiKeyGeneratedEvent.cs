using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when an API key is generated for an organization
/// </summary>
public sealed class OrganizationApiKeyGeneratedEvent : DomainEvent
{
    /// <summary>
    /// Gets the organization identifier
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets a value indicating whether this is a regeneration of an existing key
    /// </summary>
    public bool IsRegeneration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationApiKeyGeneratedEvent"/> class
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="isRegeneration">Whether this is a regeneration</param>
    public OrganizationApiKeyGeneratedEvent(Guid organizationId, bool isRegeneration)
    {
        OrganizationId = organizationId;
        IsRegeneration = isRegeneration;
    }
}
