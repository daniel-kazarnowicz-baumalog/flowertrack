using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Domain event raised when a service user is created
/// </summary>
public class ServiceUserCreatedEvent : DomainEventBase
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public ServiceUserCreatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

/// <summary>
/// Domain event raised when a service user is activated
/// </summary>
public class ServiceUserActivatedEvent : DomainEventBase
{
    public Guid UserId { get; }

    public ServiceUserActivatedEvent(Guid userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// Domain event raised when a service user is deactivated
/// </summary>
public class ServiceUserDeactivatedEvent : DomainEventBase
{
    public Guid UserId { get; }
    public string Reason { get; }

    public ServiceUserDeactivatedEvent(Guid userId, string reason)
    {
        UserId = userId;
        Reason = reason;
    }
}

/// <summary>
/// Domain event raised when a service user's availability changes
/// </summary>
public class ServiceUserAvailabilityChangedEvent : DomainEventBase
{
    public Guid UserId { get; }
    public bool IsAvailable { get; }

    public ServiceUserAvailabilityChangedEvent(Guid userId, bool isAvailable)
    {
        UserId = userId;
        IsAvailable = isAvailable;
    }
}
