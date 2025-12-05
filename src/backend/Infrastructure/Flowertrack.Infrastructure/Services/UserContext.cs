using System.Security.Claims;
using Flowertrack.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Implementation of IUserContext for accessing current user information
/// </summary>
public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        }
    }
}
