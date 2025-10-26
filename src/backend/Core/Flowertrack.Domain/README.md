# Repository Pattern Implementation

This document describes the repository pattern implementation in the FLOWerTRACK Domain layer.

## Overview

The Repository pattern is implemented following Clean Architecture principles and Domain-Driven Design (DDD). Repository interfaces are defined in the **Domain layer** (contracts), while their implementations will be provided in the **Infrastructure layer** (Phase 3).

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                            │
│                  (Flowertrack.Api)                       │
└─────────────────────┬───────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│                Application Layer                         │
│              (Flowertrack.Application)                   │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │           IUnitOfWork Interface                  │  │
│  │  • Coordinates multiple repositories             │  │
│  │  • Manages transactions                          │  │
│  │  • Ensures data consistency                      │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────┬───────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│                  Domain Layer                            │
│               (Flowertrack.Domain)                       │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │        Repository Interfaces (Contracts)         │  │
│  │  • IRepository<T> (base)                         │  │
│  │  • ITicketRepository                             │  │
│  │  • IOrganizationRepository                       │  │
│  │  • IMachineRepository                            │  │
│  │  • IServiceUserRepository                        │  │
│  │  • IOrganizationUserRepository                   │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │              Domain Entities                     │  │
│  │  • Ticket (Aggregate Root)                       │  │
│  │  • Organization (Aggregate Root)                 │  │
│  │  • Machine (Aggregate Root)                      │  │
│  │  • ServiceUser                                   │  │
│  │  • OrganizationUser                              │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                      ▲
                      │
┌─────────────────────┴───────────────────────────────────┐
│              Infrastructure Layer                        │
│            (Flowertrack.Infrastructure)                  │
│                    [Phase 3]                             │
│                                                          │
│  • Repository implementations (EF Core)                  │
│  • UnitOfWork implementation                             │
│  • DbContext                                             │
└─────────────────────────────────────────────────────────┘
```

## Base Repository Interface

The `IRepository<T>` interface provides common CRUD operations for all entities:

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
```

## Specialized Repository Interfaces

Each aggregate root has its own repository interface with custom query methods:

### ITicketRepository

Specialized methods for ticket management:

- `GetByTicketNumberAsync(ticketNumber)` - Find ticket by unique ticket number
- `GetByOrganizationIdAsync(organizationId)` - Get all tickets for an organization
- `GetByMachineIdAsync(machineId)` - Get all tickets for a specific machine
- `GetByAssignedUserIdAsync(userId)` - Get tickets assigned to a user
- `GetByStatusAsync(status)` - Filter tickets by status
- `GetNextSequentialNumberAsync(year)` - Get next ticket number for the year
- `TicketNumberExistsAsync(ticketNumber)` - Check if ticket number exists

### IOrganizationRepository

Specialized methods for organization management:

- `GetByNameAsync(name)` - Find organization by name
- `GetByApiKeyAsync(apiKey)` - Find organization by API key (for integrations)
- `NameExistsAsync(name, excludeId)` - Check name uniqueness
- `ApiKeyExistsAsync(apiKey, excludeId)` - Check API key uniqueness
- `GetActiveOrganizationsAsync()` - Get organizations with valid contracts
- `GetExpiredContractsAsync()` - Get organizations with expired contracts

### IMachineRepository

Specialized methods for machine management:

- `GetBySerialNumberAsync(serialNumber)` - Find machine by serial number
- `GetByApiTokenAsync(apiToken)` - Find machine by API token (for machine logs)
- `GetByOrganizationIdAsync(organizationId)` - Get all machines for an organization
- `GetByStatusAsync(status)` - Filter machines by operational status
- `SerialNumberExistsAsync(serialNumber, excludeId)` - Check serial number uniqueness
- `ApiTokenExistsAsync(apiToken, excludeId)` - Check API token uniqueness
- `GetMachinesDueForMaintenanceAsync(beforeDate)` - Get machines due for maintenance

### IServiceUserRepository

Specialized methods for service user management:

- `GetByEmailAsync(email)` - Find service user by email
- `GetActiveUsersAsync()` - Get all active service users
- `GetAvailableUsersAsync()` - Get users available for ticket assignment
- `EmailExistsAsync(email, excludeId)` - Check email uniqueness

### IOrganizationUserRepository

Specialized methods for organization user management:

- `GetByEmailAsync(email)` - Find organization user by email
- `GetByOrganizationIdAsync(organizationId)` - Get all users for an organization
- `EmailExistsAsync(email, excludeId)` - Check email uniqueness

## Unit of Work Pattern

The `IUnitOfWork` interface coordinates multiple repositories and manages transactions:

```csharp
public interface IUnitOfWork : IDisposable
{
    ITicketRepository Tickets { get; }
    IOrganizationRepository Organizations { get; }
    IMachineRepository Machines { get; }
    IServiceUserRepository ServiceUsers { get; }
    IOrganizationUserRepository OrganizationUsers { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

### Benefits of Unit of Work

1. **Transaction Management**: Group multiple operations into a single atomic unit
2. **Consistency**: Ensures all changes are saved together or none at all
3. **Coordination**: Single point to access all repositories
4. **Testability**: Easy to mock for unit tests

## Usage Examples

### Basic CRUD Operations

```csharp
// Using IUnitOfWork in a command handler
public class CreateTicketCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Ticket> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        // Validate machine exists
        var machine = await _unitOfWork.Machines.GetByIdAsync(request.MachineId, ct);
        if (machine == null)
            throw new NotFoundException("Machine not found");
        
        // Create ticket
        var ticket = Ticket.Create(request.Title, request.Description, ...);
        
        // Add to repository
        await _unitOfWork.Tickets.AddAsync(ticket, ct);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync(ct);
        
        return ticket;
    }
}
```

### Transaction Example

```csharp
public class AssignTicketCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task Handle(AssignTicketCommand request, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        try
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId, ct);
            var user = await _unitOfWork.ServiceUsers.GetByIdAsync(request.UserId, ct);
            
            if (!user.IsAvailable)
                throw new DomainException("User is not available");
            
            ticket.AssignTo(request.UserId, request.AssignedBy);
            
            await _unitOfWork.Tickets.UpdateAsync(ticket, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
            await _unitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
```

### Query Examples

```csharp
// Get all active organizations
var activeOrgs = await _unitOfWork.Organizations.GetActiveOrganizationsAsync(ct);

// Get tickets by status
var newTickets = await _unitOfWork.Tickets.GetByStatusAsync(TicketStatus.New, ct);

// Get available technicians
var availableTechs = await _unitOfWork.ServiceUsers.GetAvailableUsersAsync(ct);

// Get machines due for maintenance
var dueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
var machines = await _unitOfWork.Machines.GetMachinesDueForMaintenanceAsync(dueDate, ct);
```

## Design Principles

### 1. Dependency Inversion Principle
- High-level modules (Application layer) depend on abstractions (interfaces)
- Low-level modules (Infrastructure) implement these abstractions
- Domain layer contains only interfaces, no implementations

### 2. Separation of Concerns
- **Domain**: Business logic and contracts (what)
- **Application**: Use cases and orchestration (how)
- **Infrastructure**: Technical implementation details (where/with what)

### 3. Repository Pattern Benefits
- **Abstraction**: Hides data access complexity
- **Testability**: Easy to mock for unit tests
- **Flexibility**: Can change data source without affecting domain
- **Centralization**: Query logic in one place

### 4. Unit of Work Benefits
- **Atomicity**: All or nothing transactions
- **Consistency**: Ensures data integrity
- **Single Source**: One place to access all repositories

## Testing Strategy

### Unit Tests with Mocks

```csharp
[Fact]
public async Task CreateTicket_WhenMachineExists_ShouldSucceed()
{
    // Arrange
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockMachineRepo = new Mock<IMachineRepository>();
    
    var machine = new Machine { Id = Guid.NewGuid(), ... };
    mockMachineRepo
        .Setup(r => r.GetByIdAsync(machine.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(machine);
    
    mockUnitOfWork.Setup(u => u.Machines).Returns(mockMachineRepo.Object);
    
    var handler = new CreateTicketCommandHandler(mockUnitOfWork.Object);
    
    // Act
    var result = await handler.Handle(new CreateTicketCommand { ... }, default);
    
    // Assert
    Assert.NotNull(result);
    mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}
```

## Value Objects

### TicketNumber

The `TicketNumber` value object ensures ticket numbers follow the format `TICK-YYYY-XXXXX`:

```csharp
// Create a ticket number
var ticketNumber = TicketNumber.Create(2025, 1);
Console.WriteLine(ticketNumber.Value); // "TICK-2025-00001"

// Parse from string
var parsed = TicketNumber.Parse("TICK-2025-00042");
```

## Enums

### TicketStatus
- `Draft`, `New`, `Assigned`, `InProgress`, `Resolved`, `Closed`, `Reopened`

### MachineStatus
- `Active`, `Inactive`, `Maintenance`, `Alarm`, `Retired`

## Implementation Roadmap

### Phase 1.4 (Current) ✅
- [x] Repository interface definitions
- [x] Unit of Work interface
- [x] Placeholder entities
- [x] Value objects (TicketNumber)
- [x] Enums (TicketStatus, MachineStatus)

### Phase 2 (Commands & Queries)
- [ ] CQRS pattern implementation
- [ ] Command handlers using repositories
- [ ] Query handlers using repositories

### Phase 3 (Infrastructure)
- [ ] Repository implementations with EF Core
- [ ] UnitOfWork implementation
- [ ] DbContext configuration
- [ ] Database migrations

## Best Practices

1. **Always use repositories through Unit of Work** - Ensures transaction consistency
2. **Use CancellationToken** - Support cancellation for long-running operations
3. **Return IReadOnlyList** - Prevents accidental modifications of query results
4. **Include excludeId parameter** - For uniqueness checks during updates
5. **Use nullable return types** - Explicitly handle not-found scenarios
6. **Follow naming conventions** - `GetBy...Async`, `...ExistsAsync`, etc.

## Related Documentation

- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [Domain-Driven Design](https://domainlanguage.com/ddd/)

## Dependencies

This implementation depends on:
- Issue #1: Ticket entity (full implementation)
- Issue #2: Machine entity (full implementation)
- Issue #3: Organization entity (full implementation)
- Issue #4: User entities (full implementation)

## Next Steps

After full entity implementations (Issues #1-4), the Infrastructure layer will provide:
- Entity Framework Core configurations
- Repository implementations
- UnitOfWork implementation
- Database migrations
- Specification pattern support (optional)

---

**Note**: Current placeholder entities provide minimal structure for interface compilation. Full domain logic will be implemented in Phase 1.1 (Issues #1-4).
