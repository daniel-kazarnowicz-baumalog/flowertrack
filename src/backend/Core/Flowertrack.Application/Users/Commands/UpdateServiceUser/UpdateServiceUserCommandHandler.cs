using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Commands.UpdateServiceUser;

public class UpdateServiceUserCommandHandler : IRequestHandler<UpdateServiceUserCommand, Result<Unit>>
{
    private readonly IServiceUserRepository _serviceUserRepository;

    public UpdateServiceUserCommandHandler(IServiceUserRepository serviceUserRepository)
    {
        _serviceUserRepository = serviceUserRepository;
    }

    public async Task<Result<Unit>> Handle(UpdateServiceUserCommand request, CancellationToken ct)
    {
        var user = await _serviceUserRepository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return Result.Failure<Unit>("Service user not found");

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        
        if (request.Specialization != user.Specialization)
        {
            user.UpdateSpecialization(request.Specialization);
        }

        await _serviceUserRepository.UpdateAsync(user, ct);

        return Result.Success(Unit.Value);
    }
}
