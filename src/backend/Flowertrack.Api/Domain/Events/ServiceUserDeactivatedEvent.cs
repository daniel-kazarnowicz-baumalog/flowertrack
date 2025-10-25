namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a service user account is deactivated.
/// This happens when an employee leaves or access needs to be revoked.
/// </summary>
public sealed record ServiceUserDeactivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Reason for deactivating the account
    /// </summary>
    public string DeactivationReason { get; init; }

    /// <summary>
    /// When the account was deactivated
    /// </summary>
    public DateTimeOffset DeactivatedAt { get; init; }

    /// <summary>
    /// User who deactivated the account
    /// </summary>
    public Guid DeactivatedBy { get; init; }

    public ServiceUserDeactivatedEvent(
        Guid userId,
        string deactivationReason,
        DateTimeOffset deactivatedAt,
        Guid deactivatedBy)
        : base(userId)
    {
        UserId = userId;
        DeactivationReason = deactivationReason;
        DeactivatedAt = deactivatedAt;
        DeactivatedBy = deactivatedBy;
    }
}
