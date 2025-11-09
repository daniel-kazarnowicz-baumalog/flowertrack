using Flowertrack.Application.Auth.Commands.Login;
using Flowertrack.Application.Tickets.Commands.AddComment;
using Flowertrack.Application.Tickets.Commands.AddNote;
using Flowertrack.Application.Tickets.Queries.GetTicketHistory;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using Flowertrack.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flowertrack.Application.Tests.Integration;

/// <summary>
/// Integration tests for Phase A - Authentication and History features
/// </summary>
public class PhaseAIntegrationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public PhaseAIntegrationTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task LoginCommand_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        var command = new LoginCommand(
            "test@example.com",
            "TestPassword123!",
            "127.0.0.1");

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.User.Should().NotBeNull();
    }

    [Fact]
    public async Task AddCommentCommand_ShouldCreateHistoryEntry()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        
        // Create a test ticket first
        var ticketNumber = TicketNumber.Create(DateTime.UtcNow.Year, 1);
        var ticket = Ticket.Create(
            ticketNumber,
            "Test Ticket",
            "Test Description",
            Guid.NewGuid(), // organizationId
            Guid.NewGuid(), // machineId
            Priority.Medium,
            Guid.NewGuid()); // createdBy

        await ticketRepository.AddAsync(ticket);

        var command = new AddCommentCommand(
            ticket.Id,
            "Test comment",
            Guid.NewGuid(),
            "Test User",
            "ServiceUser",
            false);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AddNoteCommand_WithNonServiceUser_ShouldFail()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        var command = new AddNoteCommand(
            Guid.NewGuid(),
            "Internal note",
            Guid.NewGuid(),
            "Test User",
            "OrganizationUser"); // Not a service user

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Only service users");
    }

    [Fact]
    public async Task GetTicketHistoryQuery_ShouldReturnAllPublicEntries()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var historyRepository = scope.ServiceProvider.GetRequiredService<ITicketHistoryRepository>();

        // Create test ticket
        var ticketNumber = TicketNumber.Create(DateTime.UtcNow.Year, 2);
        var ticket = Ticket.Create(
            ticketNumber,
            "Test Ticket",
            "Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Priority.Medium,
            Guid.NewGuid());

        await ticketRepository.AddAsync(ticket);

        // Add public comment
        var publicComment = TicketHistory.CreateComment(
            ticket.Id,
            "Public comment",
            Guid.NewGuid(),
            "User1",
            "ServiceUser",
            false);

        // Add internal note
        var internalNote = TicketHistory.CreateNote(
            ticket.Id,
            "Internal note",
            Guid.NewGuid(),
            "User2",
            "ServiceUser");

        await historyRepository.AddAsync(publicComment);
        await historyRepository.AddAsync(internalNote);

        var query = new GetTicketHistoryQuery(
            ticket.Id,
            IncludeInternal: false);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(1); // Only public comment
        result.Value.Items.First().Content.Should().Be("Public comment");
    }

    [Fact]
    public async Task GetTicketHistoryQuery_WithIncludeInternal_ShouldReturnAllEntries()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
        var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var historyRepository = scope.ServiceProvider.GetRequiredService<ITicketHistoryRepository>();

        // Create test ticket
        var ticketNumber = TicketNumber.Create(DateTime.UtcNow.Year, 3);
        var ticket = Ticket.Create(
            ticketNumber,
            "Test Ticket 2",
            "Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Priority.High,
            Guid.NewGuid());

        await ticketRepository.AddAsync(ticket);

        // Add entries
        var publicComment = TicketHistory.CreateComment(
            ticket.Id,
            "Public",
            Guid.NewGuid(),
            "User1",
            "ServiceUser",
            false);

        var internalNote = TicketHistory.CreateNote(
            ticket.Id,
            "Internal",
            Guid.NewGuid(),
            "User2",
            "ServiceUser");

        await historyRepository.AddAsync(publicComment);
        await historyRepository.AddAsync(internalNote);

        var query = new GetTicketHistoryQuery(
            ticket.Id,
            IncludeInternal: true);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2); // Both entries
    }
}

/// <summary>
/// Test fixture for setting up test infrastructure
/// </summary>
public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public TestFixture()
    {
        var services = new ServiceCollection();
        
        // Configure test services here
        // Add DbContext with in-memory database
        // Add MediatR
        // Add repositories
        // Add other required services
        
        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
