using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.ValueObjects;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ChangeMachineStatus;

/// <summary>
/// Command to change machine operational status
/// </summary>
public sealed record ChangeMachineStatusCommand : IRequest<Result>
{
    public Guid MachineId { get; init; }
    public MachineStatus NewStatus { get; init; }
    public string Reason { get; init; } = string.Empty;
}
