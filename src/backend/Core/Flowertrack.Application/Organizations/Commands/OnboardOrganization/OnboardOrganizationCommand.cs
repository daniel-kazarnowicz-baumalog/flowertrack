using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.OnboardOrganization;

/// <summary>
/// Command to onboard a new organization with admin user
/// US-025: Inicjowanie onboardingu nowej organizacji
/// </summary>
public sealed record OnboardOrganizationCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminFirstName { get; init; } = string.Empty;
    public string AdminLastName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? Notes { get; init; }
}
