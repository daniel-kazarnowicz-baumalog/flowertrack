using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Auth.Queries.GetCurrentServiceUser;

/// <summary>
/// Handler for getting current authenticated service user
/// </summary>
public sealed class GetCurrentServiceUserQueryHandler : IRequestHandler<GetCurrentServiceUserQuery, Result<ServiceUserDto>>
{
    private readonly IServiceUserRepository _serviceUserRepository;

    public GetCurrentServiceUserQueryHandler(IServiceUserRepository serviceUserRepository)
    {
        _serviceUserRepository = serviceUserRepository ?? throw new ArgumentNullException(nameof(serviceUserRepository));
    }

    public async Task<Result<ServiceUserDto>> Handle(GetCurrentServiceUserQuery request, CancellationToken cancellationToken)
    {
        // Get service user
        var serviceUser = await _serviceUserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (serviceUser == null)
        {
            return Result.Failure<ServiceUserDto>($"Service user with ID {request.UserId} was not found");
        }

        // Map to DTO
        var dto = new ServiceUserDto
        {
            Id = serviceUser.Id,
            Email = serviceUser.Email.Value,
            FullName = $"{serviceUser.FirstName} {serviceUser.LastName}",
            Role = "ServiceUser", // TODO: Get actual role from UserRoles when navigation property is loaded
            Status = serviceUser.Status.ToString()
        };

        return Result.Success(dto);
    }
}
