using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.UpdateMachine;

/// <summary>
/// Command to update machine information
/// </summary>
public sealed record UpdateMachineCommand : IRequest<Result>
{
    public Guid MachineId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Location { get; init; }
}
