using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizations;

/// <summary>
/// Query to get all organizations with optional pagination
/// US-024: Wyświetlenie listy organizacji z kluczowymi informacjami
/// </summary>
public sealed record GetOrganizationsQuery : IRequest<PaginatedList<OrganizationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public string? ServiceStatus { get; init; }
}
