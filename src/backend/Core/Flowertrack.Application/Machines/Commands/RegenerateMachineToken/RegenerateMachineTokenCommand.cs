using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.RegenerateMachineToken;

/// <summary>
/// Command to regenerate machine API token
/// </summary>
public sealed record RegenerateMachineTokenCommand(Guid MachineId, string Reason) : IRequest<Result<string>>;
