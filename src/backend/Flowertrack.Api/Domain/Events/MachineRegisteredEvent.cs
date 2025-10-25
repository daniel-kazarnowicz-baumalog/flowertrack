using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when a new machine is registered in the system
/// </summary>
public sealed record MachineRegisteredEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public Guid OrganizationId { get; init; }
    public string SerialNumber { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }

    public MachineRegisteredEvent(Guid machineId, Guid organizationId, string serialNumber, string? brand, string? model)
    {
        MachineId = machineId;
        OrganizationId = organizationId;
        SerialNumber = serialNumber;
        Brand = brand;
        Model = model;
    }
}
