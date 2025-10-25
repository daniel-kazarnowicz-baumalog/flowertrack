namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when an organization's service is suspended.
/// This typically happens due to non-payment, contract expiration, or policy violations.
/// </summary>
public sealed record OrganizationServiceSuspendedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the organization
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Reason for the service suspension
    /// </summary>
    public string SuspensionReason { get; init; }

    /// <summary>
    /// When the service was suspended
    /// </summary>
    public DateTimeOffset SuspendedAt { get; init; }

    /// <summary>
    /// User who suspended the service
    /// </summary>
    public Guid SuspendedBy { get; init; }

    public OrganizationServiceSuspendedEvent(
        Guid organizationId,
        string suspensionReason,
        DateTimeOffset suspendedAt,
        Guid suspendedBy)
        : base(organizationId)
    {
        OrganizationId = organizationId;
        SuspensionReason = suspensionReason;
        SuspendedAt = suspendedAt;
        SuspendedBy = suspendedBy;
    }
}
