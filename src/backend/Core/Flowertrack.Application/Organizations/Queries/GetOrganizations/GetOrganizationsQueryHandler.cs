using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizations;

/// <summary>
/// Handler for GetOrganizationsQuery
/// </summary>
public sealed class GetOrganizationsQueryHandler
    : IRequestHandler<GetOrganizationsQuery, PaginatedList<OrganizationDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrganizationsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<OrganizationDto>> Handle(
        GetOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Organizations.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(o =>
                o.Name.Contains(request.SearchTerm) ||
                (o.Email != null && o.Email.Contains(request.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.ServiceStatus) &&
            Enum.TryParse<ServiceStatus>(request.ServiceStatus, out var status))
        {
            query = query.Where(o => o.ServiceStatus == status);
        }

        // Project to DTO
        var dtoQuery = query
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                Email = o.Email,
                Phone = o.Phone,
                City = o.City,
                Country = o.Country,
                ServiceStatus = o.ServiceStatus.ToString(),
                ContractStartDate = o.ContractStartDate,
                ContractEndDate = o.ContractEndDate,
                HasApiKey = !string.IsNullOrEmpty(o.ApiKey),
                CreatedAt = o.CreatedAt
            });

        return await PaginatedList<OrganizationDto>.CreateAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );
    }
}
