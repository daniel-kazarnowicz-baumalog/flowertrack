---
applyTo: "**/*.cs"
---

# C# & .NET Guidelines

## Code Style & Conventions

### Naming Conventions
- **PascalCase**: Classes, Methods, Properties, Public Fields, Constants
  ```csharp
  public class ServiceTicket { }
  public void ProcessTicket() { }
  public string TicketNumber { get; set; }
  public const int MaxRetries = 3;
  ```

- **camelCase**: Parameters, Local Variables, Private Fields (with `_` prefix)
  ```csharp
  private readonly ITicketRepository _ticketRepository;
  private int _retryCount;
  
  public void UpdateTicket(string ticketId, TicketStatus status)
  {
      var currentTicket = _ticketRepository.GetById(ticketId);
      // ...
  }
  ```

- **IPascalCase**: Interfaces (prefix with `I`)
  ```csharp
  public interface ITicketService { }
  public interface IRepository<T> { }
  ```

### File Organization
- One class per file (exceptions: small related classes/records)
- Filename matches class name: `TicketService.cs`
- Organize using statements (remove unused)
- Use file-scoped namespaces (.NET 6+):
  ```csharp
  namespace Flowertrack.Api.Services;
  
  public class TicketService
  {
      // Class implementation
  }
  ```

## Language Features

### Nullable Reference Types
Project has nullable reference types enabled - handle nulls explicitly:

```csharp
// Good - explicit null handling
public string? GetTicketDescription(int ticketId)
{
    var ticket = _repository.FindById(ticketId);
    return ticket?.Description;
}

// Good - null check
public void ProcessTicket(Ticket? ticket)
{
    if (ticket is null)
    {
        throw new ArgumentNullException(nameof(ticket));
    }
    
    // Safe to use ticket here
}

// Avoid - suppressing warnings without validation
public void BadExample(Ticket? ticket)
{
    var description = ticket!.Description; // Don't do this
}
```

### Async/Await
**ALL I/O operations must be asynchronous:**

```csharp
// Database operations
public async Task<Ticket> GetTicketAsync(int id)
{
    return await _context.Tickets
        .Include(t => t.Assignee)
        .FirstOrDefaultAsync(t => t.Id == id);
}

// HTTP calls
public async Task<WeatherData> GetWeatherAsync()
{
    using var client = new HttpClient();
    var response = await client.GetAsync(weatherApiUrl);
    return await response.Content.ReadFromJsonAsync<WeatherData>();
}

// File operations
public async Task<string> ReadFileAsync(string path)
{
    return await File.ReadAllTextAsync(path);
}
```

### Pattern Matching
Use modern C# pattern matching features:

```csharp
// Switch expressions
public string GetStatusDescription(TicketStatus status) => status switch
{
    TicketStatus.New => "Ticket has been created",
    TicketStatus.InProgress => "Technician is working on it",
    TicketStatus.Resolved => "Issue has been resolved",
    TicketStatus.Closed => "Ticket is closed",
    _ => throw new ArgumentOutOfRangeException(nameof(status))
};

// Property patterns
public decimal CalculateDiscount(Order order) => order switch
{
    { Total: > 1000, IsPremium: true } => 0.2m,
    { Total: > 500 } => 0.1m,
    { IsPremium: true } => 0.05m,
    _ => 0m
};

// Type patterns
public void ProcessMessage(object message)
{
    if (message is EmailMessage { IsUrgent: true } email)
    {
        SendUrgentEmail(email);
    }
}
```

### Records for DTOs
Use records for data transfer objects and value objects:

```csharp
// Simple DTO
public record TicketDto(int Id, string Title, string Status);

// With validation
public record CreateTicketRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public int? AssigneeId { get; init; }
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ValidationException("Title is required");
    }
}
```

## Dependency Injection

### Service Registration
Register services in `Program.cs`:

```csharp
// Scoped (per request)
builder.Services.AddScoped<ITicketService, TicketService>();

// Singleton (single instance)
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

// Transient (new instance each time)
builder.Services.AddTransient<IEmailSender, EmailSender>();
```

### Constructor Injection
Inject dependencies via constructor (preferred):

```csharp
public class TicketService : ITicketService
{
    private readonly ITicketRepository _repository;
    private readonly ILogger<TicketService> _logger;
    private readonly IEmailSender _emailSender;
    
    public TicketService(
        ITicketRepository repository,
        ILogger<TicketService> logger,
        IEmailSender emailSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }
    
    public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request)
    {
        _logger.LogInformation("Creating ticket: {Title}", request.Title);
        // Implementation
    }
}
```

## LINQ Best Practices

### Prefer LINQ for Collections
```csharp
// Good - readable and efficient
var activeTickets = tickets
    .Where(t => t.Status == TicketStatus.Active)
    .OrderByDescending(t => t.CreatedDate)
    .Take(10)
    .ToList();

// Avoid - imperative style
var activeTickets = new List<Ticket>();
foreach (var ticket in tickets)
{
    if (ticket.Status == TicketStatus.Active)
        activeTickets.Add(ticket);
}
```

### Deferred Execution
```csharp
// Query is not executed until enumerated
var query = tickets.Where(t => t.Status == TicketStatus.Active);

// Executed here
var list = query.ToList();
var count = query.Count();
var first = query.FirstOrDefault();
```

### Async LINQ with EF Core
```csharp
// Good - async database query
var tickets = await _context.Tickets
    .Where(t => t.Status == TicketStatus.Active)
    .Include(t => t.Assignee)
    .ToListAsync();

// Avoid - synchronous database query
var tickets = _context.Tickets
    .Where(t => t.Status == TicketStatus.Active)
    .ToList(); // Blocks thread
```

## Error Handling

### Specific Exceptions
Create domain-specific exceptions:

```csharp
public class TicketNotFoundException : Exception
{
    public int TicketId { get; }
    
    public TicketNotFoundException(int ticketId) 
        : base($"Ticket with ID {ticketId} was not found")
    {
        TicketId = ticketId;
    }
}
```

### Exception Handling
```csharp
// Good - specific exception handling
public async Task<Ticket> GetTicketAsync(int id)
{
    try
    {
        var ticket = await _repository.GetByIdAsync(id);
        if (ticket is null)
        {
            throw new TicketNotFoundException(id);
        }
        return ticket;
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Database error while retrieving ticket {TicketId}", id);
        throw;
    }
}

// Avoid - catching generic Exception
try
{
    // Code
}
catch (Exception ex) // Too broad
{
    // Don't do this unless absolutely necessary
}
```

### Validation
```csharp
public class CreateTicketValidator
{
    public ValidationResult Validate(CreateTicketRequest request)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Title is required");
            
        if (request.Title?.Length > 200)
            errors.Add("Title cannot exceed 200 characters");
            
        return new ValidationResult(errors);
    }
}
```

## API Design (ASP.NET Core)

### Controller Structure
```csharp
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;
    
    public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetTicket(int id)
    {
        var ticket = await _ticketService.GetTicketAsync(id);
        if (ticket is null)
        {
            return NotFound();
        }
        return Ok(ticket);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var ticket = await _ticketService.CreateTicketAsync(request);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
    {
        try
        {
            await _ticketService.UpdateTicketAsync(id, request);
            return NoContent();
        }
        catch (TicketNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        await _ticketService.DeleteTicketAsync(id);
        return NoContent();
    }
}
```

### RESTful Conventions
- `GET /api/tickets` - List all tickets
- `GET /api/tickets/{id}` - Get specific ticket
- `POST /api/tickets` - Create new ticket
- `PUT /api/tickets/{id}` - Update entire ticket
- `PATCH /api/tickets/{id}` - Partial update
- `DELETE /api/tickets/{id}` - Delete ticket

### Status Codes
- `200 OK` - Successful GET, PUT, PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource doesn't exist
- `500 Internal Server Error` - Server error

## Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class TicketRepository : IRepository<Ticket>
{
    private readonly ApplicationDbContext _context;
    
    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Assignee)
            .Include(t => t.Machine)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
    
    // Other methods...
}
```

## Logging

Use structured logging with ILogger:

```csharp
// Good - structured logging
_logger.LogInformation(
    "Ticket {TicketId} assigned to {TechnicianName}", 
    ticketId, 
    technician.Name
);

_logger.LogWarning(
    "Ticket {TicketId} has been open for {Days} days", 
    ticket.Id, 
    (DateTime.Now - ticket.CreatedDate).Days
);

_logger.LogError(
    ex, 
    "Failed to update ticket {TicketId}", 
    ticketId
);

// Avoid - string concatenation
_logger.LogInformation($"Ticket {ticketId} assigned"); // Don't do this
```

## Testing

### Unit Tests Structure
```csharp
public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _mockRepository;
    private readonly Mock<ILogger<TicketService>> _mockLogger;
    private readonly TicketService _sut; // System Under Test
    
    public TicketServiceTests()
    {
        _mockRepository = new Mock<ITicketRepository>();
        _mockLogger = new Mock<ILogger<TicketService>>();
        _sut = new TicketService(_mockRepository.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetTicketAsync_WhenTicketExists_ReturnsTicket()
    {
        // Arrange
        var expectedTicket = new Ticket { Id = 1, Title = "Test" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedTicket);
        
        // Act
        var result = await _sut.GetTicketAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTicket.Id, result.Id);
    }
    
    [Fact]
    public async Task CreateTicketAsync_WithInvalidData_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateTicketRequest { Title = "" };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _sut.CreateTicketAsync(request)
        );
    }
}
```

## Common Pitfalls to Avoid

❌ **Don't:**
- Block async code with `.Result` or `.Wait()`
- Ignore nullable warnings
- Use `catch (Exception)` without rethrowing
- Mutate collections while iterating
- Return `null` from async methods (use `Task<T?>`)
- Put business logic in controllers
- Hard-code configuration values

✅ **Do:**
- Use `async`/`await` consistently
- Handle nulls explicitly
- Use specific exception types
- Use LINQ for collection operations
- Return `Task` or `Task<T?>` from async methods
- Keep controllers thin, logic in services
- Use configuration files and DI
