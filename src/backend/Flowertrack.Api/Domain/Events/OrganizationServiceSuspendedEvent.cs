using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when an organization's service is suspended
/// </summary>
public sealed class OrganizationServiceSuspendedEvent : DomainEvent
{
    /// <summary>
    /// Gets the organization identifier
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the reason for suspension
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationServiceSuspendedEvent"/> class
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="reason">The reason for suspension</param>
    public OrganizationServiceSuspendedEvent(Guid organizationId, string reason)
    {
        OrganizationId = organizationId;
        Reason = reason;
    }
}
