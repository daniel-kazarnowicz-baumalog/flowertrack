using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.InviteServiceUser;

/// <summary>
/// Command to invite a new service user (technician)
/// US-032: Dodawanie nowego serwisanta
/// </summary>
public sealed record InviteServiceUserCommand : IRequest<Result<Guid>>
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Specialization { get; init; }
}
