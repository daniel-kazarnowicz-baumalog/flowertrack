using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ReactivateServiceUser;

public record ReactivateServiceUserCommand(Guid Id) : IRequest<Result<Unit>>;
