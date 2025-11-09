using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.CompleteMaintenance;

public sealed record CompleteMaintenanceCommand(Guid MachineId, DateOnly CompletedDate, int? IntervalDays) : IRequest<Result>;
