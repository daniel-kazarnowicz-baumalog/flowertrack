using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an organization is deleted (soft delete).
/// </summary>
public sealed class OrganizationDeletedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationDeletedEvent"/> class.
    /// </summary>
    /// <param name="organizationId">The unique identifier of the deleted organization.</param>
    /// <param name="name">The name of the organization.</param>
    public OrganizationDeletedEvent(Guid organizationId, string name)
    {
        OrganizationId = organizationId;
        Name = name;
    }

    /// <summary>
    /// Gets the unique identifier of the deleted organization.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the name of the organization.
    /// </summary>
    public string Name { get; }
}
