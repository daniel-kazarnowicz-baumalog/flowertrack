---
applyTo: "**/appsettings*.json"
---

# Configuration Guidelines

## appsettings.json Structure

### Naming Conventions
- Use PascalCase for section names
- Use PascalCase for property names
- Group related settings into sections

### Example Structure
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FlowerTrack;..."
  },
  "ApplicationSettings": {
    "MaxTicketsPerPage": 50,
    "EnableNotifications": true,
    "TicketExpirationDays": 30
  },
  "ExternalServices": {
    "EmailService": {
      "ApiKey": "use-environment-variables",
      "FromAddress": "noreply@flowertrack.com"
    }
  }
}
```

## Environment-Specific Settings

### Development (appsettings.Development.json)
- Use for local development overrides
- Can include verbose logging
- May point to local databases/services

### Production (appsettings.json)
- Should NOT contain sensitive data
- Use environment variables for secrets
- Include only safe defaults

## Sensitive Data Handling

### ❌ NEVER commit:
- API keys
- Passwords
- Connection strings with credentials
- OAuth secrets
- Encryption keys

### ✅ Use instead:
- Environment variables
- Azure Key Vault (for production)
- User Secrets (for development)

```csharp
// Reading from environment variables
var apiKey = builder.Configuration["ExternalServices:EmailService:ApiKey"];

// Or with fallback
var apiKey = builder.Configuration["EmailApiKey"] 
    ?? throw new InvalidOperationException("Email API key not configured");
```

## User Secrets (Development)

Set up user secrets for local development:

```powershell
# Initialize user secrets
dotnet user-secrets init

# Add a secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..."
dotnet user-secrets set "EmailApiKey" "your-api-key-here"

# List secrets
dotnet user-secrets list
```

## Configuration Best Practices

1. **Organize by feature/domain**
   - Group related settings together
   - Use nested objects for complex configurations

2. **Use strongly-typed configuration**
   ```csharp
   // Configuration class
   public class ApplicationSettings
   {
       public int MaxTicketsPerPage { get; set; }
       public bool EnableNotifications { get; set; }
       public int TicketExpirationDays { get; set; }
   }
   
   // Register in Program.cs
   builder.Services.Configure<ApplicationSettings>(
       builder.Configuration.GetSection("ApplicationSettings"));
   
   // Inject in services
   public class TicketService
   {
       private readonly ApplicationSettings _settings;
       
       public TicketService(IOptions<ApplicationSettings> settings)
       {
           _settings = settings.Value;
       }
   }
   ```

3. **Validate configuration on startup**
   ```csharp
   builder.Services.AddOptions<ApplicationSettings>()
       .Bind(builder.Configuration.GetSection("ApplicationSettings"))
       .ValidateDataAnnotations()
       .ValidateOnStart();
   ```

4. **Document configuration values**
   - Add comments explaining non-obvious settings
   - Include example values
   - Note required vs optional settings
