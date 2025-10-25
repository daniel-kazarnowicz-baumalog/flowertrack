namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a new organization (client company) is created in the system.
/// This marks the onboarding of a new client.
/// </summary>
public sealed record OrganizationCreatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the organization
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Name of the organization
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Initial service status (e.g., Active, Trial)
    /// </summary>
    public string ServiceStatus { get; init; }

    /// <summary>
    /// User who created the organization (typically a service administrator)
    /// </summary>
    public Guid CreatedBy { get; init; }

    public OrganizationCreatedEvent(
        Guid organizationId,
        string name,
        string serviceStatus,
        Guid createdBy)
        : base(organizationId)
    {
        OrganizationId = organizationId;
        Name = name;
        ServiceStatus = serviceStatus;
        CreatedBy = createdBy;
    }
}
