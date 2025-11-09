using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachines;

public sealed record GetMachinesQuery : IRequest<Result<List<MachineDto>>>
{
    public Guid? OrganizationId { get; init; }
    public string? Status { get; init; }
    public string? SearchTerm { get; init; }
}
