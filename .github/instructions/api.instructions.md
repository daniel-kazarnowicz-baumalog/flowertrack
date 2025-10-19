---
applyTo: "**/Controllers/**/*.cs"
---

# API Controller Guidelines

## Controller Best Practices

### Structure
- Keep controllers thin - delegate logic to services
- One controller per resource/entity
- Use `[ApiController]` attribute for automatic model validation
- Use route attributes for RESTful endpoints

### Naming
- Controllers: `{Entity}Controller` (e.g., `TicketsController`)
- Actions: Use verb names that match HTTP methods

### Response Types
Always specify response types with `ProducesResponseType`:

```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<TicketDto>> GetTicket(int id)
{
    // Implementation
}
```

### Model Validation
```csharp
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateRequest request)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // Process valid request
}
```

### Error Responses
Return appropriate status codes and error details:

```csharp
try
{
    var result = await _service.ProcessAsync(id);
    return Ok(result);
}
catch (NotFoundException ex)
{
    return NotFound(new { message = ex.Message });
}
catch (ValidationException ex)
{
    return BadRequest(new { errors = ex.Errors });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    return StatusCode(500, new { message = "An error occurred" });
}
```

## RESTful Endpoint Patterns

### Standard CRUD Operations
- `GET /api/tickets` - List all
- `GET /api/tickets/{id}` - Get one
- `POST /api/tickets` - Create
- `PUT /api/tickets/{id}` - Update (full)
- `PATCH /api/tickets/{id}` - Update (partial)
- `DELETE /api/tickets/{id}` - Delete

### Nested Resources
- `GET /api/tickets/{ticketId}/comments` - Get ticket comments
- `POST /api/tickets/{ticketId}/comments` - Add comment to ticket

### Filtering & Pagination
```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<TicketDto>>> GetTickets(
    [FromQuery] TicketFilterParams filterParams)
{
    // filterParams includes: page, pageSize, status, assigneeId, etc.
    var result = await _service.GetTicketsAsync(filterParams);
    return Ok(result);
}
```

## Authentication & Authorization

```csharp
[Authorize] // Requires authentication
[Authorize(Roles = "Admin")] // Requires specific role
[Authorize(Policy = "CanManageTickets")] // Requires policy

[HttpDelete("{id}")]
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> DeleteTicket(int id)
{
    await _service.DeleteTicketAsync(id);
    return NoContent();
}
```

## API Versioning (when implemented)

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TicketsController : ControllerBase
{
    // Version 1 endpoints
}
```

## Documentation

- Add XML comments for Swagger/OpenAPI generation
- Document parameters, responses, and exceptions

```csharp
/// <summary>
/// Retrieves a specific ticket by ID
/// </summary>
/// <param name="id">The ticket identifier</param>
/// <returns>The ticket details</returns>
/// <response code="200">Returns the ticket</response>
/// <response code="404">Ticket not found</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(TicketDto), 200)]
[ProducesResponseType(404)]
public async Task<ActionResult<TicketDto>> GetTicket(int id)
{
    // Implementation
}
```
