namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when an organization user's role is changed.
/// </summary>
public sealed class OrganizationUserRoleChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Previous role of the user
    /// </summary>
    public string OldRole { get; }

    /// <summary>
    /// New role of the user
    /// </summary>
    public string NewRole { get; }

    /// <summary>
    /// Date and time when the role change occurred (UTC)
    /// </summary>
    public DateTimeOffset ChangedAt { get; }

    /// <summary>
    /// User who performed the role change
    /// </summary>
    public Guid ChangedBy { get; }

    public OrganizationUserRoleChangedEvent(
        Guid userId,
        string oldRole,
        string newRole,
        DateTimeOffset changedAt,
        Guid changedBy)
    {
        UserId = userId;
        OldRole = oldRole;
        NewRole = newRole;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
    }
}
