# Domain Layer

This directory contains the domain model for FLOWerTRACK, following Domain-Driven Design (DDD) principles.

## Structure

```
Domain/
├── Common/           # Base classes, interfaces, and shared domain types
├── Entities/         # Aggregate roots and entities
├── ValueObjects/     # Value objects
└── Events/           # Domain events
```

## Common

Base classes and interfaces used throughout the domain:

- **`AuditableEntity<TId>`** - Base class for all entities with audit trail
- **`IAggregateRoot`** - Marker interface for aggregate roots
- **`IDomainEvent`** - Interface for domain events
- **`DomainEvent`** - Base record for all domain events
- **`MachineStatus`** - Enum for machine operational status

## Entities

Aggregate roots and entities in the domain model:

- **`Machine`** - Production equipment/machine aggregate root

## Value Objects

Immutable value objects representing domain concepts:

- **`MachineApiKey`** - Secure API authentication token
- **`MaintenanceInterval`** - Maintenance scheduling configuration

## Events

Domain events raised by aggregates to signal state changes:

- **`MachineRegisteredEvent`** - New machine created
- **`MachineStatusChangedEvent`** - Machine status updated
- **`MachineApiTokenGeneratedEvent`** - API token generated/regenerated
- **`MachineMaintenanceScheduledEvent`** - Maintenance scheduled
- **`MachineAlarmActivatedEvent`** - Alarm activated
- **`MachineAlarmClearedEvent`** - Alarm cleared

## Design Principles

### Aggregate Roots

Aggregate roots:
- Inherit from `AuditableEntity<TId>`
- Implement `IAggregateRoot`
- Have a private parameterless constructor for EF Core
- Have a private parameterized constructor for domain logic
- Expose a public static factory method `Create()`
- Encapsulate all business logic
- Raise domain events for state changes
- Enforce invariants and business rules

### Value Objects

Value objects:
- Are immutable
- Have no identity (compared by value, not reference)
- Have a private constructor
- Expose a static factory method
- Implement equality operators
- Validate their state in the constructor

### Domain Events

Domain events:
- Inherit from `DomainEvent` record
- Are immutable
- Have an `OccurredOn` timestamp
- Contain only data (no behavior)
- Use past tense naming (e.g., `MachineRegisteredEvent`)

## Usage Examples

### Creating an Entity

```csharp
// Use static factory method
var machine = Machine.Create(
    organizationId: orgId,
    serialNumber: "SN-12345",
    brand: "Baumalog",
    model: "BL-2000"
);

// Check domain events
var events = machine.DomainEvents;
```

### Creating a Value Object

```csharp
// Generate new API key
var apiKey = MachineApiKey.Generate();

// Or reconstitute from database
var apiKey = MachineApiKey.FromValue(tokenValue);
```

### Handling Domain Events

```csharp
// Get all domain events from aggregate
var events = machine.DomainEvents;

// Process events (typically in application layer)
foreach (var @event in events)
{
    await eventPublisher.PublishAsync(@event);
}

// Clear events after publishing
machine.ClearDomainEvents();
```

## Testing

Tests for the domain layer are located in:
- **`Flowertrack.Domain.Tests`** project

To run domain tests:

```bash
cd src/backend
dotnet test Flowertrack.Domain.Tests
```

## Business Rules

Domain entities enforce business rules:

### Machine Entity

1. **Status Transitions**
   - Cannot transition from `Alarm` to `Active` without calling `ClearAlarm()`
   - All other transitions are allowed via `UpdateStatus()`

2. **Maintenance Scheduling**
   - Next maintenance must be in the future
   - Next maintenance must be after last maintenance

3. **API Token Management**
   - Token regeneration requires a reason
   - Tokens are cryptographically secure

## Dependencies

The domain layer should:
- ✅ Have **NO** dependencies on infrastructure
- ✅ Have **NO** dependencies on application layer
- ✅ Have **NO** dependencies on frameworks (except .NET base classes)
- ✅ Be pure C# with domain logic only

## Further Reading

- See `/docs/PHASE_1.1_MACHINE_ENTITY.md` for detailed Machine entity documentation
- DDD patterns: https://martinfowler.com/tags/domain%20driven%20design.html
- Aggregate pattern: https://martinfowler.com/bliki/DDD_Aggregate.html

## Version History

- **v1.0** (Phase 1.1) - Initial Machine entity implementation
