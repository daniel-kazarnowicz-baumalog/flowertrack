using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a new machine is registered in the system.
/// This event captures the initial registration of production equipment.
/// </summary>
public sealed class MachineRegisteredEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// Organization that owns this machine
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Serial number of the machine (unique identifier from manufacturer)
    /// </summary>
    public string SerialNumber { get; }

    /// <summary>
    /// Brand/manufacturer of the machine
    /// </summary>
    public string Brand { get; }

    /// <summary>
    /// Model identifier of the machine
    /// </summary>
    public string Model { get; }

    public MachineRegisteredEvent(
        Guid machineId,
        Guid organizationId,
        string serialNumber,
        string brand,
        string model)
        : base(machineId)
    {
        MachineId = machineId;
        OrganizationId = organizationId;
        SerialNumber = serialNumber;
        Brand = brand;
        Model = model;
    }
}
