# Supabase Authentication Configuration

## Overview

FLOWerTRACK uses **Supabase Auth** for user authentication and authorization. This document describes the authentication flow and configuration.

## Architecture

```
Client → API Endpoint → JWT Validation → Authorization Policy → Handler → Supabase Auth
```

## JWT Token Configuration

### Token Validation Parameters

- **Issuer**: `{Supabase URL}` (e.g., `http://127.0.0.1:54321` for local dev)
- **Audience**: `authenticated` (Supabase default)
- **Signing Key**: Supabase JWT Secret (HS256 algorithm)
- **Clock Skew**: Zero (strict expiration validation)

### Token Claims

Supabase JWT tokens include the following claims:

```json
{
  "sub": "user-uuid",                    // User ID (Supabase auth.users.id)
  "email": "user@example.com",           // User email
  "role": "authenticated",                // Supabase role
  "user_metadata": {                     // Custom metadata
    "role": "service_admin",             // Application role
    "full_name": "John Doe",
    "organization_id": "org-uuid"        // For organization users
  },
  "iat": 1699564800,                     // Issued at
  "exp": 1699568400                      // Expires at
}
```

## Authorization Policies

### Service Portal Policies

| Policy Name | Required Role(s) | Description |
|-------------|------------------|-------------|
| `RequireServiceAdmin` | `service_admin` | Full administrative access to service portal |
| `RequireServiceUser` | `service_admin`, `service_technician` | Access to service portal features |

### Client Portal Policies

| Policy Name | Required Role(s) | Description |
|-------------|------------------|-------------|
| `RequireOrganizationAdmin` | `organization_admin` | Administrative access to organization |
| `RequireOrganizationUser` | `organization_admin`, `organization_operator` | Access to organization features |

### General Policies

| Policy Name | Description |
|-------------|-------------|
| `RequireAuthenticatedUser` | Any authenticated user |

## Usage in Controllers

### Protecting Endpoints

```csharp
[HttpGet("admin/users")]
[Authorize(Policy = "RequireServiceAdmin")]
public async Task<IActionResult> GetAllUsers()
{
    // Only service admins can access
}

[HttpPost("tickets")]
[Authorize(Policy = "RequireOrganizationUser")]
public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
{
    // Organization users and admins can create tickets
}
```

### Accessing User Information

```csharp
// Get current user ID (Supabase User ID)
var userId = User.FindFirst("sub")?.Value;

// Get user email
var email = User.FindFirst("email")?.Value;

// Get user role from metadata
var role = User.FindFirst("user_metadata.role")?.Value;

// Check if user is authenticated
var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
```

## Configuration Files

### appsettings.json

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "AnonKey": "your-anon-key",
    "ServiceKey": "your-service-key",
    "JwtSecret": "your-jwt-secret"
  }
}
```

### appsettings.Development.json

```json
{
  "Supabase": {
    "Url": "http://127.0.0.1:54321",
    "AnonKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "ServiceKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "JwtSecret": "your-super-secret-jwt-token-with-at-least-32-characters-long"
  }
}
```

**⚠️ Important**: Never commit real secrets to version control. Use User Secrets or environment variables.

### Using User Secrets (Development)

```bash
cd src/backend/Presentation/Flowertrack.Api

# Set Supabase configuration
dotnet user-secrets set "Supabase:Url" "http://127.0.0.1:54321"
dotnet user-secrets set "Supabase:AnonKey" "your-anon-key"
dotnet user-secrets set "Supabase:ServiceKey" "your-service-key"
dotnet user-secrets set "Supabase:JwtSecret" "your-jwt-secret"
```

## Authentication Flow

### Service User Login Flow

1. **Client** sends `POST /api/auth/service/login` with email and password
2. **API** calls `IAuthService.SignInAsync()` → authenticates via Supabase Auth
3. **Supabase** validates credentials and returns JWT tokens
4. **API** retrieves `ServiceUser` entity from database using `SupabaseUserId`
5. **API** verifies user status is `Active`
6. **API** returns access token, refresh token, and user info to client
7. **Client** stores tokens and includes `Authorization: Bearer {token}` in subsequent requests

### Organization User Login Flow

Similar to Service User, but:
- Uses `OrganizationUser` entity
- Checks `IsActivated = true` before allowing login
- May require initial activation via invitation link

### Token Refresh Flow

1. **Client** sends `POST /api/auth/refresh` with refresh token
2. **API** calls `IAuthService.RefreshTokenAsync()`
3. **Supabase** validates refresh token and issues new access token
4. **API** returns new access token to client

## Testing Authentication

### Swagger UI

1. Navigate to `/swagger` in your browser
2. Click "Authorize" button
3. Enter: `Bearer {your-jwt-token}`
4. Click "Authorize"
5. All authenticated endpoints will now include the token

### Postman

1. Create a new request
2. Go to "Authorization" tab
3. Select "Bearer Token"
4. Paste your JWT token
5. Send request

### cURL

```bash
curl -X GET "https://localhost:5001/api/auth/service/me" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Troubleshooting

### Common Issues

**Issue**: "401 Unauthorized" on protected endpoints
- **Solution**: Verify token is included in `Authorization` header with `Bearer` prefix

**Issue**: "JWT signature validation failed"
- **Solution**: Ensure `Supabase:JwtSecret` in configuration matches Supabase project

**Issue**: "Token has expired"
- **Solution**: Use refresh token to get new access token

**Issue**: "Invalid audience"
- **Solution**: Verify `ValidAudience` is set to `"authenticated"` in `Program.cs`

### Logging

JWT authentication events are logged with the following prefixes:
- `JWT Authentication failed:` - Token validation errors
- `JWT Token validated for user:` - Successful validation
- `JWT Authentication challenge:` - Missing or invalid token

Check logs in `logs/flowertrack-{date}.log` for detailed information.

## Security Best Practices

1. **Never commit secrets** to version control
2. **Use HTTPS** in production (Supabase requires it)
3. **Rotate JWT secrets** periodically
4. **Set short token expiration** times (1 hour recommended)
5. **Use refresh tokens** for long-lived sessions
6. **Validate tokens on every request** (already handled by middleware)
7. **Check user status** (Active/Inactive) in application logic
8. **Implement rate limiting** on auth endpoints (TODO)

## References

- [Supabase Auth Documentation](https://supabase.com/docs/guides/auth)
- [JWT.io](https://jwt.io/) - Token debugger
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
