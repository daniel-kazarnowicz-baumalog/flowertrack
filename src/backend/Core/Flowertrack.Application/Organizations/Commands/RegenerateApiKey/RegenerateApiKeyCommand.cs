using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.RegenerateApiKey;

/// <summary>
/// Command to regenerate API key for an organization
/// </summary>
public sealed record RegenerateApiKeyCommand(Guid OrganizationId) : IRequest<Result<string>>;
