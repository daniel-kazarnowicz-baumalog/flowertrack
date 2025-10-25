# Phase 0.3: Logging, Exception Handling, CORS & Health Checks

## Overview

This phase implements production-ready infrastructure for the FLOWerTRACK API including:
- Structured logging with Serilog
- Global exception handling with RFC 7807 compliance
- CORS configuration
- Health check endpoints

## Components

### 1. Serilog Logging

**Packages:**
- Serilog.AspNetCore 9.0.0
- Serilog.Enrichers.Environment 3.0.1
- Serilog.Enrichers.Thread 4.0.0

**Features:**
- Structured logging with JSON format
- Console and File sinks
- Daily rolling file logs (100MB max, 30 days retention)
- Request logging middleware
- Custom enrichers (Environment, Machine, Thread)

**Configuration:**
- `appsettings.json` - Serilog configuration section
- `Program.cs` - Bootstrap configuration
- `Infrastructure/LoggerExtensions.cs` - Structured logging helpers

**Documentation:** See [Backend Infrastructure README](../src/backend/Flowertrack.Api/Infrastructure/README.md)

### 2. Global Exception Handler

**Packages:**
- Built-in ASP.NET Core middleware

**Features:**
- RFC 7807 compliant error responses
- Custom exception types for common scenarios
- Environment-aware (detailed errors in Development only)
- Structured error logging

**Components:**
- `Middleware/GlobalExceptionHandlerMiddleware.cs` - Main middleware
- `Exceptions/` - Custom exception types
  - `BaseException` - Abstract base class
  - `NotFoundException` - 404 errors
  - `ValidationException` - 400 errors with validation details
  - `UnauthorizedException` - 401 errors
  - `ForbiddenException` - 403 errors
  - `DomainException` - Business rule violations
- `Contracts/Common/ErrorResponse.cs` - RFC 7807 response model

**Documentation:** See [Middleware README](../src/backend/Flowertrack.Api/Middleware/README.md)

### 3. CORS Configuration

**Features:**
- Named policy: "AllowFrontend"
- Configuration-driven URLs
- Credentials support
- All methods and headers allowed

**Configuration:**
```json
{
  "Frontend": {
    "Url": "http://localhost:5173"
  }
}
```

**Environments:**
- Development: `http://localhost:5173` (Vite default)
- Production: Configurable in `appsettings.json`

### 4. Health Checks

**Packages:**
- AspNetCore.HealthChecks.NpgSql 9.0.0

**Endpoints:**
- `/health` - All health checks (database, etc.)
- `/health/ready` - Readiness probe (tagged checks)
- `/health/live` - Liveness probe (always healthy if running)

**Response Format:**
```json
{
  "status": "Healthy",
  "totalDuration": 246.67,
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": "PostgreSQL connection successful",
      "duration": 185.74,
      "data": {}
    }
  ]
}
```

**Features:**
- Custom response writer with detailed information
- PostgreSQL database health check
- Timeout: 3 seconds
- Tagged checks for Kubernetes readiness probes

## Configuration Files

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/flowertrack-.log",
          "rollingInterval": "Day",
          "fileSizeLimitBytes": 104857600,
          "retainedFileCountLimit": 30
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithEnvironmentName", "WithMachineName", "WithThreadId"]
  },
  "Frontend": {
    "Url": "http://localhost:5173"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=flowertrack;Username=postgres;Password=postgres"
  }
}
```

### appsettings.Development.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  },
  "Frontend": {
    "Url": "http://localhost:5173"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=flowertrack_dev;Username=postgres;Password=postgres"
  }
}
```

## Testing

### Manual Testing

**Test Exception Handling (Development only):**
```bash
# NotFoundException (404)
curl http://localhost:5102/test/not-found

# ValidationException (400)
curl http://localhost:5102/test/validation

# UnauthorizedException (401)
curl http://localhost:5102/test/unauthorized

# ForbiddenException (403)
curl http://localhost:5102/test/forbidden

# DomainException (400)
curl http://localhost:5102/test/domain

# Server Error (500)
curl http://localhost:5102/test/server-error
```

**Test Health Checks:**
```bash
# Full health check
curl http://localhost:5102/health

# Liveness probe
curl http://localhost:5102/health/live

# Readiness probe
curl http://localhost:5102/health/ready
```

**Test Logging:**
```bash
# Run the application
dotnet run

# Check console output for structured logs
# Check logs/flowertrack-YYYYMMDD.log for file output
```

## Usage Examples

### Throwing Custom Exceptions

```csharp
// In a service or controller
public async Task<Ticket> GetTicketAsync(int id)
{
    var ticket = await _repository.GetByIdAsync(id);
    if (ticket == null)
    {
        throw new NotFoundException("Ticket", id);
    }
    return ticket;
}

public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request)
{
    // Validation
    var errors = new Dictionary<string, string[]>();
    if (string.IsNullOrEmpty(request.Title))
    {
        errors.Add("Title", new[] { "Title is required" });
    }
    
    if (errors.Any())
    {
        throw new ValidationException(errors);
    }
    
    // Business logic
    if (!_authService.CanCreateTicket(userId))
    {
        throw new ForbiddenException("You don't have permission to create tickets");
    }
    
    // Domain rules
    if (request.Priority > 10)
    {
        throw new DomainException("Priority cannot exceed 10");
    }
    
    // Create ticket...
}
```

### Using Structured Logging

```csharp
public class TicketService
{
    private readonly ILogger<TicketService> _logger;
    
    public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request)
    {
        _logger.LogTicketCreated(ticket.Id, ticket.Title, currentUser.Id);
        
        // ... business logic
        
        if (exception != null)
        {
            _logger.LogServiceError(exception, "TicketService", "CreateTicket", 
                new Dictionary<string, object> 
                {
                    { "RequestTitle", request.Title },
                    { "UserId", currentUser.Id }
                });
        }
        
        return ticket;
    }
}
```

## Deployment Considerations

### Production Environment

1. **Logging:**
   - Configure external logging service (Seq, ELK, Azure App Insights)
   - Adjust log levels (Information or Warning for production)
   - Monitor disk space for log files

2. **CORS:**
   - Update Frontend URL in production appsettings
   - Consider multiple origins if needed
   - Restrict to specific domains only

3. **Health Checks:**
   - Configure Kubernetes liveness and readiness probes
   - Monitor health check endpoints
   - Set up alerts for unhealthy status

4. **Exception Handling:**
   - Ensure sensitive information is not leaked
   - Monitor error logs for patterns
   - Set up error alerting

### Security

- Never log sensitive data (passwords, tokens, PII)
- Stack traces only in Development
- Sanitize error messages in Production
- Use secure connection strings (Azure Key Vault, etc.)

## Files Modified/Created

### New Files
- `Exceptions/BaseException.cs`
- `Exceptions/NotFoundException.cs`
- `Exceptions/ValidationException.cs`
- `Exceptions/UnauthorizedException.cs`
- `Exceptions/ForbiddenException.cs`
- `Exceptions/DomainException.cs`
- `Contracts/Common/ErrorResponse.cs`
- `Middleware/GlobalExceptionHandlerMiddleware.cs`
- `Middleware/README.md`
- `Extensions/HealthCheckResponseWriter.cs`
- `Infrastructure/LoggerExtensions.cs`
- `Infrastructure/README.md`

### Modified Files
- `Flowertrack.Api.csproj` - Added NuGet packages
- `Program.cs` - Configured all middleware
- `appsettings.json` - Added Serilog, Frontend, ConnectionStrings sections
- `appsettings.Development.json` - Development-specific settings

### Directory Structure
```
Flowertrack.Api/
├── Contracts/
│   └── Common/
│       └── ErrorResponse.cs
├── Exceptions/
│   ├── BaseException.cs
│   ├── NotFoundException.cs
│   ├── ValidationException.cs
│   ├── UnauthorizedException.cs
│   ├── ForbiddenException.cs
│   └── DomainException.cs
├── Extensions/
│   └── HealthCheckResponseWriter.cs
├── Infrastructure/
│   ├── LoggerExtensions.cs
│   └── README.md
├── Middleware/
│   ├── GlobalExceptionHandlerMiddleware.cs
│   └── README.md
├── logs/ (gitignored)
│   └── flowertrack-YYYYMMDD.log
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## Next Steps

After Phase 0.3, the application is ready for:
- Database integration (Entity Framework Core)
- Authentication & Authorization (JWT, IdentityServer)
- API endpoint implementation
- Business logic development

## References

- [RFC 7807 - Problem Details](https://tools.ietf.org/html/rfc7807)
- [Serilog Documentation](https://serilog.net/)
- [ASP.NET Core Health Checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [CORS in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/cors)
