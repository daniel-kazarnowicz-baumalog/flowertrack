using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachine;

public sealed record GetMachineQuery(Guid MachineId) : IRequest<Result<MachineDetailsDto>>;
