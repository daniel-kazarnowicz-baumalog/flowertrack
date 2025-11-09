# 🚀 Quick Start Guide - Backend Development

## Before You Start

### Prerequisites
```powershell
# Verify .NET installation
dotnet --version
# Should be 10.0.x

# Verify database connection
dotnet user-secrets list
# Should show connection strings

# Verify NuGet packages
dotnet restore
```

---

## Development Setup

### 1. Create Feature Branch
```powershell
git checkout develop
git pull origin develop
git checkout -b feature/tickets-crud-operations
```

### 2. Verify Build
```powershell
cd src/backend/
dotnet build
# Should compile without errors
```

### 3. Create Folder Structure
```powershell
# Application Layer
mkdir Core\Flowertrack.Application\Tickets\Commands\CreateTicket
mkdir Core\Flowertrack.Application\Tickets\Commands\UpdateTicket
mkdir Core\Flowertrack.Application\Tickets\Commands\DeleteTicket
mkdir Core\Flowertrack.Application\Tickets\Commands\UpdateTicketStatus
mkdir Core\Flowertrack.Application\Tickets\Commands\AssignTicket
mkdir Core\Flowertrack.Application\Tickets\Queries\GetTicket
mkdir Core\Flowertrack.Application\Tickets\Queries\GetTickets
mkdir Core\Flowertrack.Application\Tickets\Queries\GetTicketsGroupedByStatus

# Contracts Layer
mkdir Presentation\Flowertrack.Contracts\Tickets\Requests
mkdir Presentation\Flowertrack.Contracts\Tickets\Responses

# API Layer
# (TicketsController goes in Controllers folder)

# Tests
mkdir Tests\Flowertrack.Application.Tests\Tickets\Commands
mkdir Tests\Flowertrack.Application.Tests\Tickets\Queries
mkdir Tests\Flowertrack.Api.IntegrationTests\Tickets
```

---

## Creating a Feature - Step by Step

### Example: CreateTicketCommand

#### Step 1: Create Command Class
**File:** `Core/Flowertrack.Application/Tickets/Commands/CreateTicket/CreateTicketCommand.cs`

```csharp
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    Guid OrganizationId,
    Guid MachineId,
    string Title,
    string Description,
    TicketPriority Priority) : IRequest<CreateTicketResponse>;
```

**Key Points:**
- Use `record` for immutability
- Inherit from `IRequest<T>` for MediatR
- T = Response type

#### Step 2: Create Validator
**File:** `Core/Flowertrack.Application/Tickets/Commands/CreateTicket/CreateTicketCommandValidator.cs`

```csharp
using FluentValidation;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("Machine ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(10).WithMessage("Title must be at least 10 characters")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(20).WithMessage("Description must be at least 20 characters")
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");
    }
}
```

#### Step 3: Create Handler
**File:** `Core/Flowertrack.Application/Tickets/Commands/CreateTicket/CreateTicketCommandHandler.cs`

```csharp
using MediatR;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateTicketCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CreateTicketResponse> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify organization exists
        var organization = await _unitOfWork.Organizations.GetByIdAsync(request.OrganizationId);
        if (organization is null)
            throw new NotFoundException(nameof(Organization), request.OrganizationId);

        // 2. Verify machine exists and belongs to organization
        var machine = await _unitOfWork.Machines.GetByIdAsync(request.MachineId);
        if (machine is null)
            throw new NotFoundException(nameof(Machine), request.MachineId);

        if (machine.OrganizationId != request.OrganizationId)
            throw new ForbiddenException("Machine does not belong to this organization");

        // 3. Create ticket using domain method
        var ticket = Ticket.Create(
            organizationId: request.OrganizationId,
            machineId: request.MachineId,
            title: request.Title,
            description: request.Description,
            priority: request.Priority,
            createdBy: _currentUserService.UserId);

        // 4. Add to repository
        await _unitOfWork.Tickets.AddAsync(ticket);

        // 5. Commit and raise events
        await _unitOfWork.SaveChangesAsync();

        // 6. Return response
        return new CreateTicketResponse(
            TicketId: ticket.Id,
            TicketNumber: ticket.Number.Value);
    }
}
```

#### Step 4: Create DTOs
**File:** `Presentation/Flowertrack.Contracts/Tickets/Requests/CreateTicketRequest.cs`

```csharp
namespace Flowertrack.Contracts.Tickets.Requests;

public record CreateTicketRequest(
    Guid OrganizationId,
    Guid MachineId,
    string Title,
    string Description,
    string Priority);
```

**File:** `Presentation/Flowertrack.Contracts/Tickets/Responses/CreateTicketResponse.cs`

```csharp
namespace Flowertrack.Contracts.Tickets.Responses;

public record CreateTicketResponse(
    Guid TicketId,
    string TicketNumber);
```

#### Step 5: Create API Endpoint
**File:** `Presentation/Flowertrack.Api/Controllers/TicketsController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Flowertrack.Contracts.Tickets.Requests;
using Flowertrack.Contracts.Tickets.Responses;
using Flowertrack.Application.Tickets.Commands.CreateTicket;

namespace Flowertrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new ticket
    /// </summary>
    /// <param name="request">Ticket creation request</param>
    /// <returns>Created ticket response</returns>
    /// <response code="201">Ticket created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="403">Forbidden - no permission</response>
    /// <response code="404">Organization or Machine not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateTicketResponse>> CreateTicket(
        [FromBody] CreateTicketRequest request)
    {
        var command = new CreateTicketCommand(
            request.OrganizationId,
            request.MachineId,
            request.Title,
            request.Description,
            Enum.Parse<TicketPriority>(request.Priority));

        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetTicket), new { ticketId = result.TicketId }, result);
    }

    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("{ticketId}")]
    public async Task<ActionResult<TicketResponse>> GetTicket(Guid ticketId)
    {
        // Implementation
        return Ok();
    }
}
```

#### Step 6: Create Unit Tests
**File:** `Tests/Flowertrack.Application.Tests/Tickets/Commands/CreateTicketCommandHandlerTests.cs`

```csharp
using Moq;
using Xunit;
using FluentAssertions;
using Flowertrack.Application.Tickets.Commands.CreateTicket;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tests.Tickets.Commands;

public class CreateTicketCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateTicket()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var machineId = Guid.NewGuid();
        var organization = new Organization { Id = organizationId };
        var machine = new Machine { Id = machineId, OrganizationId = organizationId };

        _unitOfWorkMock.Setup(x => x.Organizations.GetByIdAsync(organizationId))
            .ReturnsAsync(organization);
        _unitOfWorkMock.Setup(x => x.Machines.GetByIdAsync(machineId))
            .ReturnsAsync(machine);
        _currentUserServiceMock.Setup(x => x.UserId)
            .Returns("test-user");

        var command = new CreateTicketCommand(
            organizationId,
            machineId,
            "Valid Title Here",
            "Valid description with more than 20 characters",
            TicketPriority.High);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TicketId.Should().NotBeEmpty();
        result.TicketNumber.Should().NotBeNullOrEmpty();

        _unitOfWorkMock.Verify(x => x.Tickets.AddAsync(It.IsAny<Ticket>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrganization_ShouldThrowNotFoundException()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var machineId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Organizations.GetByIdAsync(organizationId))
            .ReturnsAsync((Organization)null);

        var command = new CreateTicketCommand(
            organizationId,
            machineId,
            "Valid Title",
            "Valid description text here",
            TicketPriority.Medium);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
```

#### Step 7: Create Integration Test
**File:** `Tests/Flowertrack.Api.IntegrationTests/Tickets/CreateTicketIntegrationTests.cs`

```csharp
using Xunit;
using FluentAssertions;
using Flowertrack.Api.IntegrationTests.Common;
using Flowertrack.Contracts.Tickets.Requests;

namespace Flowertrack.Api.IntegrationTests.Tickets;

public class CreateTicketIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public CreateTicketIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTicket_WithValidData_ShouldReturn201()
    {
        // Arrange
        var request = new CreateTicketRequest(
            OrganizationId: TestData.DefaultOrganizationId,
            MachineId: TestData.DefaultMachineId,
            Title: "Test Ticket Title",
            Description: "This is a test ticket description with sufficient length",
            Priority: "High");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tickets", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadAsAsync<CreateTicketResponse>();
        result.TicketId.Should().NotBeEmpty();
        result.TicketNumber.Should().StartWith("TICK-");
    }

    [Fact]
    public async Task CreateTicket_WithInvalidData_ShouldReturn400()
    {
        // Arrange
        var request = new CreateTicketRequest(
            OrganizationId: Guid.NewGuid(),
            MachineId: Guid.NewGuid(),
            Title: "Too short",
            Description: "Too short",
            Priority: "Invalid");

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tickets", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
```

---

## Command Patterns to Follow

### Repository Usage
```csharp
// Get by ID
var entity = await _unitOfWork.EntitySet.GetByIdAsync(id);

// Get all
var entities = await _unitOfWork.EntitySet.GetAllAsync();

// Add
await _unitOfWork.EntitySet.AddAsync(entity);

// Update
await _unitOfWork.EntitySet.UpdateAsync(entity);

// Delete
await _unitOfWork.EntitySet.DeleteAsync(entity);

// Save all changes
await _unitOfWork.SaveChangesAsync();
```

### Exception Handling
```csharp
// Not found
if (entity is null)
    throw new NotFoundException(nameof(Entity), id);

// Validation
if (invalid)
    throw new ValidationException("Field", "Error message");

// Forbidden
if (!hasPermission)
    throw new ForbiddenException("You don't have permission");

// Business rule conflict
if (violatesRule)
    throw new ConflictException("Business rule violated");
```

### Event Raising
```csharp
// In domain method
RaiseDomainEvent(new TicketCreatedEvent(
    TicketId: ticket.Id,
    TicketNumber: ticket.Number,
    OrganizationId: ticket.OrganizationId));

// Events are automatically raised by UnitOfWorkBehavior
// No need to handle them manually in handler
```

---

## Testing Patterns

### Unit Test Structure
```csharp
[Fact]
public async Task Handler_WithScenario_ShouldExpectation()
{
    // Arrange - Setup test data and mocks
    var mockRepository = new Mock<IRepository>();
    var handler = new YourHandler(mockRepository.Object);
    var command = new YourCommand(...);

    // Act - Execute the handler
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert - Verify the result
    result.Should().NotBeNull();
    mockRepository.Verify(x => x.Method(), Times.Once);
}
```

### Validator Test Pattern
```csharp
[Theory]
[InlineData("")]
[InlineData(null)]
public async Task Validate_WithInvalidTitle_ShouldFail(string title)
{
    var validator = new CreateTicketCommandValidator();
    var command = new CreateTicketCommand(..., title, ...);

    var result = await validator.ValidateAsync(command);

    result.IsValid.Should().BeFalse();
}
```

---

## Common Commands

### Build & Test
```powershell
# Build
dotnet build

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity=normal

# Run specific test
dotnet test --filter "ClassName=CreateTicketCommandHandlerTests"

# Code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Git Workflow
```powershell
# Check current branch
git status

# Stage changes
git add .

# Commit
git commit -m "feat(tickets): implement create ticket command"

# Push
git push origin feature/tickets-crud-operations

# Create PR (or use GitHub UI)
```

### Debugging
```csharp
// In Visual Studio
Debug > Start Debugging (F5)

// Breakpoints
Left-click on line number to add breakpoint

// Watch
Debug > Windows > Watch

// Output
Debug > Windows > Output
```

---

## File Templates

### Command Template
```csharp
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.[FeatureName];

public record [FeatureName]Command(...) : IRequest<[Response]>;
```

### Query Template
```csharp
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.[FeatureName];

public record [FeatureName]Query(...) : IRequest<[Response]>;
```

### Handler Template
```csharp
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.[FeatureName];

public class [FeatureName]CommandHandler : IRequestHandler<[FeatureName]Command, [Response]>
{
    private readonly IUnitOfWork _unitOfWork;

    public [FeatureName]CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<[Response]> Handle([FeatureName]Command request, CancellationToken cancellationToken)
    {
        // Implementation
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
```

---

## Useful References

### Domain Entity Location
- **Ticket:** `src/backend/Core/Flowertrack.Domain/Entities/Ticket.cs`
- **Machine:** `src/backend/Core/Flowertrack.Domain/Entities/Machine.cs`
- **Organization:** `src/backend/Core/Flowertrack.Domain/Entities/Organization.cs`

### Existing Implementations (Reference)
- **Organizations Command:** `src/backend/Core/Flowertrack.Application/Organizations/Commands/OnboardOrganization/`
- **Organizations Query:** `src/backend/Core/Flowertrack.Application/Organizations/Queries/GetOrganizations/`
- **Organizations Controller:** `src/backend/Presentation/Flowertrack.Api/Controllers/OrganizationsController.cs`

### Configuration Files
- **Application DI:** `src/backend/Core/Flowertrack.Application/DependencyInjection.cs`
- **Infrastructure DI:** `src/backend/Infrastructure/Flowertrack.Infrastructure/DependencyInjection.cs`
- **API Startup:** `src/backend/Presentation/Flowertrack.Api/Program.cs`

---

## Checklist Before Committing

- [ ] Code compiles without errors/warnings
- [ ] All tests pass locally
- [ ] Code coverage > 80%
- [ ] XML documentation added
- [ ] Swagger comments added
- [ ] No hardcoded values
- [ ] Proper error handling
- [ ] Following naming conventions
- [ ] Following CQRS pattern
- [ ] PR description ready

---

**Last Updated:** 09.11.2025  
**Version:** 1.0
