using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.DeactivateServiceUser;

public record DeactivateServiceUserCommand(
    Guid Id,
    string Reason) : IRequest<Result<Unit>>;
