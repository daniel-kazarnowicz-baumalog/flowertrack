namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a service user account is deactivated.
/// This happens when an employee leaves or access needs to be revoked.
/// </summary>
public sealed class ServiceUserDeactivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Reason for deactivating the account
    /// </summary>
    public string DeactivationReason { get; }

    /// <summary>
    /// When the account was deactivated
    /// </summary>
    public DateTimeOffset DeactivatedAt { get; }

    /// <summary>
    /// User who deactivated the account
    /// </summary>
    public Guid DeactivatedBy { get; }

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
