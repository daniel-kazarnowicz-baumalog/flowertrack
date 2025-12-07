using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.DeleteOrganization;

/// <summary>
/// Command to delete (soft delete) an organization
/// </summary>
public sealed record DeleteOrganizationCommand(Guid OrganizationId) : IRequest<Result>;
