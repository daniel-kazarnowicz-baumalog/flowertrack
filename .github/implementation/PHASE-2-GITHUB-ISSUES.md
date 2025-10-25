# Phase 2: Application Layer - GitHub Issues

This document contains detailed GitHub issues for Phase 2 implementation. Each issue is designed to be clear, actionable, and independently completable.

---

## Issue Template Format

Each issue should include:
- Clear title
- Description with context
- Technical requirements
- Acceptance criteria
- Related files/dependencies
- Labels and estimates

---

## 🔥 Critical Priority Issues

### Issue #10: Implement Repository Interfaces (Phase 1 Completion)

**Title:** Implement Domain Repository Interfaces

**Labels:** `phase-1`, `domain`, `priority:critical`, `type:feature`  
**Milestone:** Phase 1 - Domain Layer  
**Estimate:** 2 days

**Description:**

Complete Phase 1 by implementing all repository interfaces in the Domain layer. These interfaces define contracts for data access without coupling the domain to specific persistence technologies.

**Context:**

Repository pattern abstracts data access, allowing the Domain layer to remain persistence-ignorant. These interfaces will be implemented in the Infrastructure layer (Phase 3).

**Technical Requirements:**

1. Create base `IRepository<T>` interface in `Flowertrack.Domain/Repositories/`
2. Create specialized repository interfaces:
   - `ITicketRepository`
   - `IOrganizationRepository`
   - `IMachineRepository`
   - `IServiceUserRepository`
   - `IOrganizationUserRepository`
3. Create `IUnitOfWork` interface in `Flowertrack.Application/Common/Interfaces/`

**Files to Create:**

```
Flowertrack.Domain/Repositories/
├── IRepository.cs
├── ITicketRepository.cs
├── IOrganizationRepository.cs
├── IMachineRepository.cs
├── IServiceUserRepository.cs
└── IOrganizationUserRepository.cs

Flowertrack.Application/Common/Interfaces/
└── IUnitOfWork.cs
```

**Acceptance Criteria:**

- [x] `IRepository<T>` includes standard CRUD operations
- [x] `IRepository<T>` supports Ardalis.Specification pattern
- [x] All methods use `async/await` with `CancellationToken`
- [x] All interfaces have XML documentation
- [x] Specialized repositories extend `IRepository<T>`
- [x] Each specialized repository has domain-specific query methods
- [x] `IUnitOfWork` includes transaction support
- [x] No compilation errors
- [x] Follows C# naming conventions

**Example Code:**

```csharp
public interface IRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) 
        where TId : notnull;
    
    /// <summary>
    /// Lists all entities matching the specification.
    /// </summary>
    Task<List<T>> ListAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    // ... other methods
}

public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Gets a ticket by its unique ticket number.
    /// </summary>
    Task<Ticket?> GetByNumberAsync(TicketNumber number, 
        CancellationToken cancellationToken = default);
    
    // ... other specialized methods
}
```

**Dependencies:**

- Phase 1 entities must be complete
- Ardalis.Specification package

**Related Issues:** #4, #5, #6

---

### Issue #11: Setup Application Layer Infrastructure

**Title:** Configure Application Layer Foundation (MediatR, FluentValidation, AutoMapper)

**Labels:** `phase-2`, `application`, `priority:critical`, `type:infrastructure`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 3 days

**Description:**

Setup the foundational infrastructure for the Application layer including CQRS with MediatR, validation pipeline with FluentValidation, and object mapping with AutoMapper.

**Context:**

The Application layer orchestrates domain logic through CQRS pattern. This issue establishes the core infrastructure needed for all commands, queries, and behaviors.

**Technical Requirements:**

1. **Add NuGet Packages:**
   - MediatR (v12.4.0+)
   - FluentValidation (v11.9.2+)
   - FluentValidation.DependencyInjectionExtensions
   - AutoMapper (v13.0.1+)
   - Ardalis.Result (v9.0.0+)
   - Ardalis.Specification (v8.0.0+)

2. **Create Common Interfaces:**
   - `IApplicationDbContext` - DbContext abstraction
   - `ICurrentUserService` - User context
   - `IDateTime` - Testable date/time
   - `IIdentityService` - User management
   - `IEmailService` - Email notifications

3. **Create Pipeline Behaviors:**
   - `LoggingBehaviour` - Request/response logging
   - `ValidationBehaviour` - FluentValidation integration
   - `PerformanceBehaviour` - Performance monitoring
   - `UnhandledExceptionBehaviour` - Exception handling
   - `AuthorizationBehaviour` - Permission checking

4. **Create Common Models:**
   - `PaginatedList<T>` - Generic pagination
   - Base mapping interfaces

5. **Create DependencyInjection.cs**

**Files to Create:**

```
Flowertrack.Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   ├── ICurrentUserService.cs
│   │   ├── IDateTime.cs
│   │   ├── IIdentityService.cs
│   │   ├── IEmailService.cs
│   │   └── IUnitOfWork.cs
│   ├── Models/
│   │   └── PaginatedList.cs
│   ├── Mappings/
│   │   ├── IMapFrom.cs
│   │   └── MappingProfile.cs
│   ├── Behaviours/
│   │   ├── LoggingBehaviour.cs
│   │   ├── ValidationBehaviour.cs
│   │   ├── PerformanceBehaviour.cs
│   │   ├── UnhandledExceptionBehaviour.cs
│   │   └── AuthorizationBehaviour.cs
│   └── Exceptions/
│       ├── ValidationException.cs
│       ├── NotFoundException.cs
│       ├── ForbiddenAccessException.cs
│       └── ConflictException.cs
└── DependencyInjection.cs
```

**Acceptance Criteria:**

- [x] All NuGet packages installed and referenced
- [x] `IApplicationDbContext` defines DbSet properties for all entities
- [x] `ICurrentUserService` provides user context (Id, Email, Roles)
- [x] All pipeline behaviors implement `IPipelineBehavior<,>`
- [x] `ValidationBehaviour` integrates FluentValidation validators
- [x] `PerformanceBehaviour` logs slow requests (>500ms threshold)
- [x] `LoggingBehaviour` logs all requests/responses
- [x] `PaginatedList<T>` supports pagination metadata
- [x] `DependencyInjection.cs` registers all services
- [x] Behaviors execute in correct order
- [x] Zero compilation errors
- [x] XML documentation for all public types

**Example Code:**

```csharp
// DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
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
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        return services;
    }
}

// ValidationBehaviour.cs
public class ValidationBehaviour<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

**Dependencies:**

- Domain layer complete

**Testing:**

Create unit tests in `Flowertrack.Application.Tests/Common/Behaviours/`:
- ValidationBehaviourTests.cs
- PerformanceBehaviourTests.cs
- LoggingBehaviourTests.cs

**Related Issues:** #10

---

### Issue #12: Implement Tickets Feature - Queries

**Title:** Implement Ticket Queries (GetTickets, GetTicketById, GetTicketHistory)

**Labels:** `phase-2`, `application`, `feature:tickets`, `priority:critical`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 3 days

**Description:**

Implement CQRS queries for the Tickets feature, including list view with filtering/sorting, detail view, and history timeline.

**User Stories:**

- US-010: As a Service Technician, I want to view a list of all tickets with filtering options
- US-015: As a Service Technician, I want to view full details of a ticket
- US-016: As a User, I want to see the complete event timeline for a ticket

**Technical Requirements:**

1. **GetTicketsQuery:**
   - Pagination support (page, pageSize)
   - Filtering: status, priority, organization, machine, assignedTo, search
   - Sorting: by createdAt, updatedAt, priority (asc/desc)
   - Returns `PaginatedList<TicketDto>`

2. **GetTicketByIdQuery:**
   - Load ticket with all navigation properties
   - Returns `TicketDetailDto` with nested objects
   - Permission check: user can only see tickets from their organization (unless service team)

3. **GetTicketHistoryQuery:**
   - Returns timeline of all ticket events
   - Chronological order (newest first)
   - Returns `List<TicketHistoryEventDto>`
   - Filter internal notes for organization users

**Files to Create:**

```
Flowertrack.Application/Features/Tickets/
├── Queries/
│   ├── GetTickets/
│   │   ├── GetTicketsQuery.cs
│   │   ├── GetTicketsQueryHandler.cs
│   │   ├── GetTicketsQueryValidator.cs
│   │   └── TicketDto.cs
│   ├── GetTicketById/
│   │   ├── GetTicketByIdQuery.cs
│   │   ├── GetTicketByIdQueryHandler.cs
│   │   ├── GetTicketByIdQueryValidator.cs
│   │   └── TicketDetailDto.cs
│   └── GetTicketHistory/
│       ├── GetTicketHistoryQuery.cs
│       ├── GetTicketHistoryQueryHandler.cs
│       ├── GetTicketHistoryQueryValidator.cs
│       └── TicketHistoryEventDto.cs
```

**Acceptance Criteria:**

- [x] All queries return `Result<T>` from Ardalis.Result
- [x] All queries have validators with FluentValidation
- [x] All handlers use `ITicketRepository`
- [x] DTOs use AutoMapper for mapping
- [x] Proper error handling (NotFound, Forbidden)
- [x] GetTickets supports all filters from api-plan.md
- [x] GetTickets returns correct pagination metadata
- [x] GetTicketById includes Organization, Machine, Users
- [x] GetTicketHistory filters internal notes for clients
- [x] All async methods use CancellationToken
- [x] Unit tests with 85%+ coverage
- [x] XML documentation

**Example Code:**

```csharp
// GetTicketsQuery.cs
public record GetTicketsQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "CreatedAt",
    string SortOrder = "desc",
    TicketStatus? Status = null,
    Priority? Priority = null,
    Guid? OrganizationId = null,
    Guid? MachineId = null,
    Guid? AssignedToUserId = null,
    string? Search = null
) : IRequest<Result<PaginatedList<TicketDto>>>;

// GetTicketsQueryHandler.cs
public class GetTicketsQueryHandler 
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketDto>>>
{
    private readonly ITicketRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetTicketsQueryHandler(
        ITicketRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<TicketDto>>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        // Build specification
        var spec = new GetTicketsSpecification(
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortOrder,
            request.Status,
            request.Priority,
            request.OrganizationId,
            request.MachineId,
            request.AssignedToUserId,
            request.Search,
            _currentUser.UserId,
            _currentUser.IsInRole("ServiceTechnician")
        );

        var tickets = await _repository.ListAsync(spec, cancellationToken);
        var totalCount = await _repository.CountAsync(spec, cancellationToken);

        var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);

        var result = new PaginatedList<TicketDto>(
            ticketDtos,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result.Success(result);
    }
}

// GetTicketsQueryValidator.cs
public class GetTicketsQueryValidator : AbstractValidator<GetTicketsQuery>
{
    public GetTicketsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage("Invalid sort field.");

        RuleFor(x => x.SortOrder)
            .Must(x => x == "asc" || x == "desc")
            .WithMessage("SortOrder must be 'asc' or 'desc'.");
    }

    private bool BeValidSortField(string? sortBy)
    {
        var validFields = new[] { "CreatedAt", "UpdatedAt", "Priority", "Status" };
        return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}

// TicketDto.cs
public class TicketDto : IMapFrom<Ticket>
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string MachineSerialNumber { get; set; } = string.Empty;
    public string? AssignedToName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Ticket, TicketDto>()
            .ForMember(d => d.TicketNumber, opt => opt.MapFrom(s => s.TicketNumber.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()))
            .ForMember(d => d.OrganizationName, opt => opt.MapFrom(s => s.Organization.Name))
            .ForMember(d => d.MachineSerialNumber, opt => opt.MapFrom(s => s.Machine.SerialNumber))
            .ForMember(d => d.AssignedToName, opt => opt.MapFrom(s => 
                s.AssignedTo != null ? $"{s.AssignedTo.FirstName} {s.AssignedTo.LastName}" : null));
    }
}
```

**Dependencies:**

- Issue #10 (Repository Interfaces)
- Issue #11 (Application Infrastructure)

**Testing:**

Unit tests in `Flowertrack.Application.Tests/Features/Tickets/Queries/`:
- GetTicketsQueryHandlerTests.cs
- GetTicketsQueryValidatorTests.cs
- GetTicketByIdQueryHandlerTests.cs
- GetTicketHistoryQueryHandlerTests.cs

**Related Issues:** #13

---

### Issue #13: Implement Tickets Feature - Commands

**Title:** Implement Ticket Commands (Create, Update, Assign, ChangeStatus, etc.)

**Labels:** `phase-2`, `application`, `feature:tickets`, `priority:critical`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 4 days

**Description:**

Implement all CQRS commands for the Tickets feature including create, update, assign, status changes, comments, and attachments.

**User Stories:**

- US-042: As an Organization User, I want to create a new ticket
- US-018: As a Service Technician, I want to add internal notes
- US-019: As a Service Admin, I want to assign tickets to technicians
- US-017: As a Service Technician, I want to change ticket status
- US-047: As an Organization Admin, I want to reopen resolved tickets

**Technical Requirements:**

Implement the following commands:

1. **CreateTicketCommand** - Create new ticket
2. **UpdateTicketCommand** - Update title/description
3. **AssignTicketCommand** - Assign to technician
4. **ChangeTicketStatusCommand** - Change status with validation
5. **ResolveTicketCommand** - Mark as resolved
6. **CloseTicketCommand** - Close ticket
7. **ReopenTicketCommand** - Reopen within 14 days
8. **AddCommentCommand** - Add public comment
9. **AddNoteCommand** - Add internal note (service team only)
10. **AddAttachmentCommand** - Upload file

**Files to Create:**

```
Flowertrack.Application/Features/Tickets/
├── Commands/
│   ├── CreateTicket/
│   │   ├── CreateTicketCommand.cs
│   │   ├── CreateTicketCommandHandler.cs
│   │   ├── CreateTicketCommandValidator.cs
│   │   └── TicketCreatedDto.cs
│   ├── UpdateTicket/
│   │   ├── UpdateTicketCommand.cs
│   │   ├── UpdateTicketCommandHandler.cs
│   │   └── UpdateTicketCommandValidator.cs
│   ├── AssignTicket/
│   │   ├── AssignTicketCommand.cs
│   │   ├── AssignTicketCommandHandler.cs
│   │   └── AssignTicketCommandValidator.cs
│   ├── ChangeTicketStatus/
│   │   ├── ChangeTicketStatusCommand.cs
│   │   ├── ChangeTicketStatusCommandHandler.cs
│   │   └── ChangeTicketStatusCommandValidator.cs
│   ├── ResolveTicket/
│   ├── CloseTicket/
│   ├── ReopenTicket/
│   ├── AddComment/
│   ├── AddNote/
│   └── AddAttachment/
```

**Acceptance Criteria:**

- [x] All commands return `Result<T>` or `Result`
- [x] All commands have validators
- [x] Handlers delegate to domain entity methods
- [x] Commands use `ICurrentUserService` for user context
- [x] Proper authorization checks
- [x] Domain events are published automatically
- [x] CreateTicket generates TicketNumber via domain service
- [x] AssignTicket validates assignee is active service user
- [x] ChangeStatus validates state transitions
- [x] ReopenTicket checks 14-day window
- [x] AddNote only accessible to service team
- [x] AddAttachment validates file size and mime type
- [x] All transactions use UnitOfWork
- [x] Unit tests with 90%+ coverage
- [x] Integration tests for critical commands

**Example Code:**

```csharp
// CreateTicketCommand.cs
public record CreateTicketCommand(
    Guid MachineId,
    string Title,
    string Description,
    Priority Priority
) : IRequest<Result<Guid>>;

// CreateTicketCommandHandler.cs
public class CreateTicketCommandHandler 
    : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly ITicketNumberGenerator _numberGenerator;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IMachineRepository machineRepository,
        ITicketNumberGenerator numberGenerator,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _machineRepository = machineRepository;
        _numberGenerator = numberGenerator;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        // Validate machine exists and user has access
        var machine = await _machineRepository.GetByIdAsync(
            request.MachineId, 
            cancellationToken);

        if (machine is null)
            return Result.NotFound("Machine not found.");

        // Check permissions (simplified - actual logic in AuthorizationBehaviour)
        if (!_currentUser.IsInRole("ServiceTechnician") && 
            machine.OrganizationId != _currentUser.OrganizationId)
        {
            return Result.Forbidden();
        }

        // Generate ticket number
        var ticketNumber = await _numberGenerator.GenerateNextNumberAsync(
            cancellationToken);

        // Create ticket (domain entity constructor handles business logic)
        var ticket = Ticket.Create(
            ticketNumber,
            request.Title,
            request.Description,
            machine.OrganizationId,
            request.MachineId,
            request.Priority,
            _currentUser.UserId!.Value
        );

        // Save to repository
        await _ticketRepository.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Domain events are published automatically via EF Core

        return Result.Success(ticket.Id);
    }
}

// CreateTicketCommandValidator.cs
public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MinimumLength(10)
            .WithMessage("Title must be at least 10 characters.")
            .MaximumLength(255)
            .WithMessage("Title cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description cannot exceed 5000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority value.");
    }
}

// ChangeTicketStatusCommand.cs
public record ChangeTicketStatusCommand(
    Guid TicketId,
    TicketStatus NewStatus,
    string? Justification = null
) : IRequest<Result>;

// ChangeTicketStatusCommandHandler.cs
public class ChangeTicketStatusCommandHandler 
    : IRequestHandler<ChangeTicketStatusCommand, Result>
{
    private readonly ITicketRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTicketStatusCommandHandler(
        ITicketRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ChangeTicketStatusCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(
            request.TicketId, 
            cancellationToken);

        if (ticket is null)
            return Result.NotFound("Ticket not found.");

        try
        {
            // Domain entity handles state transition validation
            ticket.UpdateStatus(
                request.NewStatus,
                request.Justification ?? string.Empty,
                _currentUser.UserId!.Value
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            // Domain exceptions for invalid transitions
            return Result.Error(ex.Message);
        }
    }
}
```

**Dependencies:**

- Issue #10 (Repository Interfaces)
- Issue #11 (Application Infrastructure)
- Issue #12 (Ticket Queries)

**Testing:**

Unit tests in `Flowertrack.Application.Tests/Features/Tickets/Commands/`:
- CreateTicketCommandHandlerTests.cs
- CreateTicketCommandValidatorTests.cs
- AssignTicketCommandHandlerTests.cs
- ChangeTicketStatusCommandHandlerTests.cs
- ReopenTicketCommandHandlerTests.cs

**Related Issues:** #12, #14

---

## 🟡 High Priority Issues

### Issue #14: Implement Ticket Event Handlers

**Title:** Implement Domain Event Handlers for Ticket Events

**Labels:** `phase-2`, `application`, `feature:tickets`, `priority:high`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 2 days

**Description:**

Implement event handlers for all ticket-related domain events. Event handlers handle side effects like notifications, logging, and triggering other processes.

**Context:**

Domain events are raised by entities when significant business actions occur. Event handlers in the Application layer process these events asynchronously.

**Technical Requirements:**

Implement handlers for:
1. `TicketCreatedEvent` - Log creation, send notifications (future)
2. `TicketAssignedEvent` - Notify assigned technician
3. `TicketStatusChangedEvent` - Notify stakeholders
4. `TicketResolvedEvent` - Send resolution email
5. `TicketClosedEvent` - Archive/cleanup (future)
6. `TicketReopenedEvent` - Notify team

**Files to Create:**

```
Flowertrack.Application/Features/Tickets/EventHandlers/
├── TicketCreatedEventHandler.cs
├── TicketAssignedEventHandler.cs
├── TicketStatusChangedEventHandler.cs
├── TicketResolvedEventHandler.cs
├── TicketClosedEventHandler.cs
└── TicketReopenedEventHandler.cs
```

**Acceptance Criteria:**

- [x] All handlers implement `INotificationHandler<TEvent>`
- [x] Handlers are registered automatically by MediatR
- [x] Handlers use `ILogger` for logging
- [x] Handlers use `IEmailService` for notifications (interface only, impl in Phase 3)
- [x] Handlers are async and use CancellationToken
- [x] Handlers handle errors gracefully (don't break main flow)
- [x] Unit tests with mocked dependencies
- [x] Events logged with structured logging
- [x] XML documentation

**Example Code:**

```csharp
// TicketCreatedEventHandler.cs
public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public TicketCreatedEventHandler(
        ILogger<TicketCreatedEventHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(
        TicketCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket created: {TicketId} by {UserId}",
            notification.TicketId,
            notification.CreatedByUserId);

        // Future: Send notification email to organization admin
        // await _emailService.SendTicketCreatedNotificationAsync(...);

        await Task.CompletedTask;
    }
}

// TicketAssignedEventHandler.cs
public class TicketAssignedEventHandler : INotificationHandler<TicketAssignedEvent>
{
    private readonly ILogger<TicketAssignedEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IServiceUserRepository _userRepository;

    public TicketAssignedEventHandler(
        ILogger<TicketAssignedEventHandler> logger,
        IEmailService emailService,
        IServiceUserRepository userRepository)
    {
        _logger = logger;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task Handle(
        TicketAssignedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} assigned to {UserId}",
            notification.TicketId,
            notification.AssignedToUserId);

        if (notification.AssignedToUserId.HasValue)
        {
            var user = await _userRepository.GetByUserIdAsync(
                notification.AssignedToUserId.Value,
                cancellationToken);

            if (user is not null)
            {
                // Send assignment notification
                // await _emailService.SendTicketAssignedNotificationAsync(...);
            }
        }

        await Task.CompletedTask;
    }
}
```

**Dependencies:**

- Issue #11 (Application Infrastructure)
- Issue #13 (Ticket Commands)

**Testing:**

Unit tests in `Flowertrack.Application.Tests/Features/Tickets/EventHandlers/`:
- Test each handler independently
- Mock dependencies (IEmailService, ILogger, repositories)
- Verify correct methods called with correct parameters

---

### Issue #15: Implement Organizations Feature - Queries & Commands

**Title:** Implement Organization Queries and Commands

**Labels:** `phase-2`, `application`, `feature:organizations`, `priority:high`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 3 days

**Description:**

Implement CQRS commands and queries for the Organizations feature including listing, viewing details, onboarding, and managing organization data.

**User Stories:**

- US-024: As a Service Admin, I want to view all organizations
- US-025: As a Service Admin, I want to onboard new organizations
- US-026: As a Service Technician, I want to view organization details

**Technical Requirements:**

**Queries:**
1. `GetOrganizationsQuery` - List all organizations with filtering
2. `GetOrganizationByIdQuery` - Get organization with machines and contacts
3. `GetOrganizationMachinesQuery` - List machines for organization

**Commands:**
1. `CreateOrganizationCommand` - Create new organization
2. `UpdateOrganizationCommand` - Update organization details
3. `OnboardOrganizationCommand` - Create organization + admin user + send invitation
4. `SuspendOrganizationCommand` - Suspend organization service
5. `RenewOrganizationContractCommand` - Renew contract

**Files to Create:**

```
Flowertrack.Application/Features/Organizations/
├── Queries/
│   ├── GetOrganizations/
│   │   ├── GetOrganizationsQuery.cs
│   │   ├── GetOrganizationsQueryHandler.cs
│   │   ├── GetOrganizationsQueryValidator.cs
│   │   └── OrganizationDto.cs
│   ├── GetOrganizationById/
│   │   ├── GetOrganizationByIdQuery.cs
│   │   ├── GetOrganizationByIdQueryHandler.cs
│   │   ├── GetOrganizationByIdQueryValidator.cs
│   │   └── OrganizationDetailDto.cs
│   └── GetOrganizationMachines/
│       └── ...
├── Commands/
│   ├── CreateOrganization/
│   │   └── ...
│   ├── UpdateOrganization/
│   │   └── ...
│   ├── OnboardOrganization/
│   │   ├── OnboardOrganizationCommand.cs
│   │   ├── OnboardOrganizationCommandHandler.cs
│   │   ├── OnboardOrganizationCommandValidator.cs
│   │   └── OrganizationOnboardedDto.cs
│   ├── SuspendOrganization/
│   │   └── ...
│   └── RenewContract/
│       └── ...
└── EventHandlers/
    ├── OrganizationCreatedEventHandler.cs
    ├── OrganizationServiceSuspendedEventHandler.cs
    └── OrganizationContractRenewedEventHandler.cs
```

**Acceptance Criteria:**

- [x] All queries support filtering and pagination
- [x] GetOrganizations restricted to service team
- [x] GetOrganizationById includes nested data (machines, contacts)
- [x] OnboardOrganization creates organization AND admin user
- [x] OnboardOrganization sends invitation email
- [x] OnboardOrganization uses transaction (rollback if email fails)
- [x] SuspendOrganization validates business rules
- [x] All commands have validators
- [x] DTOs use AutoMapper
- [x] Event handlers log actions
- [x] Unit tests with 85%+ coverage
- [x] XML documentation

**Example Code:**

```csharp
// OnboardOrganizationCommand.cs
public record OnboardOrganizationCommand(
    string OrganizationName,
    string AdminEmail,
    string AdminFirstName,
    string AdminLastName,
    string Street,
    string City,
    string PostalCode,
    string Country
) : IRequest<Result<Guid>>;

// OnboardOrganizationCommandHandler.cs
public class OnboardOrganizationCommandHandler 
    : IRequestHandler<OnboardOrganizationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _orgRepository;
    private readonly IOrganizationUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OnboardOrganizationCommandHandler> _logger;

    public async Task<Result<Guid>> Handle(
        OnboardOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // Check if organization name already exists
        var existingOrg = await _orgRepository.GetByNameAsync(
            request.OrganizationName,
            cancellationToken);

        if (existingOrg is not null)
            return Result.Conflict("Organization with this name already exists.");

        // Check if email already exists
        var email = Email.Create(request.AdminEmail);
        var existingUser = await _userRepository.ExistsByEmailAsync(
            email,
            cancellationToken);

        if (existingUser)
            return Result.Conflict("User with this email already exists.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Create Organization
            var organization = Organization.Create(
                request.OrganizationName,
                request.Street,
                request.City,
                request.PostalCode,
                request.Country
            );

            await _orgRepository.AddAsync(organization, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create Admin User (via IIdentityService - implementation in Infrastructure)
            var userId = await _identityService.CreateUserAsync(
                email,
                request.AdminFirstName,
                request.AdminLastName,
                "OrganizationAdmin",
                cancellationToken);

            var organizationUser = OrganizationUser.Create(
                userId,
                request.AdminFirstName,
                request.AdminLastName,
                organization.Id
            );

            await _userRepository.AddAsync(organizationUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate activation token
            var activationToken = await _identityService.GenerateActivationTokenAsync(
                userId,
                cancellationToken);

            // Send invitation email
            await _emailService.SendOrganizationInvitationAsync(
                email.Value,
                request.AdminFirstName,
                request.OrganizationName,
                activationToken,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Organization {OrganizationName} onboarded with admin {Email}",
                request.OrganizationName,
                email.Value);

            return Result.Success(organization.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to onboard organization {Name}", request.OrganizationName);
            return Result.Error("Failed to onboard organization. Please try again.");
        }
    }
}
```

**Dependencies:**

- Issue #10, #11

**Testing:**

- Unit tests for all handlers
- Integration test for OnboardOrganization (complex transaction)

---

### Issue #16: Implement Machines Feature - Queries & Commands

**Title:** Implement Machine Queries and Commands

**Labels:** `phase-2`, `application`, `feature:machines`, `priority:high`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 3 days

**Description:**

Implement CQRS commands and queries for the Machines feature including registration, status management, maintenance scheduling, and API token management.

**User Stories:**

- US-027: As a Service Admin, I want to register new machines
- US-028: As a Service Admin, I want to generate API tokens for machines
- US-029: As a Technician, I want to view machine status and history

**Technical Requirements:**

**Queries:**
1. `GetMachinesQuery` - List all machines with filtering
2. `GetMachineByIdQuery` - Get machine with organization and tickets
3. `GetMachinesByOrganizationQuery` - List machines for organization
4. `GetMachinesDueForMaintenanceQuery` - Get machines needing maintenance

**Commands:**
1. `RegisterMachineCommand` - Register new machine
2. `UpdateMachineCommand` - Update machine details
3. `UpdateMachineStatusCommand` - Change machine status
4. `ScheduleMaintenanceCommand` - Schedule maintenance
5. `CompleteMaintenanceCommand` - Mark maintenance as completed
6. `RegenerateApiTokenCommand` - Regenerate machine API token
7. `ActivateAlarmCommand` - Activate machine alarm
8. `ClearAlarmCommand` - Clear machine alarm

**Files to Create:**

```
Flowertrack.Application/Features/Machines/
├── Queries/
│   ├── GetMachines/
│   ├── GetMachineById/
│   ├── GetMachinesByOrganization/
│   └── GetMachinesDueForMaintenance/
├── Commands/
│   ├── RegisterMachine/
│   ├── UpdateMachine/
│   ├── UpdateMachineStatus/
│   ├── ScheduleMaintenance/
│   ├── CompleteMaintenance/
│   ├── RegenerateApiToken/
│   ├── ActivateAlarm/
│   └── ClearAlarm/
└── EventHandlers/
    ├── MachineRegisteredEventHandler.cs
    ├── MachineAlarmActivatedEventHandler.cs
    ├── MachineAlarmClearedEventHandler.cs
    └── MaintenanceScheduledEventHandler.cs
```

**Acceptance Criteria:**

- [x] RegisterMachine generates secure API token
- [x] RegisterMachine validates serial number uniqueness
- [x] UpdateMachineStatus validates state transitions
- [x] ScheduleMaintenance calculates next maintenance date
- [x] RegenerateApiToken invalidates old token
- [x] ActivateAlarm auto-creates critical ticket (via event handler)
- [x] All queries support filtering by organization
- [x] Service team can see all machines
- [x] Organization users only see their machines
- [x] DTOs include nested organization data
- [x] Unit tests with 85%+ coverage
- [x] Integration tests for RegisterMachine

**Example Code:**

```csharp
// RegisterMachineCommand.cs
public record RegisterMachineCommand(
    Guid OrganizationId,
    string SerialNumber,
    string Brand,
    string Model,
    string? Location = null
) : IRequest<Result<MachineRegisteredDto>>;

// RegisterMachineCommandHandler.cs
public class RegisterMachineCommandHandler 
    : IRequestHandler<RegisterMachineCommand, Result<MachineRegisteredDto>>
{
    private readonly IMachineRepository _repository;
    private readonly IOrganizationRepository _orgRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<MachineRegisteredDto>> Handle(
        RegisterMachineCommand request,
        CancellationToken cancellationToken)
    {
        // Validate organization exists
        var organization = await _orgRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken);

        if (organization is null)
            return Result.NotFound("Organization not found.");

        // Check serial number uniqueness
        var existingMachine = await _repository.GetBySerialNumberAsync(
            request.SerialNumber,
            cancellationToken);

        if (existingMachine is not null)
            return Result.Conflict("Machine with this serial number already exists.");

        // Create machine (generates API token automatically)
        var machine = Machine.Register(
            request.OrganizationId,
            request.SerialNumber,
            request.Brand,
            request.Model,
            request.Location
        );

        await _repository.AddAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new MachineRegisteredDto
        {
            Id = machine.Id,
            SerialNumber = machine.SerialNumber,
            ApiToken = machine.ApiToken.ToString(),
            Status = machine.Status.ToString()
        };

        return Result.Success(dto);
    }
}

// MachineAlarmActivatedEventHandler.cs
public class MachineAlarmActivatedEventHandler 
    : INotificationHandler<MachineAlarmActivatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<MachineAlarmActivatedEventHandler> _logger;

    public async Task Handle(
        MachineAlarmActivatedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Machine alarm activated: {MachineId} - {AlarmCode}",
            notification.MachineId,
            notification.AlarmCode);

        // Auto-create critical ticket
        var createTicketCommand = new CreateTicketCommand(
            MachineId: notification.MachineId,
            Title: $"ALARM: {notification.AlarmCode} - {notification.Message}",
            Description: $"Automatic ticket created from machine alarm.\n\nAlarm Code: {notification.AlarmCode}\nMessage: {notification.Message}\nActivated: {notification.ActivatedAt:yyyy-MM-dd HH:mm:ss}",
            Priority: Priority.Critical
        );

        var result = await _mediator.Send(createTicketCommand, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Auto-created ticket {TicketId} for machine alarm {MachineId}",
                result.Value,
                notification.MachineId);
        }
        else
        {
            _logger.LogError(
                "Failed to auto-create ticket for machine alarm {MachineId}: {Error}",
                notification.MachineId,
                result.Errors);
        }
    }
}
```

**Dependencies:**

- Issue #10, #11

**Testing:**

- Unit tests for all commands/queries
- Test MachineAlarmActivatedEventHandler creates ticket
- Test RegenerateApiToken generates unique tokens

---

## 🟢 Medium Priority Issues

### Issue #17: Implement Users Feature - Queries & Commands

**Title:** Implement User Management Queries and Commands

**Labels:** `phase-2`, `application`, `feature:users`, `priority:medium`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 2-3 days

**Description:**

Implement CQRS commands and queries for user management including listing, inviting, activating, and deactivating users.

**User Stories:**

- US-031: As a Service Admin, I want to view all service users
- US-032: As a Service Admin, I want to invite new service users
- US-049: As an Organization Admin, I want to view my team
- US-050: As an Organization Admin, I want to invite new operators

**Technical Requirements:**

**Queries:**
1. `GetServiceUsersQuery` - List all service users (admin only)
2. `GetOrganizationUsersQuery` - List users for organization
3. `GetCurrentUserQuery` - Get current user profile

**Commands:**
1. `InviteServiceUserCommand` - Invite new service user
2. `InviteOrganizationUserCommand` - Invite new organization user
3. `ActivateUserCommand` - Activate user account with password
4. `DeactivateUserCommand` - Deactivate user account
5. `UpdateUserProfileCommand` - Update user profile

(See PHASE-2-APPLICATION.md section 2.5 for full details)

**Acceptance Criteria:**

- [x] GetServiceUsers restricted to service admins
- [x] GetOrganizationUsers shows only users from same organization
- [x] Invite commands create user AND send email
- [x] ActivateUser validates activation token
- [x] ActivateUser sets password via IIdentityService
- [x] DeactivateUser soft-deletes (status = Inactive)
- [x] Unit tests with 80%+ coverage

---

### Issue #18: Implement Application Services

**Title:** Implement Application Layer Services

**Labels:** `phase-2`, `application`, `priority:medium`, `type:feature`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 2 days

**Description:**

Implement application services including TicketNumberGenerator and define interfaces for external services (Email, File Storage).

**Technical Requirements:**

1. **TicketNumberGeneratorService** - Implements `ITicketNumberGenerator`
   - Generate sequential ticket numbers
   - Format: `TICK-{year}-{sequential:D5}`
   - Handle concurrency with database locking

2. **IEmailService** - Interface for email notifications
   - SendInvitationEmail
   - SendTicketNotification
   - SendPasswordResetEmail
   - Implementation in Infrastructure layer (Phase 3)

3. **IFileStorageService** - Interface for file uploads
   - UploadAsync
   - DeleteAsync
   - GetDownloadUrlAsync
   - Implementation in Infrastructure layer (Phase 3)

**Files to Create:**

```
Flowertrack.Application/
├── Services/
│   └── TicketNumberGeneratorService.cs
└── Common/Interfaces/
    ├── IEmailService.cs
    └── IFileStorageService.cs
```

**Acceptance Criteria:**

- [x] TicketNumberGenerator handles concurrent requests
- [x] TicketNumberGenerator uses database for state
- [x] Generated numbers are sequential within year
- [x] Year resets sequential counter
- [x] All interface methods have XML documentation
- [x] Unit tests for TicketNumberGenerator
- [x] Mock implementations for testing

**Example Code:**

```csharp
// TicketNumberGeneratorService.cs
public class TicketNumberGeneratorService : ITicketNumberGenerator
{
    private readonly IApplicationDbContext _context;

    public TicketNumberGeneratorService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketNumber> GenerateNextNumberAsync(
        CancellationToken cancellationToken = default)
    {
        var currentYear = DateTime.UtcNow.Year;

        // Get last ticket number for current year
        // This query should use database locking to prevent race conditions
        var lastTicket = await _context.Tickets
            .Where(t => t.CreatedAt.Year == currentYear)
            .OrderByDescending(t => t.TicketNumber)
            .Select(t => t.TicketNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextSequential = 1;
        
        if (lastTicket is not null && lastTicket.Year == currentYear)
        {
            nextSequential = lastTicket.Sequential + 1;
        }

        return TicketNumber.Create(currentYear, nextSequential);
    }
}
```

---

### Issue #19: Complete Application Layer Unit Tests

**Title:** Write Comprehensive Unit Tests for Application Layer

**Labels:** `phase-2`, `application`, `priority:medium`, `type:testing`  
**Milestone:** Phase 2 - Application Layer  
**Estimate:** 4-5 days (parallel with development)

**Description:**

Create comprehensive unit tests for all commands, queries, validators, and behaviors in the Application layer.

**Test Coverage Goals:**

- Commands: 90%+
- Queries: 85%+
- Validators: 100%
- Event Handlers: 80%+
- Pipeline Behaviors: 95%+
- Overall Application Layer: 85%+

**Test Structure:**

```
Flowertrack.Application.Tests/
├── Features/
│   ├── Tickets/
│   │   ├── Commands/
│   │   │   ├── CreateTicketCommandHandlerTests.cs
│   │   │   ├── CreateTicketCommandValidatorTests.cs
│   │   │   └── ...
│   │   ├── Queries/
│   │   │   ├── GetTicketsQueryHandlerTests.cs
│   │   │   └── ...
│   │   └── EventHandlers/
│   │       └── ...
│   ├── Organizations/...
│   ├── Machines/...
│   └── Users/...
├── Common/
│   ├── Behaviours/
│   │   ├── ValidationBehaviourTests.cs
│   │   ├── PerformanceBehaviourTests.cs
│   │   └── LoggingBehaviourTests.cs
│   └── Mappings/
│       └── MappingTests.cs
└── TestHelpers/
    ├── MockRepositories/
    │   ├── MockTicketRepository.cs
    │   └── ...
    ├── FakeData/
    │   ├── TicketFaker.cs
    │   └── ...
    └── TestBase.cs
```

**Testing Requirements:**

1. **Command Tests:**
   - Happy path scenarios
   - Validation failures
   - Business rule violations
   - Permission checks
   - Concurrent operations

2. **Query Tests:**
   - Filtering logic
   - Pagination
   - Sorting
   - Authorization
   - Data mapping

3. **Validator Tests:**
   - All validation rules
   - Edge cases
   - Boundary conditions

4. **Event Handler Tests:**
   - Event processing
   - Side effects
   - Error handling

5. **Behavior Tests:**
   - Validation pipeline
   - Exception handling
   - Performance logging

**Acceptance Criteria:**

- [x] All commands have unit tests
- [x] All queries have unit tests
- [x] All validators have 100% coverage
- [x] Mock repositories for testing
- [x] Fake data generators using Bogus
- [x] TestBase class for common setup
- [x] Clear test naming (Method_Scenario_ExpectedBehavior)
- [x] Use FluentAssertions for assertions
- [x] Coverage reports generated
- [x] All tests pass in CI/CD

**Example Test:**

```csharp
public class CreateTicketCommandHandlerTests
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IMachineRepository> _machineRepositoryMock;
    private readonly Mock<ITicketNumberGenerator> _numberGeneratorMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _machineRepositoryMock = new Mock<IMachineRepository>();
        _numberGeneratorMock = new Mock<ITicketNumberGenerator>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateTicketCommandHandler(
            _ticketRepositoryMock.Object,
            _machineRepositoryMock.Object,
            _numberGeneratorMock.Object,
            _currentUserMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTicketSuccessfully()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        
        var machine = Machine.Register(
            organizationId,
            "SN-123",
            "Baumalog",
            "Model-X",
            "Location-A");

        var ticketNumber = TicketNumber.Create(2025, 1);

        _machineRepositoryMock
            .Setup(x => x.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        _numberGeneratorMock
            .Setup(x => x.GenerateNextNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketNumber);

        _currentUserMock.Setup(x => x.UserId).Returns(userId);
        _currentUserMock.Setup(x => x.OrganizationId).Returns(organizationId);

        var command = new CreateTicketCommand(
            MachineId: machineId,
            Title: "Test Ticket Title",
            Description: "Test Description",
            Priority: Priority.High
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _ticketRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MachineNotFound_ReturnsNotFound()
    {
        // Arrange
        var machineId = Guid.NewGuid();

        _machineRepositoryMock
            .Setup(x => x.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Machine?)null);

        var command = new CreateTicketCommand(
            MachineId: machineId,
            Title: "Test Ticket",
            Description: "Test",
            Priority: Priority.High
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain("Machine not found.");
    }

    // ... more test cases
}
```

---

## Summary

**Total Issues for Phase 2:** 10

**Breakdown by Priority:**
- 🔥 Critical: 4 issues (Repository Interfaces, Infrastructure, Ticket Queries, Ticket Commands)
- 🟡 High: 4 issues (Event Handlers, Organizations, Machines, Users)
- 🟢 Medium: 2 issues (Application Services, Unit Tests)

**Total Estimated Time:** 25-30 days (with parallel development)

**Critical Path:**
1. Issue #10 (Repository Interfaces) - 2 days
2. Issue #11 (Application Infrastructure) - 3 days
3. Issue #12 (Ticket Queries) - 3 days
4. Issue #13 (Ticket Commands) - 4 days
5. Issue #14 (Event Handlers) - 2 days

**Parallel Work:**
- Issue #15, #16, #17 can be developed in parallel after #11
- Issue #19 (Unit Tests) developed alongside all features

---

**Last Updated:** 2025-10-26  
**Version:** 1.0
