namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a new organization user (client user) is created.
/// Organization users are employees of client companies (operators or administrators).
/// </summary>
public sealed record OrganizationUserCreatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Organization the user belongs to
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Role of the user within the organization (e.g., Admin, Operator)
    /// </summary>
    public string Role { get; init; }

    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    public OrganizationUserCreatedEvent(
        Guid userId,
        Guid organizationId,
        string email,
        string role,
        DateTimeOffset createdAt)
        : base(userId)
    {
        UserId = userId;
        OrganizationId = organizationId;
        Email = email;
        Role = role;
        CreatedAt = createdAt;
    }
}
