using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ReactivateServiceUser;

public class ReactivateServiceUserCommandHandler : IRequestHandler<ReactivateServiceUserCommand, Result<Unit>>
{
    private readonly IServiceUserRepository _serviceUserRepository;

    public ReactivateServiceUserCommandHandler(IServiceUserRepository serviceUserRepository)
    {
        _serviceUserRepository = serviceUserRepository;
    }

    public async Task<Result<Unit>> Handle(ReactivateServiceUserCommand request, CancellationToken ct)
    {
        var user = await _serviceUserRepository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return Result.Failure<Unit>("Service user not found");

        user.Activate();

        await _serviceUserRepository.UpdateAsync(user, ct);

        return Result.Success(Unit.Value);
    }
}
