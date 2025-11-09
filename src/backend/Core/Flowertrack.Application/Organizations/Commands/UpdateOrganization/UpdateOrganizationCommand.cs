using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Command to update organization information
/// </summary>
public sealed record UpdateOrganizationCommand : IRequest<Result<Guid>>
{
    public Guid OrganizationId { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? Notes { get; init; }
}
