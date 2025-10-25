using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when an organization user is created
/// </summary>
public class OrganizationUserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Guid OrganizationId { get; }

    public OrganizationUserCreatedEvent(Guid userId, string email, string firstName, string lastName, Guid organizationId)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        OrganizationId = organizationId;
    }
}

/// <summary>
/// Domain event raised when an organization user is activated
/// </summary>
public class OrganizationUserActivatedEvent : DomainEvent
{
    public Guid UserId { get; }

    public OrganizationUserActivatedEvent(Guid userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// Domain event raised when an organization user is deactivated
/// </summary>
public class OrganizationUserDeactivatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Reason { get; }

    public OrganizationUserDeactivatedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
    }
}

/// <summary>
/// Domain event raised when an organization user's role changes
/// </summary>
public class OrganizationUserRoleChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string OldRole { get; }
    public string NewRole { get; }

    public OrganizationUserRoleChangedEvent(Guid userId, string oldRole, string newRole)
    {
        UserId = userId;
        OldRole = oldRole;
        NewRole = newRole;
    }
}
