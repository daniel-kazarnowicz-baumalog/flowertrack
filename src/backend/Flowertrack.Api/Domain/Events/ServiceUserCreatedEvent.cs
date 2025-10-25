namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a new service user (technician or administrator) is created.
/// Service users are employees of the service company.
/// </summary>
public sealed record ServiceUserCreatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; init; }

    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    public ServiceUserCreatedEvent(
        Guid userId,
        string email,
        string fullName,
        DateTimeOffset createdAt)
        : base(userId)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        CreatedAt = createdAt;
    }
}
