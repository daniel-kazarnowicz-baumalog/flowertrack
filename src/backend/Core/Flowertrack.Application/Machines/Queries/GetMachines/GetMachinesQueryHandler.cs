using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachines;

public sealed class GetMachinesQueryHandler : IRequestHandler<GetMachinesQuery, Result<List<MachineDto>>>
{
    private readonly IMachineRepository _machineRepository;

    public GetMachinesQueryHandler(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public async Task<Result<List<MachineDto>>> Handle(GetMachinesQuery request, CancellationToken cancellationToken)
    {
        var machines = await _machineRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (request.OrganizationId.HasValue)
        {
            machines = machines.Where(m => m.OrganizationId == request.OrganizationId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            machines = machines.Where(m => m.Status.ToString().Equals(request.Status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            machines = machines.Where(m =>
                m.SerialNumber.ToLower().Contains(searchLower) ||
                (m.Brand?.ToLower().Contains(searchLower) ?? false) ||
                (m.Model?.ToLower().Contains(searchLower) ?? false) ||
                (m.Location?.ToLower().Contains(searchLower) ?? false)
            ).ToList();
        }

        var dtos = machines.Select(m => new MachineDto
        {
            Id = m.Id,
            OrganizationId = m.OrganizationId,
            SerialNumber = m.SerialNumber,
            Brand = m.Brand,
            Model = m.Model,
            Location = m.Location,
            Status = m.Status.ToString(),
            HasApiToken = m.ApiToken != null,
            LastMaintenanceDate = m.LastMaintenanceDate,
            NextMaintenanceDate = m.NextMaintenanceDate,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();

        return Result.Success(dtos);
    }
}
