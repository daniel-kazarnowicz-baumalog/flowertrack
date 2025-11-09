using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ScheduleMaintenance;

/// <summary>
/// Command to schedule maintenance for a machine
/// </summary>
public sealed record ScheduleMaintenanceCommand : IRequest<Result>
{
    public Guid MachineId { get; init; }
    public DateOnly ScheduledDate { get; init; }
    public int? IntervalDays { get; init; }
}
