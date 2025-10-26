namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a service user account is activated.
/// This typically happens after account verification or approval.
/// </summary>
public sealed class ServiceUserActivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// When the account was activated
    /// </summary>
    public DateTimeOffset ActivatedAt { get; }

    /// <summary>
    /// User who activated the account (typically an administrator)
    /// </summary>
    public Guid ActivatedBy { get; }

    public ServiceUserActivatedEvent(
        Guid userId,
        DateTimeOffset activatedAt,
        Guid activatedBy)
        : base(userId)
    {
        UserId = userId;
        ActivatedAt = activatedAt;
        ActivatedBy = activatedBy;
    }
}
