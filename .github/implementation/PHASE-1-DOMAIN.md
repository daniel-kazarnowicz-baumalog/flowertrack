# FAZA 1: Domain Layer 🏗️

**Status:** ⚪ Not Started  
**Started:** TBD  
**Target Completion:** Sprint 1-2 (Week 1-3)

---

## 1.1 Core Entities 📦

### Ticket Entity
- [ ] Utworzyć `Ticket.cs` w `Domain/Entities/`
- [ ] Properties:
  - [ ] `Id` (Guid, Primary Key)
  - [ ] `TicketNumber` (TicketNumber value object)
  - [ ] `Title` (string, max 255)
  - [ ] `Description` (string, max 5000)
  - [ ] `OrganizationId` (Guid, FK)
  - [ ] `MachineId` (Guid, FK)
  - [ ] `Status` (TicketStatus enum)
  - [ ] `Priority` (Priority enum)
  - [ ] `CreatedByUserId` (Guid, FK)
  - [ ] `AssignedToUserId` (Guid?, nullable FK)
  - [ ] `CreatedAt` (DateTimeOffset)
  - [ ] `UpdatedAt` (DateTimeOffset)
  - [ ] `ResolvedAt` (DateTimeOffset?)
  - [ ] `ClosedAt` (DateTimeOffset?)
  - [ ] Navigation properties: Organization, Machine, CreatedBy, AssignedTo, History, Attachments
- [ ] Domain Methods:
  - [ ] `UpdateStatus(TicketStatus newStatus, string reason, Guid userId)`
  - [ ] `AssignTo(Guid userId, Guid assignedBy)`
  - [ ] `AddComment(string text, Guid userId, bool isInternal)`
  - [ ] `AddAttachment(string fileName, string filePath, Guid userId)`
  - [ ] `Resolve(string reason, Guid userId)`
  - [ ] `Close(string reason, Guid userId)`
  - [ ] `Reopen(string reason, Guid userId)`
- [ ] Domain Events:
  - [ ] Raise `TicketCreatedEvent` w konstruktorze
  - [ ] Raise `TicketStatusChangedEvent` w UpdateStatus
  - [ ] Raise `TicketAssignedEvent` w AssignTo
- [ ] Validation Rules:
  - [ ] Title nie może być pusty
  - [ ] Status transitions validation
  - [ ] Cannot assign to inactive user

### Organization Entity
- [ ] Utworzyć `Organization.cs`
- [ ] Properties:
  - [ ] `Id` (Guid, PK)
  - [ ] `Name` (string, max 255, required)
  - [ ] `Street` (string, max 255)
  - [ ] `City` (string, max 100)
  - [ ] `PostalCode` (string, max 20)
  - [ ] `Country` (string, max 100)
  - [ ] `ServiceStatus` (ServiceStatus enum)
  - [ ] `CreatedAt`, `UpdatedAt`
  - [ ] Navigation: Machines, Tickets, Users, Contacts
- [ ] Methods:
  - [ ] `UpdateServiceStatus(ServiceStatus status)`
  - [ ] `AddContact(OrganizationContact contact)`
  - [ ] `RegisterMachine(Machine machine)`
- [ ] Validation:
  - [ ] Name required
  - [ ] Unique name constraint

### Machine Entity
- [ ] Utworzyć `Machine.cs`
- [ ] Properties:
  - [ ] `Id` (Guid, PK)
  - [ ] `OrganizationId` (Guid, FK)
  - [ ] `SerialNumber` (string, unique, required, max 255)
  - [ ] `Brand` (string, max 100)
  - [ ] `Model` (string, max 100)
  - [ ] `Location` (string, max 255)
  - [ ] `Status` (MachineStatus enum)
  - [ ] `ApiToken` (MachineApiKey value object, unique)
  - [ ] `LastMaintenanceDate` (DateOnly?)
  - [ ] `NextMaintenanceDate` (DateOnly?)
  - [ ] `MaintenanceIntervalId` (int?, FK)
  - [ ] `CreatedAt`, `UpdatedAt`
  - [ ] Navigation: Organization, Tickets, Logs
- [ ] Methods:
  - [ ] `GenerateApiToken()` - secure token generation
  - [ ] `UpdateStatus(MachineStatus status)`
  - [ ] `ScheduleMaintenance(DateOnly date, MaintenanceInterval interval)`
  - [ ] `RecordLog(string logContent)`
- [ ] Validation:
  - [ ] Serial number unique
  - [ ] API token unique

### User (Profile Extensions)
- [ ] Utworzyć `ServiceUser.cs` (profil serwisanta)
  - [ ] `UserId` (Guid, PK, FK to auth.users)
  - [ ] `FirstName` (string, max 100, required)
  - [ ] `LastName` (string, max 100, required)
  - [ ] `PhoneNumber` (string, max 50)
  - [ ] `Status` (UserStatus enum)
  - [ ] `CreatedAt`, `UpdatedAt`
- [ ] Utworzyć `OrganizationUser.cs` (profil klienta)
  - [ ] `UserId` (Guid, PK, FK)
  - [ ] `FirstName`, `LastName`
  - [ ] `OrganizationId` (Guid, FK, required)
  - [ ] `Status` (UserStatus enum)
  - [ ] `CreatedAt`, `UpdatedAt`
- [ ] Methods:
  - [ ] `Activate()`, `Deactivate()`
  - [ ] `UpdateProfile(firstName, lastName, phone?)`

### Comment Entity
- [ ] Utworzyć `Comment.cs` jako część TicketHistory
- [ ] Properties w `TicketHistory`:
  - [ ] `EventType` = "COMMENT"
  - [ ] `Details` JSONB zawiera: text, isInternal
  
### Attachment Entity
- [ ] Utworzyć `Attachment.cs`
- [ ] Properties:
  - [ ] `Id` (Guid, PK)
  - [ ] `TicketId` (Guid, FK, required)
  - [ ] `UploadedByUserId` (Guid, FK, required)
  - [ ] `FileName` (string, max 255, required)
  - [ ] `FilePath` (string, max 1024, required)
  - [ ] `FileSizeBytes` (long, required)
  - [ ] `MimeType` (string, max 100, required)
  - [ ] `CreatedAt`
  - [ ] Navigation: Ticket, UploadedBy
- [ ] Validation:
  - [ ] Max file size: 10MB (10485760 bytes)
  - [ ] Allowed mime types validation

### AuditLog Entity
- [ ] Utworzyć `AuditLog.cs`
- [ ] Properties:
  - [ ] `Id` (long, PK, auto-increment)
  - [ ] `UserId` (Guid?, FK)
  - [ ] `ActionType` (string, max 100, required)
  - [ ] `TargetResourceId` (string, max 255)
  - [ ] `Details` (JSONB)
  - [ ] `IpAddress` (IPAddress?)
  - [ ] `UserAgent` (string)
  - [ ] `CreatedAt`
- [ ] Methods:
  - [ ] Static factory methods dla różnych action types

---

## 1.2 Value Objects 💎

- [ ] **TicketNumber**
  - [ ] Format: `TICK-{year}-{sequential}`
  - [ ] Validation: regex pattern
  - [ ] Equality overrides
  - [ ] ToString() override

- [ ] **Email**
  - [ ] Validation: regex pattern for email
  - [ ] Normalization (lowercase)
  - [ ] Equality overrides

- [ ] **Priority** (Enum)
  - [ ] Values: Low = 1, Medium = 2, High = 3, Critical = 4
  - [ ] Display names (lokalizacja)
  - [ ] Color mapping

- [ ] **TicketStatus** (Enum)
  - [ ] Values: Draft, Sent, Accepted, InProgress, Resolved, Reopened, Closed, Archived
  - [ ] Validation dla allowed transitions
  - [ ] Display names

- [ ] **MachineStatus** (Enum)
  - [ ] Values: Active, Inactive, Maintenance, Alarm
  - [ ] Display names

- [ ] **ServiceStatus** (Enum)
  - [ ] Values: Active, Suspended, Expired
  - [ ] Display names

- [ ] **UserStatus** (Enum)
  - [ ] Values: PendingActivation, Active, Inactive
  - [ ] Display names

- [ ] **MachineApiKey**
  - [ ] Secure token generation (GUID-based)
  - [ ] Validation
  - [ ] Equality overrides

---

## 1.3 Domain Events 📢

- [ ] **Base Event:**
  - [ ] `DomainEvent` abstract class
  - [ ] `OccurredOn` property (DateTimeOffset)
  - [ ] `EventId` (Guid)

- [ ] **TicketCreatedEvent**
  - [ ] `TicketId`, `OrganizationId`, `MachineId`, `CreatedBy`

- [ ] **TicketStatusChangedEvent**
  - [ ] `TicketId`, `OldStatus`, `NewStatus`, `Reason`, `ChangedBy`

- [ ] **TicketAssignedEvent**
  - [ ] `TicketId`, `AssignedTo`, `AssignedBy`, `PreviousAssignee`

- [ ] **CommentAddedEvent**
  - [ ] `TicketId`, `CommentId`, `UserId`, `IsInternal`

- [ ] **AttachmentUploadedEvent**
  - [ ] `TicketId`, `AttachmentId`, `FileName`, `UploadedBy`

- [ ] **MachineStatusChangedEvent**
  - [ ] `MachineId`, `OldStatus`, `NewStatus`

---

## 1.4 Repository Interfaces 📚

- [ ] **Base Repository:**
  - [ ] `IRepository<T>` interface w `Domain/Repositories/`
  - [ ] Methods: GetByIdAsync, ListAsync, AddAsync, UpdateAsync, DeleteAsync
  - [ ] Support dla Specifications (Ardalis.Specification)

- [ ] **ITicketRepository**
  - [ ] Extends `IRepository<Ticket>`
  - [ ] `GetByNumberAsync(TicketNumber number)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `GetAssignedToUserAsync(Guid userId, CancellationToken ct)`
  - [ ] `CountByStatusAsync(TicketStatus status, CancellationToken ct)`

- [ ] **IOrganizationRepository**
  - [ ] Extends `IRepository<Organization>`
  - [ ] `GetByNameAsync(string name, CancellationToken ct)`
  - [ ] `GetWithMachinesAsync(Guid id, CancellationToken ct)`

- [ ] **IMachineRepository**
  - [ ] Extends `IRepository<Machine>`
  - [ ] `GetBySerialNumberAsync(string serialNumber, CancellationToken ct)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`
  - [ ] `GetByApiTokenAsync(string token, CancellationToken ct)`
  - [ ] `GetActiveMachinesAsync(CancellationToken ct)`

- [ ] **IUserRepository**
  - [ ] `GetServiceUserAsync(Guid userId, CancellationToken ct)`
  - [ ] `GetOrganizationUserAsync(Guid userId, CancellationToken ct)`
  - [ ] `GetByOrganizationAsync(Guid orgId, CancellationToken ct)`

- [ ] **IUnitOfWork**
  - [ ] `SaveChangesAsync(CancellationToken ct)`
  - [ ] Transaction support
  - [ ] Access do wszystkich repositories

---

## 1.5 Domain Services 🔧

- [ ] **TicketNumberGeneratorService**
  - [ ] `GenerateNextNumberAsync(CancellationToken ct)` 
  - [ ] Format: TICK-{year}-{sequential}
  - [ ] Concurrency handling
  - [ ] Interface: `ITicketNumberGenerator`

- [ ] **TicketAssignmentService**
  - [ ] `CanAssignTo(Ticket ticket, User user)` - business rules
  - [ ] `GetRecommendedAssignee(Ticket ticket)` - round-robin logic (future)
  - [ ] Interface: `ITicketAssignmentService`

- [ ] **MachineMaintenanceService**
  - [ ] `CalculateNextMaintenanceDate(DateOnly lastDate, MaintenanceInterval interval)`
  - [ ] `IsDueSoon(Machine machine, int daysThreshold)`
  - [ ] Interface: `IMachineMaintenanceService`

- [ ] **Domain Validation Rules:**
  - [ ] Status transition validator
  - [ ] User permission validator
  - [ ] Business rule validators

---

## 1.6 Specifications (Ardalis.Specification) 🔍

- [ ] Dodać package: `Ardalis.Specification`

- [ ] **TicketByIdSpec**
  - [ ] Include: Organization, Machine, History, Attachments

- [ ] **TicketsByOrganizationSpec**
  - [ ] Filter by OrganizationId
  - [ ] Optional: filter by Status, Priority
  - [ ] Ordering by CreatedAt DESC

- [ ] **TicketsByStatusSpec**
  - [ ] Filter by Status
  - [ ] Ordering by Priority DESC, CreatedAt DESC

- [ ] **TicketsByAssigneeSpec**
  - [ ] Filter by AssignedToUserId
  - [ ] Include related data

- [ ] **ActiveMachinesSpec**
  - [ ] Filter: Status == Active
  - [ ] Include: Organization
  - [ ] Order by SerialNumber

- [ ] **MachinesByOrganizationSpec**
  - [ ] Filter by OrganizationId
  - [ ] Optional: filter by Status

- [ ] **DueMaintenanceMachinesSpec**
  - [ ] Filter: NextMaintenanceDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(threshold))
  - [ ] Include: Organization

---

## 1.7 Common Domain Types 🧩

- [ ] **Result Pattern** (jeśli nie używamy Ardalis.Result)
  - [ ] `Result` class z Success/Failure
  - [ ] `Result<T>` generic
  - [ ] Error collection

- [ ] **Entity Base Classes:**
  - [ ] `Entity<TId>` abstract class
  - [ ] `IAggregateRoot` marker interface
  - [ ] `IDomainEventContainer` interface
  - [ ] `AuditableEntity` z CreatedAt, UpdatedAt

- [ ] **Domain Exceptions:**
  - [ ] `DomainException` base
  - [ ] `TicketStatusTransitionException`
  - [ ] `InvalidTicketNumberException`
  - [ ] `MachineAlreadyRegisteredException`
  - [ ] `UnauthorizedOperationException`

---

## Postęp Fazy 1

**Ukończone:** 0/85+ (0%)  
**W trakcie:** 0  
**Do zrobienia:** 85+

---

## Notatki

### Decyzje Architektoniczne:
- **Pure Domain**: Domain layer nie ma dependencies (poza może Ardalis.Specification)
- **Rich Domain Model**: Logika biznesowa w entities, nie w services
- **Value Objects**: Immutable, validation w konstruktorze
- **Domain Events**: Raise w aggregate roots, handle w Application layer

### Mapowanie do DB Schema:
- Entities mapują 1:1 do tabel z `db-plan.md`
- TicketHistory przechowuje komentarze jako JSONB events
- Audit log osobna tabela dla pełnego trackingu

### Pytania/Uwagi:
- ❓ Czy `TicketHistory` powinien być osobnym aggregate, czy częścią Ticket?
- ❓ Jak obsługujemy soft delete vs hard delete?
- ❓ Czy potrzebujemy OptimisticConcurrency (rowversion)?

---

**Next Steps:** Po ukończeniu Fazy 1 przejdziemy do Fazy 2 (Application Layer)
