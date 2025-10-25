namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a service user account is activated.
/// This typically happens after account verification or approval.
/// </summary>
public sealed record ServiceUserActivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// When the account was activated
    /// </summary>
    public DateTimeOffset ActivatedAt { get; init; }

    /// <summary>
    /// User who activated the account (typically an administrator)
    /// </summary>
    public Guid ActivatedBy { get; init; }

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
