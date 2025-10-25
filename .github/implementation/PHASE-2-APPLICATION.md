# FAZA 2: Application Layer 🎯

**Status:** ⚪ Not Started  
**Started:** TBD  
**Target Completion:** Sprint 2-3 (Week 3-5)  
**Dependencies:** Phase 1 (Domain Layer) must be complete

---

## Overview

Phase 2 focuses on implementing the Application Layer using **CQRS (Command Query Responsibility Segregation)** pattern with MediatR. This layer orchestrates domain logic, handles use cases, and provides a clean API for the Presentation layer.

### Architecture Approach

We're adopting **Vertical Slice Architecture** principles within Clean Architecture:
- Feature-based organization (not technical layers)
- Each use case is self-contained
- Commands and Queries per feature
- Minimal cross-feature dependencies

### Key Technologies
- **MediatR** - CQRS implementation
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **Ardalis.Result** - Result pattern (no exceptions for business logic failures)

---

## 2.0 Repository Interfaces (Complete Phase 1) 📚

**Priority:** 🔥 CRITICAL - Must complete before Application Layer  
**Estimated Time:** 1-2 days

### 2.0.1 Base Repository Interface

- [ ] **IRepository<T>** in `Domain/Repositories/`
  - [ ] `GetByIdAsync(TId id, CancellationToken ct = default)`
  - [ ] `ListAsync(CancellationToken ct = default)`
  - [ ] `ListAsync(ISpecification<T> spec, CancellationToken ct = default)`
  - [ ] `AddAsync(T entity, CancellationToken ct = default)`
  - [ ] `UpdateAsync(T entity, CancellationToken ct = default)`
  - [ ] `DeleteAsync(T entity, CancellationToken ct = default)`
  - [ ] `CountAsync(CancellationToken ct = default)`
  - [ ] `CountAsync(ISpecification<T> spec, CancellationToken ct = default)`
  - [ ] `AnyAsync(ISpecification<T> spec, CancellationToken ct = default)`
  - [ ] `FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default)`
  - [ ] XML documentation for all methods

### 2.0.2 Specialized Repository Interfaces

- [ ] **ITicketRepository** (extends IRepository<Ticket>)
  - [ ] `GetByNumberAsync(TicketNumber number, CancellationToken ct)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `GetAssignedToUserAsync(Guid userId, CancellationToken ct)`
  - [ ] `CountByStatusAsync(TicketStatus status, CancellationToken ct)`
  - [ ] `GetWithFullDetailsAsync(Guid id, CancellationToken ct)` - includes all navigation props

- [ ] **IOrganizationRepository** (extends IRepository<Organization>)
  - [ ] `GetByNameAsync(string name, CancellationToken ct)`
  - [ ] `GetWithMachinesAsync(Guid id, CancellationToken ct)`
  - [ ] `GetWithContactsAsync(Guid id, CancellationToken ct)`
  - [ ] `GetActiveOrganizationsAsync(CancellationToken ct)`

- [ ] **IMachineRepository** (extends IRepository<Machine>)
  - [ ] `GetBySerialNumberAsync(string serialNumber, CancellationToken ct)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `GetByApiTokenAsync(MachineApiKey token, CancellationToken ct)`
  - [ ] `GetActiveMachinesAsync(CancellationToken ct)`
  - [ ] `GetMachinesDueForMaintenanceAsync(int daysThreshold, CancellationToken ct)`

- [ ] **IServiceUserRepository**
  - [ ] `GetByUserIdAsync(Guid userId, CancellationToken ct)`
  - [ ] `GetActiveServiceUsersAsync(CancellationToken ct)`
  - [ ] `ExistsByEmailAsync(Email email, CancellationToken ct)`

- [ ] **IOrganizationUserRepository**
  - [ ] `GetByUserIdAsync(Guid userId, CancellationToken ct)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `GetActiveByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `ExistsByEmailAsync(Email email, CancellationToken ct)`

- [ ] **IUnitOfWork** in `Application/Common/Interfaces/`
  - [ ] `Task<int> SaveChangesAsync(CancellationToken ct = default)`
  - [ ] `Task BeginTransactionAsync(CancellationToken ct = default)`
  - [ ] `Task CommitTransactionAsync(CancellationToken ct = default)`
  - [ ] `Task RollbackTransactionAsync(CancellationToken ct = default)`
  - [ ] Properties for accessing all repositories

---

## 2.1 Application Infrastructure 🏗️

**Priority:** 🔥 HIGH  
**Estimated Time:** 2-3 days

### 2.1.1 Install Required Packages

- [ ] Add NuGet packages to `Flowertrack.Application.csproj`:
  ```xml
  <PackageReference Include="MediatR" Version="12.4.0" />
  <PackageReference Include="FluentValidation" Version="11.9.2" />
  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.2" />
  <PackageReference Include="AutoMapper" Version="13.0.1" />
  <PackageReference Include="Ardalis.Result" Version="9.0.0" />
  <PackageReference Include="Ardalis.Specification" Version="8.0.0" />
  ```

### 2.1.2 Common Interfaces

- [ ] **`Application/Common/Interfaces/`**
  - [ ] `IApplicationDbContext.cs` - DbContext abstraction
    ```csharp
    DbSet<Ticket> Tickets { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Machine> Machines { get; }
    DbSet<ServiceUser> ServiceUsers { get; }
    DbSet<OrganizationUser> OrganizationUsers { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
    ```
  - [ ] `IDateTime.cs` - Testable date/time
  - [ ] `ICurrentUserService.cs` - Current user context
    ```csharp
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    IEnumerable<string> Roles { get; }
    ```
  - [ ] `IIdentityService.cs` - User management
  - [ ] `IEmailService.cs` - Email notifications

### 2.1.3 Common Base Classes

- [ ] **`Application/Common/Models/`**
  - [ ] `PaginatedList<T>.cs` - Generic pagination
  - [ ] `Result.cs` - Custom result wrapper (if not using Ardalis.Result)

- [ ] **`Application/Common/Behaviours/`** (MediatR Pipeline Behaviors)
  - [ ] `LoggingBehaviour.cs` - Request/response logging
  - [ ] `ValidationBehaviour.cs` - FluentValidation pipeline
  - [ ] `PerformanceBehaviour.cs` - Performance monitoring
  - [ ] `UnhandledExceptionBehaviour.cs` - Exception handling
  - [ ] `AuthorizationBehaviour.cs` - Permission checking

### 2.1.4 Common Mappings

- [ ] **`Application/Common/Mappings/`**
  - [ ] `MappingProfile.cs` - AutoMapper profile base
  - [ ] `IMapFrom<T>.cs` - Mapping interface
    ```csharp
    void Mapping(Profile profile) 
        => profile.CreateMap(typeof(T), GetType());
    ```

### 2.1.5 Common Exceptions

- [ ] **`Application/Common/Exceptions/`**
  - [ ] `ValidationException.cs` - Validation failures
  - [ ] `NotFoundException.cs` - Entity not found
  - [ ] `ForbiddenAccessException.cs` - Authorization failures
  - [ ] `ConflictException.cs` - Business rule violations

---

## 2.2 Tickets Feature 🎫

**Priority:** 🔥 CRITICAL - Core MVP feature  
**Estimated Time:** 5-7 days

### 2.2.1 Queries

#### GetTicketsQuery
- [ ] **File:** `Application/Features/Tickets/Queries/GetTickets/GetTicketsQuery.cs`
  - [ ] Query record with filters:
    ```csharp
    public record GetTicketsQuery(
        int Page = 1,
        int PageSize = 20,
        string? SortBy = "CreatedAt",
        string SortOrder = "desc",
        int? StatusId = null,
        int? PriorityId = null,
        Guid? OrganizationId = null,
        Guid? MachineId = null,
        Guid? AssignedToUserId = null,
        string? Search = null
    ) : IRequest<Result<PaginatedList<TicketDto>>>;
    ```
- [ ] **Handler:** `GetTicketsQueryHandler.cs`
  - [ ] Apply filters using Specifications
  - [ ] Apply sorting
  - [ ] Return paginated results
  - [ ] Include: Organization.Name, Machine.SerialNumber, AssignedTo.FullName
- [ ] **Validator:** `GetTicketsQueryValidator.cs`
  - [ ] Page >= 1
  - [ ] PageSize between 1-100
  - [ ] Valid SortBy fields
- [ ] **DTO:** `TicketDto.cs`
  - [ ] Maps from Ticket entity
  - [ ] Flattened properties for API response

#### GetTicketByIdQuery
- [ ] **File:** `Application/Features/Tickets/Queries/GetTicketById/GetTicketByIdQuery.cs`
  - [ ] `record GetTicketByIdQuery(Guid Id) : IRequest<Result<TicketDetailDto>>`
- [ ] **Handler:** `GetTicketByIdQueryHandler.cs`
  - [ ] Load ticket with all navigation properties
  - [ ] Return NotFound if ticket doesn't exist
  - [ ] Check permissions (user can only see own org tickets)
- [ ] **DTO:** `TicketDetailDto.cs`
  - [ ] Full ticket details
  - [ ] Nested objects: Organization, Machine, CreatedBy, AssignedTo
- [ ] **Validator:** `GetTicketByIdQueryValidator.cs`
  - [ ] Id not empty

#### GetTicketHistoryQuery
- [ ] **File:** `Application/Features/Tickets/Queries/GetTicketHistory/GetTicketHistoryQuery.cs`
- [ ] **Handler:** Returns timeline of all ticket events
- [ ] **DTO:** `TicketHistoryEventDto.cs`

### 2.2.2 Commands

#### CreateTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/CreateTicket/CreateTicketCommand.cs`
  - [ ] Properties: MachineId, Title, Description, PriorityId
  - [ ] Returns: `Result<Guid>` (new ticket ID)
- [ ] **Handler:** `CreateTicketCommandHandler.cs`
  - [ ] Validate machine exists and belongs to user's organization
  - [ ] Generate TicketNumber using domain service
  - [ ] Create Ticket entity
  - [ ] Set CreatedByUserId from ICurrentUserService
  - [ ] Status = Draft
  - [ ] Save to repository
  - [ ] Publish TicketCreatedEvent
- [ ] **Validator:** `CreateTicketCommandValidator.cs`
  - [ ] MachineId not empty
  - [ ] Title: required, length 10-255
  - [ ] Description: max 5000 chars
  - [ ] PriorityId: valid enum value
- [ ] **Response DTO:** `TicketCreatedDto.cs`

#### UpdateTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/UpdateTicket/UpdateTicketCommand.cs`
  - [ ] Properties: Id, Title, Description
- [ ] **Handler:** `UpdateTicketCommandHandler.cs`
  - [ ] Load ticket
  - [ ] Check permissions (creator or admin)
  - [ ] Update allowed fields
  - [ ] Save changes
- [ ] **Validator:** `UpdateTicketCommandValidator.cs`

#### AssignTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/AssignTicket/AssignTicketCommand.cs`
  - [ ] Properties: TicketId, AssignedToUserId (nullable)
- [ ] **Handler:** `AssignTicketCommandHandler.cs`
  - [ ] Load ticket
  - [ ] Call `ticket.AssignTo()` domain method
  - [ ] Save changes
  - [ ] Publish TicketAssignedEvent
- [ ] **Validator:** `AssignTicketCommandValidator.cs`
  - [ ] Verify assignee is active service user
  - [ ] Verify ticket exists

#### ChangeTicketStatusCommand
- [ ] **File:** `Application/Features/Tickets/Commands/ChangeTicketStatus/ChangeTicketStatusCommand.cs`
  - [ ] Properties: TicketId, NewStatus, Justification (optional)
- [ ] **Handler:** `ChangeTicketStatusCommandHandler.cs`
  - [ ] Load ticket
  - [ ] Call `ticket.UpdateStatus()` domain method
  - [ ] Handle domain exceptions for invalid transitions
  - [ ] Save changes
- [ ] **Validator:** `ChangeTicketStatusCommandValidator.cs`

#### ResolveTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/ResolveTicket/ResolveTicketCommand.cs`
- [ ] **Handler:** Calls `ticket.Resolve()`
- [ ] **Validator:**

#### ReopenTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/ReopenTicket/ReopenTicketCommand.cs`
- [ ] **Handler:** Calls `ticket.Reopen()`
- [ ] **Validator:** Check 14-day window

#### CloseTicketCommand
- [ ] **File:** `Application/Features/Tickets/Commands/CloseTicket/CloseTicketCommand.cs`
- [ ] **Handler:** Calls `ticket.Close()`

#### AddCommentCommand
- [ ] **File:** `Application/Features/Tickets/Commands/AddComment/AddCommentCommand.cs`
  - [ ] Properties: TicketId, Text, IsInternal
- [ ] **Handler:** `AddCommentCommandHandler.cs`
  - [ ] Call `ticket.AddComment()`
  - [ ] Save to TicketHistory table
- [ ] **Validator:**

#### AddAttachmentCommand
- [ ] **File:** `Application/Features/Tickets/Commands/AddAttachment/AddAttachmentCommand.cs`
  - [ ] Properties: TicketId, File (IFormFile), FileName
- [ ] **Handler:** Store file, create Attachment entity
- [ ] **Validator:** File size, mime type validation

---

## 2.3 Organizations Feature 🏢

**Priority:** 🔥 HIGH  
**Estimated Time:** 3-4 days

### 2.3.1 Queries

#### GetOrganizationsQuery
- [ ] **File:** `Application/Features/Organizations/Queries/GetOrganizations/GetOrganizationsQuery.cs`
- [ ] **Handler:** List all organizations (service team only)
- [ ] **DTO:** `OrganizationDto.cs`
- [ ] **Validator:**

#### GetOrganizationByIdQuery
- [ ] **File:** `Application/Features/Organizations/Queries/GetOrganizationById/GetOrganizationByIdQuery.cs`
- [ ] **Handler:** Get organization with machines, contacts
- [ ] **DTO:** `OrganizationDetailDto.cs`

#### GetOrganizationMachinesQuery
- [ ] **File:** `Application/Features/Organizations/Queries/GetOrganizationMachines/`
- [ ] **Handler:** List machines for organization
- [ ] **DTO:** `MachineDto.cs`

### 2.3.2 Commands

#### CreateOrganizationCommand
- [ ] **File:** `Application/Features/Organizations/Commands/CreateOrganization/CreateOrganizationCommand.cs`
  - [ ] Properties: Name, Address fields, ServiceStatus
- [ ] **Handler:** Create Organization entity
- [ ] **Validator:** Name required, unique

#### UpdateOrganizationCommand
- [ ] **File:** `Application/Features/Organizations/Commands/UpdateOrganization/`
- [ ] **Handler:** Update organization details
- [ ] **Validator:**

#### OnboardOrganizationCommand
- [ ] **File:** `Application/Features/Organizations/Commands/OnboardOrganization/OnboardOrganizationCommand.cs`
  - [ ] Properties: Name, AdminEmail, AdminFirstName, AdminLastName
- [ ] **Handler:**
  - [ ] Create Organization
  - [ ] Create OrganizationUser (admin)
  - [ ] Generate activation token
  - [ ] Send invitation email via IEmailService
- [ ] **Validator:** Email format, unique

#### SuspendOrganizationCommand
- [ ] **File:** `Application/Features/Organizations/Commands/SuspendOrganization/`
- [ ] **Handler:** Change ServiceStatus to Suspended
- [ ] **Validator:**

---

## 2.4 Machines Feature ⚙️

**Priority:** 🔥 HIGH  
**Estimated Time:** 3-4 days

### 2.4.1 Queries

#### GetMachinesQuery
- [ ] **File:** `Application/Features/Machines/Queries/GetMachines/GetMachinesQuery.cs`
- [ ] **Handler:** List all machines with filters
- [ ] **DTO:** `MachineDto.cs`

#### GetMachineByIdQuery
- [ ] **File:** `Application/Features/Machines/Queries/GetMachineById/GetMachineByIdQuery.cs`
- [ ] **Handler:** Get machine with organization, tickets
- [ ] **DTO:** `MachineDetailDto.cs`

#### GetMachinesByOrganizationQuery
- [ ] **File:** `Application/Features/Machines/Queries/GetMachinesByOrganization/`
- [ ] **Handler:** List machines for specific organization

### 2.4.2 Commands

#### RegisterMachineCommand
- [ ] **File:** `Application/Features/Machines/Commands/RegisterMachine/RegisterMachineCommand.cs`
  - [ ] Properties: OrganizationId, SerialNumber, Brand, Model, Location
- [ ] **Handler:**
  - [ ] Create Machine entity
  - [ ] Generate API token via `machine.GenerateApiToken()`
  - [ ] Save to repository
- [ ] **Validator:** SerialNumber unique

#### UpdateMachineStatusCommand
- [ ] **File:** `Application/Features/Machines/Commands/UpdateMachineStatus/`
- [ ] **Handler:** Call `machine.UpdateStatus()`

#### ScheduleMaintenanceCommand
- [ ] **File:** `Application/Features/Machines/Commands/ScheduleMaintenance/`
- [ ] **Handler:** Call `machine.ScheduleMaintenance()`

#### RegenerateApiTokenCommand
- [ ] **File:** `Application/Features/Machines/Commands/RegenerateApiToken/`
- [ ] **Handler:** Call `machine.GenerateApiToken()`

---

## 2.5 Users Feature 👥

**Priority:** 🟡 MEDIUM  
**Estimated Time:** 3-4 days

### 2.5.1 Queries

#### GetServiceUsersQuery
- [ ] **File:** `Application/Features/Users/Queries/GetServiceUsers/`
- [ ] **Handler:** List all service users (admin only)
- [ ] **DTO:** `ServiceUserDto.cs`

#### GetOrganizationUsersQuery
- [ ] **File:** `Application/Features/Users/Queries/GetOrganizationUsers/`
- [ ] **Handler:** List users for organization

#### GetCurrentUserQuery
- [ ] **File:** `Application/Features/Users/Queries/GetCurrentUser/`
- [ ] **Handler:** Get current user profile

### 2.5.2 Commands

#### InviteServiceUserCommand
- [ ] **File:** `Application/Features/Users/Commands/InviteServiceUser/`
- [ ] **Handler:** Create ServiceUser, send invitation
- [ ] **Validator:** Email unique

#### InviteOrganizationUserCommand
- [ ] **File:** `Application/Features/Users/Commands/InviteOrganizationUser/`
- [ ] **Handler:** Create OrganizationUser, send invitation

#### ActivateUserCommand
- [ ] **File:** `Application/Features/Users/Commands/ActivateUser/`
- [ ] **Handler:** Activate user account, set password

#### DeactivateUserCommand
- [ ] **File:** `Application/Features/Users/Commands/DeactivateUser/`
- [ ] **Handler:** Call `user.Deactivate()`

---

## 2.6 Domain Event Handlers 📢

**Priority:** 🟡 MEDIUM  
**Estimated Time:** 2-3 days

### 2.6.1 Ticket Event Handlers

- [ ] **`Application/Features/Tickets/EventHandlers/`**
  - [ ] `TicketCreatedEventHandler.cs`
    - [ ] Log event
    - [ ] Send notification (future)
  - [ ] `TicketAssignedEventHandler.cs`
    - [ ] Notify assigned technician
  - [ ] `TicketStatusChangedEventHandler.cs`
    - [ ] Notify stakeholders
  - [ ] `TicketResolvedEventHandler.cs`
    - [ ] Send resolution email to creator
  - [ ] `TicketClosedEventHandler.cs`
    - [ ] Archive ticket (future)

### 2.6.2 Machine Event Handlers

- [ ] **`Application/Features/Machines/EventHandlers/`**
  - [ ] `MachineAlarmActivatedEventHandler.cs`
    - [ ] Auto-create critical ticket
    - [ ] Send urgent notification
  - [ ] `MachineStatusChangedEventHandler.cs`
    - [ ] Log status change
  - [ ] `MaintenanceScheduledEventHandler.cs`
    - [ ] Create calendar reminder

### 2.6.3 Organization Event Handlers

- [ ] **`Application/Features/Organizations/EventHandlers/`**
  - [ ] `OrganizationCreatedEventHandler.cs`
  - [ ] `OrganizationServiceSuspendedEventHandler.cs`
    - [ ] Notify organization admin
    - [ ] Disable machine API tokens

---

## 2.7 Application Services 🔧

**Priority:** 🟡 MEDIUM  
**Estimated Time:** 2-3 days

### 2.7.1 Ticket Number Generator Service

- [ ] **File:** `Application/Services/TicketNumberGeneratorService.cs`
  - [ ] Implements `ITicketNumberGenerator` from Domain
  - [ ] Queries last ticket number from database
  - [ ] Generates next sequential number
  - [ ] Handles concurrency with database-level locking
  - [ ] Format: `TICK-{year}-{sequential:D5}`

### 2.7.2 Email Service (Interface only)

- [ ] **File:** `Application/Common/Interfaces/IEmailService.cs`
  - [ ] `SendInvitationEmailAsync(string to, string inviteLink, ct)`
  - [ ] `SendTicketNotificationAsync(TicketDto ticket, string recipientEmail, ct)`
  - [ ] `SendPasswordResetEmailAsync(string to, string resetLink, ct)`
  - [ ] Implementation will be in Infrastructure layer

### 2.7.3 File Storage Service (Interface)

- [ ] **File:** `Application/Common/Interfaces/IFileStorageService.cs`
  - [ ] `UploadAsync(Stream file, string fileName, string contentType, ct)`
  - [ ] `DeleteAsync(string filePath, ct)`
  - [ ] `GetDownloadUrlAsync(string filePath, ct)`

---

## 2.8 Dependency Injection Configuration 🔌

**Priority:** 🔥 CRITICAL  
**Estimated Time:** 1 day

- [ ] **File:** `Application/DependencyInjection.cs`
  ```csharp
  public static class DependencyInjection
  {
      public static IServiceCollection AddApplicationServices(
          this IServiceCollection services)
      {
          // MediatR
          services.AddMediatR(cfg => {
              cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
          });
          
          // Pipeline Behaviors (order matters)
          services.AddTransient(typeof(IPipelineBehavior<,>), 
              typeof(UnhandledExceptionBehaviour<,>));
          services.AddTransient(typeof(IPipelineBehavior<,>), 
              typeof(ValidationBehaviour<,>));
          services.AddTransient(typeof(IPipelineBehavior<,>), 
              typeof(PerformanceBehaviour<,>));
          
          // FluentValidation
          services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
          
          // AutoMapper
          services.AddAutoMapper(Assembly.GetExecutingAssembly());
          
          // Application Services
          services.AddScoped<ITicketNumberGenerator, TicketNumberGeneratorService>();
          
          return services;
      }
  }
  ```

---

## 2.9 Unit Tests 🧪

**Priority:** 🟡 MEDIUM  
**Estimated Time:** 4-5 days (parallel with development)

### Test Structure

```
Flowertrack.Application.Tests/
├── Features/
│   ├── Tickets/
│   │   ├── Commands/
│   │   │   ├── CreateTicketCommandHandlerTests.cs
│   │   │   ├── AssignTicketCommandHandlerTests.cs
│   │   │   └── ...
│   │   ├── Queries/
│   │   │   ├── GetTicketsQueryHandlerTests.cs
│   │   │   └── GetTicketByIdQueryHandlerTests.cs
│   │   └── EventHandlers/
│   │       └── TicketCreatedEventHandlerTests.cs
│   ├── Organizations/...
│   └── Machines/...
├── Common/
│   ├── Behaviours/
│   │   ├── ValidationBehaviourTests.cs
│   │   └── PerformanceBehaviourTests.cs
│   └── Mappings/
│       └── MappingTests.cs
└── TestHelpers/
    ├── MockRepositories/
    ├── FakeData/
    └── TestBase.cs
```

### Test Coverage Goals

- [ ] **Commands:** 90%+ coverage
  - [ ] Happy path scenarios
  - [ ] Validation failures
  - [ ] Business rule violations
  - [ ] Permission checks
- [ ] **Queries:** 85%+ coverage
  - [ ] Filtering logic
  - [ ] Pagination
  - [ ] Sorting
  - [ ] Authorization
- [ ] **Validators:** 100% coverage
  - [ ] All validation rules
  - [ ] Edge cases
- [ ] **Event Handlers:** 80%+ coverage
  - [ ] Event processing
  - [ ] Side effects
- [ ] **Pipeline Behaviors:** 95%+ coverage
  - [ ] Validation behavior
  - [ ] Exception handling
  - [ ] Performance logging

---

## Progress Tracking

| Section | Tasks | Completed | Progress |
|---------|-------|-----------|----------|
| 2.0 Repository Interfaces | 7 | 0 | 0% ⚪ |
| 2.1 Infrastructure | 15 | 0 | 0% ⚪ |
| 2.2 Tickets Feature | 25 | 0 | 0% ⚪ |
| 2.3 Organizations Feature | 12 | 0 | 0% ⚪ |
| 2.4 Machines Feature | 10 | 0 | 0% ⚪ |
| 2.5 Users Feature | 8 | 0 | 0% ⚪ |
| 2.6 Event Handlers | 9 | 0 | 0% ⚪ |
| 2.7 Application Services | 3 | 0 | 0% ⚪ |
| 2.8 DI Configuration | 1 | 0 | 0% ⚪ |
| 2.9 Unit Tests | 30 | 0 | 0% ⚪ |
| **TOTAL** | **120** | **0** | **0%** |

---

## Success Criteria

- [ ] All repository interfaces defined and documented
- [ ] All CQRS Commands and Queries implemented for core features
- [ ] FluentValidation rules for all commands/queries
- [ ] AutoMapper profiles for all DTOs
- [ ] Pipeline behaviors working correctly
- [ ] All domain events have handlers
- [ ] Unit test coverage > 80% for Application layer
- [ ] All commands/queries follow consistent patterns
- [ ] Proper error handling with Result pattern
- [ ] Zero compilation errors/warnings
- [ ] Code passes SonarQube quality gate (if configured)

---

## Architecture Decisions

### ✅ Adopted Patterns

1. **CQRS with MediatR**
   - Clear separation of reads and writes
   - Handlers are focused and testable
   - Pipeline behaviors for cross-cutting concerns

2. **Vertical Slice Architecture**
   - Features organized by business capability
   - Minimizes coupling between features
   - Each slice is independent

3. **Result Pattern**
   - Use Ardalis.Result for business logic returns
   - Exceptions only for exceptional cases
   - Better API error handling

4. **Specification Pattern**
   - Complex queries encapsulated in Specifications
   - Reusable query logic
   - Testable queries

5. **Repository Pattern**
   - Abstraction over data access
   - Domain doesn't know about EF Core
   - Easier to test

### ❌ Explicitly Not Using

1. **Anemic Domain Model** - Domain has behavior, not just properties
2. **Service Layer Anti-pattern** - Logic in domain entities and handlers
3. **Generic services** - Prefer specific command/query handlers

---

## Next Phase Preview

**Phase 3: Infrastructure Layer** will include:
- EF Core DbContext implementation
- Repository implementations
- Supabase integration
- Authentication/Authorization
- File storage
- Email service
- Background jobs
- Database migrations

---

## Notes

### Feature Organization Example

```
Application/Features/Tickets/
├── Commands/
│   ├── CreateTicket/
│   │   ├── CreateTicketCommand.cs
│   │   ├── CreateTicketCommandHandler.cs
│   │   ├── CreateTicketCommandValidator.cs
│   │   └── TicketCreatedDto.cs
│   └── AssignTicket/
│       └── ...
├── Queries/
│   ├── GetTickets/
│   │   ├── GetTicketsQuery.cs
│   │   ├── GetTicketsQueryHandler.cs
│   │   ├── GetTicketsQueryValidator.cs
│   │   └── TicketDto.cs
│   └── GetTicketById/
│       └── ...
└── EventHandlers/
    ├── TicketCreatedEventHandler.cs
    └── ...
```

### Command/Query Naming Convention

- Commands: `{Verb}{Entity}Command` (e.g., CreateTicketCommand)
- Queries: `Get{Entity}[s][By{Criteria}]Query` (e.g., GetTicketsQuery, GetTicketByIdQuery)
- DTOs: `{Entity}Dto` or `{Entity}DetailDto`

### Best Practices

1. **Keep handlers thin** - Orchestrate, don't implement business logic
2. **Domain logic in domain** - Handlers call domain methods
3. **Validate early** - Use FluentValidation in pipeline
4. **Async all the way** - All I/O operations async
5. **Use CancellationToken** - Proper cancellation support
6. **Immutable commands/queries** - Use records
7. **Test independently** - Mock dependencies, test in isolation

---

**Last Updated:** 2025-10-26  
**Version:** 1.0
