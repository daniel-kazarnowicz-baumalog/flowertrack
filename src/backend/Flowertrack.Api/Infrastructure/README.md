# Logging Documentation

## Serilog Configuration

The application uses Serilog for structured logging with multiple enrichers and sinks.

### Configuration

Serilog is configured in both `Program.cs` (bootstrap) and `appsettings.json` for runtime configuration.

### Enrichers

The following enrichers are automatically added to all log entries:

- **FromLogContext** - Adds properties from the log context
- **WithEnvironmentName** - Adds the environment name (Development, Production, etc.)
- **WithMachineName** - Adds the machine name
- **WithThreadId** - Adds the thread ID for troubleshooting concurrency issues

### Sinks

#### Console Sink
- Outputs to console with colored output
- Template: `[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}`

#### File Sink
- Outputs to rolling log files in `logs/` directory
- File pattern: `flowertrack-YYYYMMDD.log`
- Rolling: Daily (creates new file each day)
- File size limit: 100MB per file
- Retention: 30 days
- Template: `{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}`

### Request Logging

Serilog Request Logging is enabled to log all HTTP requests with:
- HTTP method
- Request path
- Status code
- Response time
- Request host
- User agent

Template: `HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms`

### Log Levels

#### Development
- Default: Debug
- Microsoft: Information
- Microsoft.AspNetCore: Information

#### Production
- Default: Information
- Microsoft: Warning
- Microsoft.AspNetCore: Warning
- System: Warning

### Structured Logging Extensions

The `LoggerExtensions` class provides convenient methods for structured logging:

#### Ticket Operations
```csharp
_logger.LogTicketCreated(ticketId, title, createdBy);
_logger.LogTicketStatusChanged(ticketId, oldStatus, newStatus, changedBy);
_logger.LogTicketAssigned(ticketId, assignedTo, assignedBy);
```

#### User Actions
```csharp
_logger.LogUserAction("Login", userId);
_logger.LogUserAction("UpdateProfile", userId, "User", userId);
```

#### Authentication
```csharp
_logger.LogAuthenticationEvent("Login", userId, success: true);
_logger.LogAuthenticationEvent("Login", userId, success: false, reason: "Invalid password");
```

#### Service Errors
```csharp
_logger.LogServiceError(exception, "TicketService", "CreateTicket", additionalData);
```

#### Database Operations
```csharp
_logger.LogDatabaseOperation("Insert", "Ticket", ticketId, durationMs);
```

#### External API Calls
```csharp
_logger.LogExternalApiCall("WeatherAPI", "/forecast", "GET", 200, durationMs);
```

### Log File Location

Log files are written to: `logs/flowertrack-YYYYMMDD.log`

The `logs/` directory is excluded from version control via `.gitignore`.

### Example Log Entry

```
2025-10-25 16:09:26.375 +00:00 [INF] Request starting HTTP/1.1 GET http://localhost:5102/health - null null {"EventId":{"Id":1},"SourceContext":"Microsoft.AspNetCore.Hosting.Diagnostics","RequestId":"0HNGJS3FAPG2F:00000001","RequestPath":"/health","ConnectionId":"0HNGJS3FAPG2F","EnvironmentName":"Development","MachineName":"runnervmwhb2z","ThreadId":9}
```

### Querying Logs

The structured format allows for easy querying and analysis:

```bash
# Find all errors
grep '\[ERR\]' logs/flowertrack-*.log

# Find all requests to specific endpoint
grep '/api/tickets' logs/flowertrack-*.log

# Find all logs for specific request ID
grep 'RequestId":"0HNGJS3FAPG2F' logs/flowertrack-*.log
```

### Production Considerations

1. **Log Aggregation**: Consider using a centralized logging solution (e.g., Seq, ELK, Azure Application Insights)
2. **Performance**: Serilog is designed for high-performance structured logging
3. **Sensitive Data**: Never log sensitive information (passwords, tokens, PII)
4. **Disk Space**: Monitor disk usage and adjust retention policy if needed
