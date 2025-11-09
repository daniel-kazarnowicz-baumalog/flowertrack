using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Auth.DTOs;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flowertrack.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IServiceUserRepository serviceUserRepository,
    IOrganizationUserRepository organizationUserRepository,
    IUserRoleRepository userRoleRepository,
    IRefreshTokenRepository refreshTokenRepository)
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private const string JwtSecret = "your-256-bit-secret-key-here-minimum-32-characters-required";
    private const string JwtIssuer = "FLOWerTRACK";
    private const string JwtAudience = "FLOWerTRACK-Clients";
    private const int JwtExpirationMinutes = 60;
    
    public async Task<Result<LoginResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        // Validate the access token (even if expired)
        var principal = ValidateToken(command.Token, validateLifetime: false);
        if (principal == null)
        {
            return Result.Failure<LoginResponse>("Invalid token");
        }

        // Extract user ID from token
        var userIdClaim = principal.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Result.Failure<LoginResponse>("Invalid token claims");
        }

        // Find the refresh token
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(
            command.RefreshToken,
            cancellationToken);

        if (refreshToken == null)
        {
            return Result.Failure<LoginResponse>("Invalid refresh token");
        }

        // Validate refresh token
        if (refreshToken.UserId != userId)
        {
            return Result.Failure<LoginResponse>("Token mismatch");
        }

        if (!refreshToken.IsActive)
        {
            return Result.Failure<LoginResponse>("Refresh token is not active");
        }

        if (refreshToken.IsExpired)
        {
            return Result.Failure<LoginResponse>("Refresh token has expired");
        }

        // Get user type from token claims
        var userType = principal.FindFirst("user_type")?.Value;
        
        // Fetch user with roles
        UserDto? userDto = null;

        if (userType == "ServiceUser")
        {
            var serviceUser = await serviceUserRepository.GetByIdAsync(userId, cancellationToken);
            if (serviceUser == null)
            {
                return Result.Failure<LoginResponse>("User not found");
            }

            // Get user roles
            var userRoles = await userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            userDto = new UserDto(
                serviceUser.Id,
                serviceUser.Email,
                serviceUser.FirstName,
                serviceUser.LastName,
                "ServiceUser",
                roleNames
            );
        }
        else if (userType == "OrganizationUser")
        {
            var orgUser = await organizationUserRepository.GetByIdAsync(userId, cancellationToken);
            if (orgUser == null)
            {
                return Result.Failure<LoginResponse>("User not found");
            }

            // Get user roles
            var userRoles = await userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            userDto = new UserDto(
                orgUser.Id,
                orgUser.Email,
                orgUser.FirstName,
                orgUser.LastName,
                "OrganizationUser",
                roleNames,
                orgUser.OrganizationId
            );
        }
        else
        {
            return Result.Failure<LoginResponse>("Invalid user type");
        }

        // Revoke the old refresh token
        refreshToken.Revoke(command.IpAddress, "Replaced by new token");
        await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        // Generate new tokens
        var newAccessToken = GenerateJwtToken(userDto, userType);
        var newRefreshToken = GenerateRefreshToken();
        
        var newRefreshTokenEntity = Domain.Entities.Authentication.RefreshToken.Create(
            userDto.Id,
            newRefreshToken,
            7, // 7 days
            command.IpAddress
        );

        await refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

        return Result.Success(new LoginResponse(
            newAccessToken,
            newRefreshToken,
            JwtExpirationMinutes * 60, // Convert to seconds
            userDto
        ));
    }

    private ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(JwtSecret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtIssuer,
                ValidAudience = JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateJwtToken(UserDto user, string userType)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("user_type", userType)
        };

        // Add roles
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add organization for OrganizationUser
        if (userType == "OrganizationUser" && user.OrganizationId.HasValue)
        {
            claims.Add(new Claim("organization_id", user.OrganizationId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(JwtExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
