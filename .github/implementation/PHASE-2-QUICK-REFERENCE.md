# Phase 2: Application Layer - Quick Reference

## Overview

Phase 2 implements the **Application Layer** using **CQRS pattern with MediatR**. This layer orchestrates domain logic and provides use cases for the Presentation layer (API).

**Duration:** Sprint 2-3 (Week 3-5)  
**Tasks:** 120 tasks  
**GitHub Issues:** 10 comprehensive issues  
**Dependencies:** Phase 1 (Domain Layer) must be complete

---

## Architecture Pattern

### Vertical Slice Architecture

```
Application/
└── Features/
    ├── Tickets/
    │   ├── Commands/
    │   │   ├── CreateTicket/
    │   │   │   ├── CreateTicketCommand.cs          ← Request
    │   │   │   ├── CreateTicketCommandHandler.cs   ← Handler
    │   │   │   ├── CreateTicketCommandValidator.cs ← Validation
    │   │   │   └── TicketCreatedDto.cs            ← Response
    │   │   └── AssignTicket/...
    │   ├── Queries/
    │   │   ├── GetTickets/
    │   │   │   ├── GetTicketsQuery.cs
    │   │   │   ├── GetTicketsQueryHandler.cs
    │   │   │   ├── GetTicketsQueryValidator.cs
    │   │   │   └── TicketDto.cs
    │   │   └── GetTicketById/...
    │   └── EventHandlers/
    │       ├── TicketCreatedEventHandler.cs
    │       └── TicketAssignedEventHandler.cs
    ├── Organizations/...
    ├── Machines/...
    └── Users/...
```

Each feature is self-contained with minimal coupling.

---

## Key Technologies

| Technology | Version | Purpose |
|-----------|---------|---------|
| **MediatR** | 12.4.0+ | CQRS implementation |
| **FluentValidation** | 11.9.2+ | Request validation |
| **AutoMapper** | 13.0.1+ | Object mapping |
| **Ardalis.Result** | 9.0.0+ | Result pattern |
| **Ardalis.Specification** | 8.0.0+ | Query specifications |

---

## Implementation Order

### Phase 2A: Foundation (Week 3)
1. ✅ **Issue #10** - Repository Interfaces (2 days)
2. ✅ **Issue #11** - Application Infrastructure (3 days)

### Phase 2B: Core Features (Week 4)
3. ✅ **Issue #12** - Ticket Queries (3 days)
4. ✅ **Issue #13** - Ticket Commands (4 days)
5. ✅ **Issue #14** - Ticket Event Handlers (2 days)

### Phase 2C: Additional Features (Week 5)
6. ✅ **Issue #15** - Organizations Feature (3 days)
7. ✅ **Issue #16** - Machines Feature (3 days)
8. ✅ **Issue #17** - Users Feature (2-3 days)

### Phase 2D: Services & Testing (Parallel)
9. ✅ **Issue #18** - Application Services (2 days)
10. ✅ **Issue #19** - Unit Tests (4-5 days, parallel)

---

## CQRS Pattern Examples

### Command Pattern

```csharp
// 1. Command (Request)
public record CreateTicketCommand(
    Guid MachineId,
    string Title,
    string Description,
    Priority Priority
) : IRequest<Result<Guid>>;

// 2. Validator
public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(10).MaximumLength(255);
    }
}

// 3. Handler
public class CreateTicketCommandHandler 
    : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    private readonly ITicketRepository _repository;
    private readonly ITicketNumberGenerator _numberGenerator;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate business rules
        // 2. Generate domain objects
        // 3. Call domain methods
        // 4. Save changes
        // 5. Return result
        
        var ticketNumber = await _numberGenerator.GenerateNextNumberAsync(ct);
        
        var ticket = Ticket.Create(
            ticketNumber,
            request.Title,
            request.Description,
            machineId,
            request.Priority,
            _currentUser.UserId.Value
        );
        
        await _repository.AddAsync(ticket, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success(ticket.Id);
    }
}
```

### Query Pattern

```csharp
// 1. Query (Request)
public record GetTicketsQuery(
    int Page = 1,
    int PageSize = 20,
    TicketStatus? Status = null,
    string? Search = null
) : IRequest<Result<PaginatedList<TicketDto>>>;

// 2. Handler
public class GetTicketsQueryHandler 
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketDto>>>
{
    private readonly ITicketRepository _repository;
    private readonly IMapper _mapper;

    public async Task<Result<PaginatedList<TicketDto>>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Build specification
        var spec = new GetTicketsSpecification(request);
        
        // 2. Query repository
        var tickets = await _repository.ListAsync(spec, ct);
        var totalCount = await _repository.CountAsync(spec, ct);
        
        // 3. Map to DTOs
        var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);
        
        // 4. Return paginated result
        return Result.Success(new PaginatedList<TicketDto>(
            ticketDtos, totalCount, request.Page, request.PageSize
        ));
    }
}

// 3. DTO
public class TicketDto : IMapFrom<Ticket>
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Ticket, TicketDto>()
            .ForMember(d => d.TicketNumber, opt => opt.MapFrom(s => s.TicketNumber.ToString()));
    }
}
```

### Event Handler Pattern

```csharp
public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public async Task Handle(
        TicketCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} created by {UserId}",
            notification.TicketId,
            notification.CreatedByUserId);

        // Send notification email (future)
        // await _emailService.SendTicketCreatedNotificationAsync(...);
    }
}
```

---

## Pipeline Behaviors

Behaviors execute in order around handlers:

```
Request → Unhandled Exception → Validation → Performance → Logging → Handler → Response
```

### Validation Behavior

```csharp
public class ValidationBehaviour<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, ct)));
        
        var failures = results.SelectMany(r => r.Errors).ToList();
        
        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

## Testing Strategy

### Unit Test Structure

```csharp
public class CreateTicketCommandHandlerTests
{
    private readonly Mock<ITicketRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITicketRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateTicketCommandHandler(/* ... */);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTicketSuccessfully()
    {
        // Arrange
        var command = new CreateTicketCommand(/* ... */);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MachineNotFound_ReturnsNotFound()
    {
        // Arrange, Act, Assert
    }
}
```

### Test Coverage Goals

- **Commands:** 90%+
- **Queries:** 85%+
- **Validators:** 100%
- **Event Handlers:** 80%+
- **Behaviors:** 95%+

---

## Common Interfaces

### IApplicationDbContext

```csharp
public interface IApplicationDbContext
{
    DbSet<Ticket> Tickets { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Machine> Machines { get; }
    DbSet<ServiceUser> ServiceUsers { get; }
    DbSet<OrganizationUser> OrganizationUsers { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

### ICurrentUserService

```csharp
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? OrganizationId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IEnumerable<string> Roles { get; }
}
```

### IUnitOfWork

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

---

## Dependency Injection

```csharp
// DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        // Pipeline Behaviors (order matters!)
        services.AddTransient(typeof(IPipelineBehavior<,>), 
            typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), 
            typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), 
            typeof(PerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), 
            typeof(LoggingBehaviour<,>));
        
        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // Application Services
        services.AddScoped<ITicketNumberGenerator, TicketNumberGeneratorService>();
        
        return services;
    }
}

// In Program.cs or Startup.cs
services.AddApplicationServices();
```

---

## Best Practices

### ✅ DO

1. **Keep handlers thin** - Orchestrate, don't implement business logic
2. **Domain logic in domain** - Handlers call domain entity methods
3. **Validate early** - Use FluentValidation in pipeline
4. **Async all the way** - All I/O operations async
5. **Use CancellationToken** - Proper cancellation support
6. **Immutable commands/queries** - Use records
7. **Result pattern** - Return Result<T>, not throw exceptions for business failures
8. **Test independently** - Mock dependencies

### ❌ DON'T

1. **Don't put business logic in handlers** - Use domain entities
2. **Don't return null** - Use Result.NotFound()
3. **Don't catch all exceptions** - Let UnhandledExceptionBehaviour handle it
4. **Don't use Task.Result** - Always await
5. **Don't ignore CancellationToken** - Pass it through
6. **Don't create anemic models** - DTOs are for transfer only

---

## Naming Conventions

### Commands

- Format: `{Verb}{Entity}Command`
- Examples:
  - `CreateTicketCommand`
  - `AssignTicketCommand`
  - `UpdateTicketStatusCommand`
  - `DeleteMachineCommand`

### Queries

- Format: `Get{Entity}[s][By{Criteria}]Query`
- Examples:
  - `GetTicketsQuery` (list)
  - `GetTicketByIdQuery` (single)
  - `GetTicketsByOrganizationQuery`
  - `GetMachinesDueForMaintenanceQuery`

### DTOs

- Format: `{Entity}Dto` or `{Entity}DetailDto`
- Examples:
  - `TicketDto` (summary)
  - `TicketDetailDto` (full details)
  - `OrganizationDto`
  - `MachineDto`

### Handlers

- Format: `{Command/Query}Handler`
- Examples:
  - `CreateTicketCommandHandler`
  - `GetTicketsQueryHandler`

### Validators

- Format: `{Command/Query}Validator`
- Examples:
  - `CreateTicketCommandValidator`
  - `GetTicketsQueryValidator`

---

## Checklist Before Starting

- [ ] Phase 1 (Domain Layer) is 100% complete
- [ ] All domain entities implemented
- [ ] All value objects implemented
- [ ] All domain events implemented
- [ ] Domain layer compiles with 0 errors
- [ ] Domain unit tests written and passing
- [ ] Repository interfaces defined
- [ ] Have access to API plan and PRD
- [ ] Understand CQRS pattern
- [ ] Understand MediatR pipeline
- [ ] Development environment setup

---

## Success Criteria for Phase 2

- [ ] All repository interfaces defined
- [ ] All CQRS Commands/Queries for core features (Tickets, Organizations, Machines)
- [ ] FluentValidation rules for all commands/queries
- [ ] AutoMapper profiles for all DTOs
- [ ] Pipeline behaviors working correctly
- [ ] All domain events have handlers
- [ ] Unit test coverage > 80%
- [ ] All commands/queries follow consistent patterns
- [ ] Proper error handling with Result pattern
- [ ] Zero compilation errors/warnings
- [ ] DependencyInjection.cs configured
- [ ] All interfaces documented with XML comments

---

## Next Steps After Phase 2

**Phase 3: Infrastructure Layer** will implement:
- EF Core DbContext (`IApplicationDbContext`)
- Repository implementations
- Supabase integration
- Authentication/Authorization (`ICurrentUserService`, `IIdentityService`)
- File storage (`IFileStorageService`)
- Email service (`IEmailService`)
- Background jobs
- Database migrations

---

## Useful Resources

### Documentation
- [MediatR Wiki](https://github.com/jbogard/MediatR/wiki)
- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- [AutoMapper Docs](https://docs.automapper.org/)
- [Ardalis.Result GitHub](https://github.com/ardalis/Result)
- [Ardalis.Specification GitHub](https://github.com/ardalis/Specification)

### Project Files
- Phase 2 Plan: `.github/implementation/PHASE-2-APPLICATION.md`
- GitHub Issues: `.github/implementation/PHASE-2-GITHUB-ISSUES.md`
- Current Progress: `.github/implementation/CURRENT-PROGRESS.md`
- API Plan: `.ai/api-plan.md`
- PRD: `.ai/PRD.md`

---

## Quick Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Run with watch
dotnet watch run --project src/backend/Flowertrack.Api

# Add package to Application layer
cd src/backend/Flowertrack.Application
dotnet add package MediatR
```

---

**Last Updated:** 2025-10-26  
**Version:** 1.0  
**Next Review:** After Phase 2 completion
