using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.RegisterMachine;

/// <summary>
/// Command to register a new machine for an organization
/// US-027: Zarządzanie maszynami w organizacji
/// </summary>
public sealed record RegisterMachineCommand : IRequest<Result<Guid>>
{
    public Guid OrganizationId { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Location { get; init; }
}
