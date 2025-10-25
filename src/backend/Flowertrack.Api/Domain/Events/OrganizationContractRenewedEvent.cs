namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when an organization's service contract is renewed.
/// This extends the service period and updates contract terms.
/// </summary>
public sealed record OrganizationContractRenewedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the organization
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Previous contract end date
    /// </summary>
    public DateTimeOffset OldEndDate { get; init; }

    /// <summary>
    /// New contract end date after renewal
    /// </summary>
    public DateTimeOffset NewEndDate { get; init; }

    /// <summary>
    /// When the contract was renewed
    /// </summary>
    public DateTimeOffset RenewedAt { get; init; }

    /// <summary>
    /// User who processed the renewal
    /// </summary>
    public Guid RenewedBy { get; init; }

    public OrganizationContractRenewedEvent(
        Guid organizationId,
        DateTimeOffset oldEndDate,
        DateTimeOffset newEndDate,
        DateTimeOffset renewedAt,
        Guid renewedBy)
        : base(organizationId)
    {
        OrganizationId = organizationId;
        OldEndDate = oldEndDate;
        NewEndDate = newEndDate;
        RenewedAt = renewedAt;
        RenewedBy = renewedBy;
    }
}
