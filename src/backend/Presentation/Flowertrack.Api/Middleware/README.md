# Middleware Documentation

## GlobalExceptionHandlerMiddleware

The `GlobalExceptionHandlerMiddleware` is a centralized exception handling middleware that catches all unhandled exceptions and returns RFC 7807 compliant error responses.

### Features

- **RFC 7807 Compliant**: All error responses follow the Problem Details specification
- **Custom Exception Mapping**: Maps custom exceptions to appropriate HTTP status codes
- **Environment-Aware**: Includes stack traces and detailed information only in Development mode
- **Structured Logging**: Logs all errors with structured properties for easy querying

### Supported Exceptions

| Exception Type | HTTP Status | Description |
|---------------|-------------|-------------|
| `NotFoundException` | 404 Not Found | Resource not found |
| `ValidationException` | 400 Bad Request | Validation errors (includes error details) |
| `UnauthorizedException` | 401 Unauthorized | Authentication required |
| `ForbiddenException` | 403 Forbidden | Insufficient permissions |
| `DomainException` | 400 Bad Request | Business rule violation |
| Generic `Exception` | 500 Internal Server Error | Unexpected errors |

### Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "instance": "/api/tickets",
  "extensions": {
    "errors": {
      "Title": ["Title is required"],
      "Priority": ["Invalid priority value"]
    }
  }
}
```

### Usage

The middleware is automatically registered in `Program.cs` and runs first in the pipeline:

```csharp
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
```

### Development Mode

In Development environment, error responses include additional debugging information:
- Stack trace
- Exception type
- Full error messages

In Production, only user-friendly messages are returned to avoid information disclosure.

### Example Usage

```csharp
public async Task<Ticket> GetTicketAsync(int id)
{
    var ticket = await _repository.GetByIdAsync(id);
    if (ticket == null)
    {
        throw new NotFoundException("Ticket", id);
    }
    return ticket;
}
```

### Testing

Test endpoints are available in Development mode:
- `/test/not-found` - Tests NotFoundException (404)
- `/test/validation` - Tests ValidationException (400)
- `/test/unauthorized` - Tests UnauthorizedException (401)
- `/test/forbidden` - Tests ForbiddenException (403)
- `/test/domain` - Tests DomainException (400)
- `/test/server-error` - Tests generic Exception (500)
