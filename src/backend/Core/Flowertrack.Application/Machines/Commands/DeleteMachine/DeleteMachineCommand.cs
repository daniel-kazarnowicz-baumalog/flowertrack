using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.DeleteMachine;

/// <summary>
/// Command to soft delete a machine
/// </summary>
public sealed record DeleteMachineCommand(Guid MachineId) : IRequest<Result>;
