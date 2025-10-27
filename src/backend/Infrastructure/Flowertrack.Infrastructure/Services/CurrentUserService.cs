using System.Security.Claims;
using Flowertrack.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Service for accessing current authenticated user information
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var orgIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("organization_id")?.Value;
            return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : null;
        }
    }

    public bool IsServiceUser
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole("ServiceAdministrator") == true ||
                   user?.IsInRole("ServiceTechnician") == true;
        }
    }

    public bool IsServiceAdministrator
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("ServiceAdministrator") == true;
        }
    }

    public bool IsOrganizationAdministrator
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("OrganizationAdministrator") == true ||
                   _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value == "Admin";
        }
    }
}
