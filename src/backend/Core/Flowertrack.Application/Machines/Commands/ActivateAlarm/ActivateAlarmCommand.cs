using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ActivateAlarm;

public sealed record ActivateAlarmCommand(Guid MachineId, string Reason) : IRequest<Result>;
