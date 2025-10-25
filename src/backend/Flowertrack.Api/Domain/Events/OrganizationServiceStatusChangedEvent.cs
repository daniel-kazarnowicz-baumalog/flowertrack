namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when an organization's service status changes.
/// This tracks changes in service level or subscription status.
/// </summary>
public sealed record OrganizationServiceStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the organization
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Previous service status
    /// </summary>
    public string OldStatus { get; init; }

    /// <summary>
    /// New service status
    /// </summary>
    public string NewStatus { get; init; }

    /// <summary>
    /// Reason for the status change
    /// </summary>
    public string Reason { get; init; }

    /// <summary>
    /// When the status was changed
    /// </summary>
    public DateTimeOffset ChangedAt { get; init; }

    public OrganizationServiceStatusChangedEvent(
        Guid organizationId,
        string oldStatus,
        string newStatus,
        string reason,
        DateTimeOffset changedAt)
        : base(organizationId)
    {
        OrganizationId = organizationId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedAt = changedAt;
    }
}
