namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when an organization user activates their account
/// </summary>
public sealed class OrganizationUserActivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Organization the user belongs to
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// When the user activated their account
    /// </summary>
    public DateTimeOffset ActivatedAt { get; }

    public OrganizationUserActivatedEvent(
        Guid userId,
        Guid organizationId,
        DateTimeOffset activatedAt)
    {
        UserId = userId;
        OrganizationId = organizationId;
        ActivatedAt = activatedAt;
    }
}
