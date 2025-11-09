using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using System.Security.Cryptography;

namespace Flowertrack.Application.Users.Commands.ResetServiceUserPassword;

public class ResetServiceUserPasswordCommandHandler : IRequestHandler<ResetServiceUserPasswordCommand, Result<string>>
{
    private readonly IServiceUserRepository _serviceUserRepository;

    public ResetServiceUserPasswordCommandHandler(IServiceUserRepository serviceUserRepository)
    {
        _serviceUserRepository = serviceUserRepository;
    }

    public async Task<Result<string>> Handle(ResetServiceUserPasswordCommand request, CancellationToken ct)
    {
        var user = await _serviceUserRepository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return Result.Failure<string>("Service user not found");

        // Generate temporary password (12 characters: letters + numbers)
        var tempPassword = GenerateTemporaryPassword();

        // TODO: Hash password and set on user entity
        // user.SetPasswordHash(hashedPassword);
        // TODO: Send email with temporary password
        // TODO: Set flag to force password change on next login

        await _serviceUserRepository.UpdateAsync(user, ct);

        return Result.Success(tempPassword);
    }

    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        var password = new char[12];
        
        for (int i = 0; i < password.Length; i++)
        {
            password[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }
        
        return new string(password);
    }
}
