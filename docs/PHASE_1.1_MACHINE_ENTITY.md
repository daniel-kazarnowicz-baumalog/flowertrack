# Phase 1.1: Machine Entity Implementation

## Overview

This document describes the implementation of the Machine entity as the first aggregate root in the FLOWerTRACK domain model. The Machine entity represents production equipment registered in the system and manages machine lifecycle, API authentication, maintenance scheduling, and alarm states.

## Implementation Date

**Completed:** October 2024 (Phase 1.1)

## Architecture

### Domain-Driven Design (DDD) Structure

The implementation follows DDD principles with the following structure:

```
Domain/
├── Common/
│   ├── AuditableEntity.cs       # Base class for entities with audit fields
│   ├── IAggregateRoot.cs        # Marker interface for aggregate roots
│   ├── IDomainEvent.cs          # Interface for domain events
│   ├── DomainEvent.cs           # Base class for domain events
│   └── MachineStatus.cs         # Enum for machine operational status
├── Entities/
│   └── Machine.cs               # Machine aggregate root
├── ValueObjects/
│   ├── MachineApiKey.cs         # Secure API token value object
│   └── MaintenanceInterval.cs   # Maintenance interval configuration
└── Events/
    ├── MachineRegisteredEvent.cs
    ├── MachineStatusChangedEvent.cs
    ├── MachineApiTokenGeneratedEvent.cs
    ├── MachineMaintenanceScheduledEvent.cs
    ├── MachineAlarmActivatedEvent.cs
    └── MachineAlarmClearedEvent.cs
```

## Components

### 1. Base Domain Classes

#### AuditableEntity&lt;TId&gt;

Base class for all entities providing:
- Unique identifier of type `TId`
- Audit fields: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Domain event collection and management
- Protected methods for raising and clearing domain events

#### IAggregateRoot

Marker interface identifying aggregate roots in the domain model. Provides:
- Access to domain events
- Method to clear domain events after publishing

#### IDomainEvent & DomainEvent

Base interface and class for domain events, including:
- `OccurredOn` timestamp (UTC)
- Automatic timestamp assignment on creation

### 2. Machine Entity

**Namespace:** `Flowertrack.Api.Domain.Entities`

#### Properties

| Property | Type | Description | Validation |
|----------|------|-------------|------------|
| `Id` | `Guid` | Unique identifier | Auto-generated |
| `OrganizationId` | `Guid` | Owner organization | Required, not empty |
| `SerialNumber` | `string` | Unique serial number | Required, max 255 chars |
| `Brand` | `string?` | Manufacturer brand | Optional, max 100 chars |
| `Model` | `string?` | Model name/number | Optional, max 100 chars |
| `Location` | `string?` | Physical location | Optional, max 255 chars |
| `Status` | `MachineStatus` | Operational status | Enum value |
| `ApiToken` | `MachineApiKey?` | API authentication token | Optional, unique |
| `LastMaintenanceDate` | `DateOnly?` | Last maintenance completion | Optional |
| `NextMaintenanceDate` | `DateOnly?` | Next scheduled maintenance | Optional |
| `MaintenanceIntervalId` | `int?` | Maintenance interval config ID | Optional |

#### Methods

##### Factory Method: Create()

```csharp
public static Machine Create(
    Guid organizationId,
    string serialNumber,
    string? brand = null,
    string? model = null,
    string? location = null,
    string? createdBy = null)
```

Creates a new machine with initial status `Inactive` and raises `MachineRegisteredEvent`.

##### GenerateApiToken()

```csharp
public void GenerateApiToken(string? updatedBy = null)
```

Generates a secure API token for machine authentication. Raises `MachineApiTokenGeneratedEvent`.

##### RegenerateApiToken()

```csharp
public void RegenerateApiToken(string reason, string? updatedBy = null)
```

Regenerates the API token (e.g., after security compromise). Requires a reason and raises `MachineApiTokenGeneratedEvent` with `IsRegeneration = true`.

##### UpdateStatus()

```csharp
public void UpdateStatus(MachineStatus newStatus, string reason, string? updatedBy = null)
```

Updates the machine's operational status. Enforces business rule: cannot transition from `Alarm` to `Active` directly (must use `ClearAlarm()`). Raises `MachineStatusChangedEvent`.

##### ScheduleMaintenance()

```csharp
public void ScheduleMaintenance(
    DateOnly date, 
    MaintenanceInterval? interval = null, 
    string? updatedBy = null)
```

Schedules future maintenance. Validates:
- Date must not be in the past
- Date must be after last maintenance date
Raises `MachineMaintenanceScheduledEvent`.

##### CompleteMaintenance()

```csharp
public void CompleteMaintenance(
    DateOnly completedDate, 
    MaintenanceInterval? interval = null, 
    string? updatedBy = null)
```

Records maintenance completion. If interval is provided, automatically schedules next maintenance. Raises `MachineMaintenanceScheduledEvent` if next maintenance is scheduled.

##### ActivateAlarm()

```csharp
public void ActivateAlarm(string reason, string? updatedBy = null)
```

Activates an alarm on the machine, changing status to `Alarm`. Raises `MachineAlarmActivatedEvent`.

##### ClearAlarm()

```csharp
public void ClearAlarm(string reason, string? updatedBy = null)
```

Clears an active alarm and restores machine to `Active` status. Business rule enforcement: can only be called when machine is in `Alarm` state. Raises both `MachineAlarmClearedEvent` and `MachineStatusChangedEvent`.

##### UpdateLocation()

```csharp
public void UpdateLocation(string location, string? updatedBy = null)
```

Updates the physical location of the machine.

### 3. MachineStatus Enum

**Namespace:** `Flowertrack.Api.Domain.Common`

```csharp
public enum MachineStatus
{
    Inactive = 0,      // Registered but not operational
    Active = 1,        // Running normally
    Maintenance = 2,   // Under maintenance
    Alarm = 3,         // Active alarm/error
    OutOfService = 4   // Decommissioned
}
```

### 4. Value Objects

#### MachineApiKey

**Namespace:** `Flowertrack.Api.Domain.ValueObjects`

Represents a secure API key for machine authentication.

**Features:**
- Cryptographically secure token generation using `RandomNumberGenerator`
- 32-character minimum length
- Base-62 character set (A-Z, a-z, 0-9)
- Automatic generation timestamp
- Masked string representation for security (shows only first 8 chars)

**Methods:**
- `Generate()` - Generate new secure token
- `FromValue(string)` - Reconstitute from database value
- Equality operators and `GetHashCode()` override

#### MaintenanceInterval

**Namespace:** `Flowertrack.Api.Domain.ValueObjects`

Represents a maintenance interval configuration.

**Properties:**
- `Id` - Unique identifier
- `Days` - Number of days between maintenance
- `Description` - Human-readable description

**Methods:**
- `Create(int id, int days, string description)` - Factory method
- `CalculateNextDate(DateOnly fromDate)` - Calculate next maintenance date

### 5. Domain Events

All domain events inherit from `DomainEvent` and include `OccurredOn` timestamp.

#### MachineRegisteredEvent

Raised when a new machine is created.

**Properties:**
- `MachineId`
- `OrganizationId`
- `SerialNumber`
- `Brand`
- `Model`

#### MachineStatusChangedEvent

Raised when machine status changes.

**Properties:**
- `MachineId`
- `PreviousStatus`
- `NewStatus`
- `Reason`

#### MachineApiTokenGeneratedEvent

Raised when API token is generated or regenerated.

**Properties:**
- `MachineId`
- `IsRegeneration` (bool)
- `Reason`

#### MachineMaintenanceScheduledEvent

Raised when maintenance is scheduled.

**Properties:**
- `MachineId`
- `ScheduledDate`
- `MaintenanceIntervalId`

#### MachineAlarmActivatedEvent

Raised when an alarm is activated.

**Properties:**
- `MachineId`
- `Reason`
- `PreviousStatus`

#### MachineAlarmClearedEvent

Raised when an alarm is cleared.

**Properties:**
- `MachineId`
- `Reason`

## Business Rules

### 1. Machine Status Transitions

- ✅ Any status can transition to `Alarm` via `ActivateAlarm()`
- ✅ Any status can transition to any other status via `UpdateStatus()` (except Alarm → Active)
- ❌ **Cannot** transition from `Alarm` to `Active` using `UpdateStatus()` - must use `ClearAlarm()`

### 2. Maintenance Scheduling

- ✅ Next maintenance date must be in the future
- ✅ Next maintenance date must be after last maintenance date
- ✅ Completing maintenance can automatically schedule next maintenance if interval provided

### 3. API Token Management

- ✅ Token generation is optional (machine can exist without token)
- ✅ Token regeneration requires a reason
- ✅ Tokens are cryptographically secure and unique

### 4. Validation Rules

- ✅ Serial number is required and must be unique (to be enforced at repository level)
- ✅ OrganizationId is required
- ✅ String length limits enforced for all text fields
- ✅ API token must be at least 32 characters

## Testing

### Test Project

**Project:** `Flowertrack.Domain.Tests`
**Framework:** xUnit
**Total Tests:** 69

### Test Coverage

#### MachineTests (52 tests)

**Creation Tests:**
- Valid machine creation with all parameters
- Domain event verification
- Validation of required fields
- String length validation

**API Token Tests:**
- Token generation and regeneration
- Event raising verification
- Validation of regeneration reason

**Status Management Tests:**
- Status updates with event raising
- Business rule enforcement (Alarm → Active)
- Same status handling

**Maintenance Tests:**
- Scheduling with validation
- Completion with automatic next scheduling
- Date validation rules

**Alarm Tests:**
- Alarm activation and clearing
- Status transition verification
- Multiple events on clear
- Business rule enforcement

**Location Tests:**
- Location update
- Validation

#### MachineApiKeyTests (15 tests)

- Secure token generation
- Token uniqueness
- Validation rules
- Equality operations
- String masking

#### MaintenanceIntervalTests (7 tests)

- Creation with validation
- Date calculation
- Various interval durations

### Running Tests

```bash
cd src/backend
dotnet test
```

**Result:** All 69 tests passing ✅

## Usage Examples

### Creating a Machine

```csharp
var machine = Machine.Create(
    organizationId: orgGuid,
    serialNumber: "SN-12345",
    brand: "Baumalog",
    model: "BL-2000",
    location: "Factory Floor A",
    createdBy: "admin@example.com"
);
```

### Generating API Token

```csharp
machine.GenerateApiToken("admin@example.com");
var token = machine.ApiToken.Value; // Use for API authentication
```

### Activating Machine

```csharp
machine.UpdateStatus(
    MachineStatus.Active, 
    "Initial activation after installation",
    "technician@example.com"
);
```

### Handling Alarms

```csharp
// When alarm occurs
machine.ActivateAlarm(
    "Temperature exceeded threshold", 
    "system"
);

// When issue resolved
machine.ClearAlarm(
    "Temperature normalized", 
    "technician@example.com"
);
```

### Scheduling Maintenance

```csharp
var interval = MaintenanceInterval.Create(1, 90, "Quarterly");
var nextDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90));

machine.ScheduleMaintenance(nextDate, interval, "admin@example.com");
```

### Completing Maintenance

```csharp
var completedDate = DateOnly.FromDateTime(DateTime.UtcNow);
var interval = MaintenanceInterval.Create(1, 90, "Quarterly");

// Automatically schedules next maintenance
machine.CompleteMaintenance(completedDate, interval, "technician@example.com");
```

## Integration Points

### Future Dependencies

This implementation prepares for:

1. **Organization Entity** - Referenced via `OrganizationId` (to be implemented)
2. **Ticket Entity** - Machines will have a collection of tickets (to be implemented)
3. **Repository Pattern** - Interface to be defined for persistence
4. **Domain Event Publishing** - Events to be published to event bus
5. **EF Core Mapping** - Entity configuration for database persistence

### Database Considerations

When implementing persistence:

1. **Unique Constraints:**
   - `SerialNumber` must be unique across all machines
   - `ApiToken.Value` must be unique across all machines

2. **Indexes:**
   - Consider indexing `OrganizationId` for query performance
   - Consider indexing `Status` for filtering active machines
   - Consider indexing `SerialNumber` for lookup

3. **Value Object Storage:**
   - `MachineApiKey` should be stored as owned entity or converted to/from string
   - `MaintenanceInterval` may be stored as separate table or inline

## Security Considerations

### API Token Security

1. **Generation:** Uses `RandomNumberGenerator` from `System.Security.Cryptography`
2. **Storage:** Tokens should be hashed in database (not implemented yet)
3. **Transmission:** Always use HTTPS for token transmission
4. **Display:** ToString() masks token (shows only first 8 characters)
5. **Regeneration:** Supported when token is compromised

### Audit Trail

All modifications record:
- Who made the change (`UpdatedBy`)
- When the change occurred (`UpdatedAt`)
- Original creation metadata preserved

## Next Steps

### Phase 1.2 (Upcoming)

1. Organization entity implementation
2. Repository interfaces and EF Core implementation
3. Database migrations for Machine table
4. API endpoints for machine management

### Phase 1.3 (Upcoming)

1. Domain event publishing infrastructure
2. Event handlers for machine events
3. Integration tests with database

## References

- Issue: Phase 1.1 - Implement Machine Entity (Aggregate Root)
- DDD Pattern: Aggregate Root
- Architecture: Clean Architecture / Onion Architecture
- Test Framework: xUnit
- .NET Version: 9.0

## Contributors

- Implementation: GitHub Copilot
- Review: (Pending)
- Testing: Comprehensive unit tests included

---

**Status:** ✅ Complete
**Last Updated:** October 2024
