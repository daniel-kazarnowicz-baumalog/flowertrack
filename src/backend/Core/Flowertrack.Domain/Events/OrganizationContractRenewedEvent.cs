using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an organization's contract is renewed.
/// </summary>
public sealed class OrganizationContractRenewedEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrganizationContractRenewedEvent"/> class.
    /// </summary>
    /// <param name="organizationId">The unique identifier of the organization.</param>
    /// <param name="previousEndDate">The previous contract end date.</param>
    /// <param name="newEndDate">The new contract end date.</param>
    public OrganizationContractRenewedEvent(
        Guid organizationId,
        DateTimeOffset? previousEndDate,
        DateTimeOffset newEndDate)
    {
        OrganizationId = organizationId;
        PreviousEndDate = previousEndDate;
        NewEndDate = newEndDate;
    }

    /// <summary>
    /// Gets the unique identifier of the organization.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Gets the previous contract end date.
    /// </summary>
    public DateTimeOffset? PreviousEndDate { get; }

    /// <summary>
    /// Gets the new contract end date.
    /// </summary>
    public DateTimeOffset NewEndDate { get; }
}
