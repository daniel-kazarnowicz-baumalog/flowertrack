using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when an organization is created
/// </summary>
public sealed class OrganizationCreatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the organization identifier
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the organization name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the organization email
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationCreatedEvent"/> class
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="name">The organization name</param>
    /// <param name="email">The organization email</param>
    public OrganizationCreatedEvent(Guid organizationId, string name, string email)
    {
        OrganizationId = organizationId;
        Name = name;
        Email = email;
    }
}
