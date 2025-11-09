using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.UpdateServiceUser;

public record UpdateServiceUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Specialization) : IRequest<Result<Unit>>;
