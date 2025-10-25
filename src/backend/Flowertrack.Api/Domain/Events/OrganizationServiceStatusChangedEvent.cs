using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Enums;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when an organization's service status changes
/// </summary>
public sealed class OrganizationServiceStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Gets the organization identifier
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the previous service status
    /// </summary>
    public ServiceStatus PreviousStatus { get; }

    /// <summary>
    /// Gets the new service status
    /// </summary>
    public ServiceStatus NewStatus { get; }

    /// <summary>
    /// Gets the reason for the status change
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationServiceStatusChangedEvent"/> class
    /// </summary>
    /// <param name="organizationId">The organization identifier</param>
    /// <param name="previousStatus">The previous service status</param>
    /// <param name="newStatus">The new service status</param>
    /// <param name="reason">The reason for the status change</param>
    public OrganizationServiceStatusChangedEvent(
        Guid organizationId,
        ServiceStatus previousStatus,
        ServiceStatus newStatus,
        string reason)
    {
        OrganizationId = organizationId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}
