using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ClearAlarm;

public sealed record ClearAlarmCommand(Guid MachineId, string Reason) : IRequest<Result>;
