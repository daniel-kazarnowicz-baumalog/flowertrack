using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationMachines;

/// <summary>
/// Handler for GetOrganizationMachinesQuery
/// </summary>
public sealed class GetOrganizationMachinesQueryHandler 
    : IRequestHandler<GetOrganizationMachinesQuery, Result<List<MachineSummaryDto>>>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationMachinesQueryHandler(
        IMachineRepository machineRepository,
        IOrganizationRepository organizationRepository)
    {
        _machineRepository = machineRepository ?? throw new ArgumentNullException(nameof(machineRepository));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    public async Task<Result<List<MachineSummaryDto>>> Handle(
        GetOrganizationMachinesQuery request, 
        CancellationToken cancellationToken)
    {
        // Verify organization exists
        var organizationExists = await _organizationRepository.ExistsAsync(request.OrganizationId, cancellationToken);
        if (!organizationExists)
        {
            return Result.Failure<List<MachineSummaryDto>>(
                $"Organization with ID {request.OrganizationId} was not found");
        }

        // Get machines
        var machines = await _machineRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);

        var machineDtos = machines.Select(m => new MachineSummaryDto
        {
            Id = m.Id,
            SerialNumber = m.SerialNumber,
            Brand = m.Brand ?? string.Empty,
            Model = m.Model ?? string.Empty,
            Status = m.Status.ToString(),
            LastMaintenanceDate = m.LastMaintenanceDate,
            NextMaintenanceDate = m.NextMaintenanceDate,
            RegisteredAt = m.CreatedAt
        }).ToList();

        return Result.Success(machineDtos);
    }
}
