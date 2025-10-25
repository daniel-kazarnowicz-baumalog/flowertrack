namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Events;

public class DomainEventTests
{
    [Fact]
    public void DomainEvent_ShouldHaveEventId_WhenCreated()
    {
        // Arrange & Act
        var @event = new TicketCreatedEvent(
            Guid.NewGuid(),
            "TKT-2024-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "High"
        );

        // Assert
        Assert.NotEqual(Guid.Empty, @event.EventId);
    }

    [Fact]
    public void DomainEvent_ShouldHaveOccurredOn_WhenCreated()
    {
        // Arrange & Act
        var beforeCreation = DateTimeOffset.UtcNow;
        var @event = new TicketCreatedEvent(
            Guid.NewGuid(),
            "TKT-2024-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "High"
        );
        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        Assert.True(@event.OccurredOn >= beforeCreation);
        Assert.True(@event.OccurredOn <= afterCreation);
    }

    [Fact]
    public void DomainEvent_ShouldSetAggregateId_WhenProvided()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();

        // Act
        var @event = new TicketCreatedEvent(
            aggregateId,
            "TKT-2024-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "High"
        );

        // Assert
        Assert.Equal(aggregateId, @event.AggregateId);
    }
}
