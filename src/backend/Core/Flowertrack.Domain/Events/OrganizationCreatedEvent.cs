using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when a new organization is created.
/// </summary>
public sealed class OrganizationCreatedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationCreatedEvent"/> class.
    /// </summary>
    /// <param name="organizationId">The unique identifier of the created organization.</param>
    /// <param name="name">The name of the organization.</param>
    public OrganizationCreatedEvent(Guid organizationId, string name)
    {
        OrganizationId = organizationId;
        Name = name;
    }

    /// <summary>
    /// Gets the unique identifier of the created organization.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the name of the organization.
    /// </summary>
    public string Name { get; }
}
