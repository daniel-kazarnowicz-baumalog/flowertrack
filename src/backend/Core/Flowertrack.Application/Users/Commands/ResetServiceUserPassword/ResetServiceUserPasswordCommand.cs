using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ResetServiceUserPassword;

public record ResetServiceUserPasswordCommand(Guid Id) : IRequest<Result<string>>;
