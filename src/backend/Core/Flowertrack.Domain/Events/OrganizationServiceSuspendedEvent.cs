using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an organization's service is suspended.
/// </summary>
public sealed class OrganizationServiceSuspendedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationServiceSuspendedEvent"/> class.
    /// </summary>
    /// <param name="organizationId">The unique identifier of the organization.</param>
    /// <param name="reason">The reason for the suspension.</param>
    public OrganizationServiceSuspendedEvent(Guid organizationId, string reason)
    {
        OrganizationId = organizationId;
        Reason = reason;
    }

    /// <summary>
    /// Gets the unique identifier of the organization.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the reason for the suspension.
    /// </summary>
    public string Reason { get; }
}
