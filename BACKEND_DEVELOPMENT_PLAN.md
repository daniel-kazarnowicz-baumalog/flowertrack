# 🛠️ Plan Rozwoju Backendu - FLOWerTRACK

## 📋 Spis Treści
1. [Faza 1: Tickets CRUD Operations](#faza-1-tickets-crud-operations)
2. [Faza 2: Tickets Advanced Features](#faza-2-tickets-advanced-features)
3. [Faza 3: Queries & Filtering](#faza-3-queries--filtering)
4. [Faza 4: Timeline & Audit](#faza-4-timeline--audit)
5. [Faza 5: Comments & Notes](#faza-5-comments--notes)
6. [Faza 6: Machines Enhancement](#faza-6-machines-enhancement)
7. [Faza 7: Dashboard & Statistics](#faza-7-dashboard--statistics)
8. [Faza 8: Authentication & Authorization](#faza-8-authentication--authorization)
9. [Faza 9: Testing & Documentation](#faza-9-testing--documentation)

---

# FAZA 1: Tickets CRUD Operations
**Estymacja:** 2-3 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Ticket Entity, TicketRepository  

## 1.1 Create Ticket (US-015 - częściowo)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/CreateTicket/
├── CreateTicketCommand.cs
├── CreateTicketCommandValidator.cs
└── CreateTicketCommandHandler.cs
```

**Szczegóły:**
- **Inputs:**
  - `OrganizationId` (required)
  - `MachineId` (required)
  - `Title` (required, min 10, max 200 chars)
  - `Description` (required, min 20, max 5000 chars)
  - `Priority` (required: Low, Medium, High, Critical)
  - `CreatedBy` (from claims)

- **Validations:**
  - Organization exists
  - Machine exists and belongs to organization
  - Title/Description not empty and length constraints
  - Priority is valid enum
  - User has permission to create ticket

- **Business Logic:**
  - Ticket created in "New" status
  - Auto-generate TicketNumber (TICK-2025-00001 format)
  - Set CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
  - Raise `TicketCreatedEvent`
  - Commit to database

- **Returns:**
  - `CreateTicketResponse` with TicketId, TicketNumber

#### 2. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
├── CreateTicketRequest.cs

src/backend/Presentation/Flowertrack.Contracts/Tickets/Responses/
├── TicketResponse.cs
├── CreateTicketResponse.cs
```

#### 3. API Controller
```
src/backend/Presentation/Flowertrack.Api/Controllers/
└── TicketsController.cs (add endpoint)

Endpoint: POST /api/tickets
```

### Implementation Checklist
- [ ] CreateTicketCommand & Validator
- [ ] CreateTicketCommandHandler
- [ ] CreateTicketRequest DTO
- [ ] TicketResponse DTO (comprehensive)
- [ ] CreateTicketResponse DTO
- [ ] POST endpoint in TicketsController
- [ ] Unit tests (command, validator, handler)
- [ ] Integration tests (API endpoint)

---

## 1.2 Get Ticket Details (US-015)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/GetTicket/
├── GetTicketQuery.cs
├── GetTicketQueryHandler.cs
└── TicketDetailDto.cs
```

**Details:**
- **Input:**
  - `TicketId` (required)
  - `UserId` (from claims, for authorization)

- **Returns:**
  - Complete ticket details:
    - Basic info (ID, Number, Title, Description)
    - Status, Priority
    - Organization & Machine info
    - Assignment info (AssignedTo, AssignedAt)
    - Timestamps (CreatedAt, UpdatedAt)
    - Participant list (who has access)

- **Authorization:**
  - Check if user has access to this ticket
  - User from same organization OR service team member

#### 2. API Controller
```
Endpoint: GET /api/tickets/{ticketId}
Response: 200 OK with TicketDetailDto
Error: 404 if not found, 403 if forbidden
```

### Implementation Checklist
- [ ] GetTicketQuery
- [ ] GetTicketQueryHandler
- [ ] TicketDetailDto
- [ ] GET endpoint in TicketsController
- [ ] Unit tests
- [ ] Integration tests

---

## 1.3 Update Ticket (Partial - Basic Fields)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/UpdateTicket/
├── UpdateTicketCommand.cs
├── UpdateTicketCommandValidator.cs
└── UpdateTicketCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `Title` (optional)
  - `Description` (optional)
  - `Priority` (optional)

- **Validations:**
  - Ticket exists
  - User has permission (owner or service team)
  - Length constraints on Title/Description
  - Cannot update if ticket is "Closed" (except by admin)

- **Business Logic:**
  - Update fields if provided
  - Set UpdatedAt, UpdatedBy
  - No event raised (use dedicated commands for status changes, assignment, etc.)

#### 2. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
└── UpdateTicketRequest.cs
```

#### 3. API Controller
```
Endpoint: PATCH /api/tickets/{ticketId}
```

### Implementation Checklist
- [ ] UpdateTicketCommand & Validator
- [ ] UpdateTicketCommandHandler
- [ ] UpdateTicketRequest DTO
- [ ] PATCH endpoint
- [ ] Unit tests
- [ ] Integration tests

---

## 1.4 Delete Ticket (Soft Delete)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/DeleteTicket/
├── DeleteTicketCommand.cs
├── DeleteTicketCommandValidator.cs
└── DeleteTicketCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `Reason` (optional, for audit)

- **Business Logic:**
  - Only owner or admin can delete
  - Only if ticket is "New" or "Closed"
  - Soft delete (IsDeleted = true)
  - No events raised
  - Audit trail logged

- **Error Handling:**
  - 403 if user doesn't have permission
  - 409 if ticket in invalid state

#### 2. API Controller
```
Endpoint: DELETE /api/tickets/{ticketId}
Response: 204 No Content
```

### Implementation Checklist
- [ ] DeleteTicketCommand & Validator
- [ ] DeleteTicketCommandHandler
- [ ] DELETE endpoint
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 2: Tickets Status Management
**Estymacja:** 2 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 1  

## 2.1 Update Ticket Status (US-017)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/UpdateTicketStatus/
├── UpdateTicketStatusCommand.cs
├── UpdateTicketStatusCommandValidator.cs
└── UpdateTicketStatusCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `NewStatus` (required: Accepted, InProgress, Resolved, Reopened, Closed)
  - `Reason` (required for Resolved/Closed status changes, for audit)

- **Validations:**
  - Ticket exists
  - Current status allows transition to new status
  - Only service team members can change status (for most cases)
  - Client (organization user) can only create/reopen tickets

- **Status Transition Rules:**
  ```
  New → Accepted OR Closed (rejection)
  Accepted → InProgress OR Closed
  InProgress → Resolved OR InProgress (no-op)
  Resolved → Reopened (within 14 days) OR Closed
  Reopened → InProgress OR Closed
  Closed → (no transitions except admin override)
  ```

- **Business Logic:**
  - Validate transition is allowed
  - Update status on Ticket entity using domain method
  - Raise `TicketStatusChangedEvent` with old/new status and reason
  - Set UpdatedAt, UpdatedBy
  - Create audit entry

#### 2. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
└── UpdateTicketStatusRequest.cs
```

#### 3. API Controller
```
Endpoint: PATCH /api/tickets/{ticketId}/status
```

### Implementation Checklist
- [ ] UpdateTicketStatusCommand & Validator (with transition rules)
- [ ] UpdateTicketStatusCommandHandler
- [ ] UpdateTicketStatusRequest DTO
- [ ] PATCH endpoint for status
- [ ] Unit tests (especially transition rules)
- [ ] Integration tests

---

## 2.2 Assign Ticket (US-019)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/AssignTicket/
├── AssignTicketCommand.cs
├── AssignTicketCommandValidator.cs
└── AssignTicketCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `AssignToUserId` (required)
  - `AssignedBy` (from claims)

- **Validations:**
  - Ticket exists
  - Target user is service team member
  - Target user is available (Active)
  - Only service admin or ticket owner can assign

- **Business Logic:**
  - Update Ticket.AssignedTo and AssignedAt
  - Raise `TicketAssignedEvent`
  - Create audit entry
  - (Optional) Send notification to assigned user

#### 2. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
└── AssignTicketRequest.cs
```

#### 3. API Controller
```
Endpoint: PATCH /api/tickets/{ticketId}/assign
```

### Implementation Checklist
- [ ] AssignTicketCommand & Validator
- [ ] AssignTicketCommandHandler
- [ ] AssignTicketRequest DTO
- [ ] PATCH endpoint for assignment
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 3: Tickets Queries & Filtering
**Estymacja:** 2-3 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 1  

## 3.1 Get Tickets List with Filtering (US-010, US-011, US-012)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/GetTickets/
├── GetTicketsQuery.cs
├── GetTicketsQueryHandler.cs
├── TicketListDto.cs
└── TicketFilterDto.cs
```

**Details:**
- **Inputs:**
  - `PageNumber` (optional, default 1)
  - `PageSize` (optional, default 10, max 100)
  - `SearchTerm` (optional, search in title, description, ticket number)
  - `Status` (optional, multiple allowed)
  - `Priority` (optional, multiple allowed)
  - `OrganizationId` (optional, required for service team)
  - `MachineId` (optional)
  - `AssignedTo` (optional)
  - `CreatedFrom` / `CreatedTo` (optional)
  - `SortBy` (optional: CreatedAt, UpdatedAt, Status, Priority)
  - `SortOrder` (optional: Asc, Desc)

- **Authorization:**
  - Service team member: Can see all tickets in their organization(s)
  - Organization user: Can only see tickets from their organization
  - Admin: Can see all tickets

- **Returns:**
  - `PaginatedResponse<TicketListDto>`
  - Each item includes: TicketNumber, Title, Status, Priority, Organization, Machine, AssignedTo, CreatedAt, UpdatedAt

- **Performance:**
  - Include Organization, Machine, AssignedTo via eager loading
  - Use pagination
  - Optimize queries with projections

#### 2. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Queries/
├── GetTicketsRequest.cs (query parameters)
└── TicketListItemDto.cs
```

#### 3. API Controller
```
Endpoint: GET /api/tickets
Query Parameters: page, pageSize, searchTerm, status, priority, organizationId, machineId, assignedTo, createdFrom, createdTo, sortBy, sortOrder
Response: 200 with PaginatedResponse<TicketListItemDto>
```

### Implementation Checklist
- [ ] GetTicketsQuery with all filters
- [ ] GetTicketsQueryHandler (optimized LINQ)
- [ ] TicketListDto & TicketListItemDto
- [ ] Query parameter model
- [ ] GET endpoint with query string binding
- [ ] Unit tests (filtering logic)
- [ ] Integration tests (with various filter combinations)
- [ ] Performance tests (large datasets)

---

## 3.2 Get Tickets Grouped by Status (US-013)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/GetTicketsByStatus/
├── GetTicketsGroupedByStatusQuery.cs
├── GetTicketsGroupedByStatusQueryHandler.cs
└── TicketsGroupDto.cs
```

**Details:**
- **Returns:**
  - Grouped response:
    ```csharp
    {
      "New": { "count": 5, "tickets": [...] },
      "Accepted": { "count": 3, "tickets": [...] },
      "InProgress": { "count": 8, "tickets": [...] },
      "Resolved": { "count": 2, "tickets": [...] },
      "Closed": { "count": 12, "tickets": [...] }
    }
    ```

#### 2. API Controller
```
Endpoint: GET /api/tickets/grouped-by-status
Query Parameters: (same as GetTickets)
```

### Implementation Checklist
- [ ] GetTicketsGroupedByStatusQuery
- [ ] GetTicketsGroupedByStatusQueryHandler
- [ ] TicketsGroupDto
- [ ] GET endpoint
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 4: Timeline & Audit
**Estymacja:** 2 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 2  

## 4.1 Ticket Timeline/History (US-016)

### Pliki do Stworzenia

#### 1. Domain Layer - New Entity
```
src/backend/Core/Flowertrack.Domain/Entities/Tickets/
└── TicketAuditLog.cs
```

**Details:**
```csharp
public class TicketAuditLog : AuditableEntity<Guid>
{
    public Guid TicketId { get; set; }
    public string Action { get; set; } // Created, StatusChanged, Assigned, Commented, NoteAdded, etc.
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
    public DateTime ActionAt { get; set; }
    public string ChangedBy { get; set; }
    
    // Navigation
    public Ticket? Ticket { get; set; }
}
```

#### 2. Infrastructure Layer
```
src/backend/Infrastructure/Flowertrack.Infrastructure/Persistence/Configurations/
└── TicketAuditLogConfiguration.cs

src/backend/Infrastructure/Flowertrack.Infrastructure/Persistence/Repositories/
└── (Update TicketRepository with timeline query)
```

#### 3. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/GetTicketTimeline/
├── GetTicketTimelineQuery.cs
├── GetTicketTimelineQueryHandler.cs
└── TimelineEventDto.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)

- **Returns:**
  - List of timeline events ordered by date DESC:
    ```
    [
      {
        "eventId": "guid",
        "action": "StatusChanged",
        "oldValue": "New",
        "newValue": "Accepted",
        "reason": "Ticket accepted",
        "changedBy": "John Doe",
        "changedAt": "2025-11-09T10:30:00Z"
      },
      ...
    ]
    ```

#### 4. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Queries/
└── TimelineEventDto.cs
```

#### 5. API Controller
```
Endpoint: GET /api/tickets/{ticketId}/timeline
Response: 200 with TimelineEventDto[]
```

### Migration Strategy
- Create new TicketAuditLog table
- Populate existing ticket changes (manual or seeded)
- Update handlers to log all changes

### Implementation Checklist
- [ ] TicketAuditLog entity
- [ ] TicketAuditLogConfiguration
- [ ] Migration file
- [ ] GetTicketTimelineQuery
- [ ] GetTicketTimelineQueryHandler
- [ ] TimelineEventDto
- [ ] GET endpoint for timeline
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 5: Comments & Notes
**Estymacja:** 2-3 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 1  

## 5.1 Add Comment to Ticket (US-020, US-045)

### Pliki do Stworzenia

#### 1. Domain Layer - New Entity
```
src/backend/Core/Flowertrack.Domain/Entities/Tickets/
└── TicketComment.cs
```

**Details:**
```csharp
public class TicketComment : AuditableEntity<Guid>
{
    public Guid TicketId { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public bool IsInternal { get; set; } // false = visible to client, true = internal only
    public DateTime CommentedAt { get; set; }
    
    // Navigation
    public Ticket? Ticket { get; set; }
    public ServiceUser? Author { get; set; }
}
```

#### 2. Infrastructure Layer
```
src/backend/Infrastructure/Flowertrack.Infrastructure/Persistence/Configurations/
└── TicketCommentConfiguration.cs
```

#### 3. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/AddTicketComment/
├── AddTicketCommentCommand.cs
├── AddTicketCommentCommandValidator.cs
└── AddTicketCommentCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `Content` (required, min 1, max 5000 chars)
  - `IsInternal` (optional, default false)
  - `AuthorId` (from claims)

- **Validations:**
  - Ticket exists
  - User has permission to comment
  - Content not empty
  - Ticket not Closed (unless admin)

- **Business Logic:**
  - Create TicketComment
  - Raise `TicketCommentedEvent`
  - (Optional) Send notification

#### 4. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/GetTicketComments/
├── GetTicketCommentsQuery.cs
├── GetTicketCommentsQueryHandler.cs
└── TicketCommentDto.cs
```

#### 5. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
└── AddTicketCommentRequest.cs

src/backend/Presentation/Flowertrack.Contracts/Tickets/Queries/
└── TicketCommentDto.cs
```

#### 6. API Controller
```
Endpoints:
  POST /api/tickets/{ticketId}/comments
  GET /api/tickets/{ticketId}/comments
```

### Implementation Checklist
- [ ] TicketComment entity
- [ ] TicketCommentConfiguration
- [ ] Migration
- [ ] AddTicketCommentCommand & Validator
- [ ] AddTicketCommentCommandHandler
- [ ] GetTicketCommentsQuery
- [ ] GetTicketCommentsQueryHandler
- [ ] DTOs
- [ ] POST & GET endpoints
- [ ] Unit tests
- [ ] Integration tests

---

## 5.2 Add Internal Note to Ticket (US-018)

### Pliki do Stworzenia

Extends TicketComment from 5.1:
- Notes are just comments with `IsInternal = true`
- Same structure, different endpoint

#### Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/AddTicketNote/
├── AddTicketNoteCommand.cs
├── AddTicketNoteCommandValidator.cs
└── AddTicketNoteCommandHandler.cs
```

**Details:**
- Force `IsInternal = true`
- Only service team members can add notes
- Raise `TicketNoteAddedEvent`

#### API Controller
```
Endpoint: POST /api/tickets/{ticketId}/notes (convenience wrapper)
```

### Implementation Checklist
- [ ] AddTicketNoteCommand & Validator
- [ ] AddTicketNoteCommandHandler
- [ ] POST endpoint
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 6: Export & Attachments
**Estymacja:** 3-4 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 5  

## 6.1 Ticket Attachments (US-046)

### Pliki do Stworzenia

#### 1. Domain Layer - New Entity
```
src/backend/Core/Flowertrack.Domain/Entities/Tickets/
└── TicketAttachment.cs
```

**Details:**
```csharp
public class TicketAttachment : AuditableEntity<Guid>
{
    public Guid TicketId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; } // S3 or local storage
    public long FileSizeBytes { get; set; }
    public string FileType { get; set; } // MIME type
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
    
    // Navigation
    public Ticket? Ticket { get; set; }
}
```

#### 2. Infrastructure Layer
```
src/backend/Infrastructure/Flowertrack.Infrastructure/Persistence/Configurations/
└── TicketAttachmentConfiguration.cs

src/backend/Infrastructure/Flowertrack.Infrastructure/Services/
└── FileStorageService.cs (S3 or local)
```

#### 3. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Tickets/Commands/UploadTicketAttachment/
├── UploadTicketAttachmentCommand.cs
├── UploadTicketAttachmentCommandValidator.cs
└── UploadTicketAttachmentCommandHandler.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `File` (IFormFile, max 10MB per spec)
  - `UploadedBy` (from claims)

- **Validations:**
  - File size < 10MB
  - Allowed file types (images, PDFs, docs)
  - Max 3 files per ticket (check spec)
  - Ticket exists

#### 4. Contracts Layer
```
src/backend/Presentation/Flowertrack.Contracts/Tickets/Requests/
└── UploadAttachmentRequest.cs (multipart form-data)

src/backend/Presentation/Flowertrack.Contracts/Tickets/Responses/
└── AttachmentDto.cs
```

#### 5. API Controller
```
Endpoint: POST /api/tickets/{ticketId}/attachments
Content-Type: multipart/form-data
Response: 201 Created with AttachmentDto
```

### Implementation Checklist
- [ ] TicketAttachment entity
- [ ] TicketAttachmentConfiguration
- [ ] Migration
- [ ] FileStorageService (S3 or local storage)
- [ ] UploadTicketAttachmentCommand & Validator
- [ ] UploadTicketAttachmentCommandHandler
- [ ] DTOs
- [ ] POST endpoint
- [ ] Unit tests
- [ ] Integration tests

---

## 6.2 Export Ticket as PDF/CSV/JSON (US-022)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Tickets/Queries/ExportTicket/
├── ExportTicketQuery.cs
├── ExportTicketQueryHandler.cs
├── ExportFormat.cs (enum)
└── ExportedTicketDto.cs
```

**Details:**
- **Inputs:**
  - `TicketId` (required)
  - `Format` (required: PDF, CSV, JSON)

- **Returns:**
  - Byte array with file content
  - Appropriate content-type

#### 2. External Library Integration
- **PDF:** iTextSharp or PdfSharpCore
- **CSV:** CsvHelper NuGet package
- **JSON:** System.Text.Json

#### 3. API Controller
```
Endpoint: GET /api/tickets/{ticketId}/export?format=pdf|csv|json
Response: 200 with file attachment (Content-Disposition: attachment)
```

### Implementation Checklist
- [ ] ExportTicketQuery
- [ ] ExportTicketQueryHandler (PDF/CSV/JSON generation)
- [ ] PDF/CSV templates
- [ ] ExportFormat enum
- [ ] GET endpoint with format parameter
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 7: Machines Enhancement
**Estymacja:** 1-2 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Machines Entity exists  

## 7.1 Get Machines List (US-028)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Machines/Queries/GetMachines/
├── GetMachinesQuery.cs
├── GetMachinesQueryHandler.cs
└── MachineListDto.cs
```

**Details:**
- **Inputs:**
  - `PageNumber` (optional)
  - `PageSize` (optional)
  - `SearchTerm` (optional, search in serial, brand, model)
  - `Status` (optional: Active, Inactive, Alarm, Maintenance)
  - `OrganizationId` (optional)
  - `SortBy` (optional)

- **Authorization:**
  - Service team: Can see all machines
  - Organization user: Can see only their org machines

#### 2. API Controller
```
Endpoint: GET /api/machines
```

### Implementation Checklist
- [ ] GetMachinesQuery
- [ ] GetMachinesQueryHandler
- [ ] MachineListDto
- [ ] GET endpoint
- [ ] Unit tests
- [ ] Integration tests

---

## 7.2 Get Machine Details (US-029)

### Pliki do Stworzenia

#### 1. Application Layer - Query
```
src/backend/Core/Flowertrack.Application/Machines/Queries/GetMachine/
├── GetMachineQuery.cs
├── GetMachineQueryHandler.cs
└── MachineDetailDto.cs
```

**Details:**
- **Returns:**
  - Full machine details
  - Last maintenance date
  - Next maintenance date
  - Recent alarms
  - Associated tickets (count)
  - Status history

#### 2. API Controller
```
Endpoint: GET /api/machines/{machineId}
```

### Implementation Checklist
- [ ] GetMachineQuery
- [ ] GetMachineQueryHandler
- [ ] MachineDetailDto
- [ ] GET endpoint
- [ ] Unit tests
- [ ] Integration tests

---

## 7.3 Update Machine Status (from Domain Logic)

### Pliki do Stworzenia

#### 1. Application Layer - Command
```
src/backend/Core/Flowertrack.Application/Machines/Commands/UpdateMachineStatus/
├── UpdateMachineStatusCommand.cs
├── UpdateMachineStatusCommandValidator.cs
└── UpdateMachineStatusCommandHandler.cs
```

**Details:**
- Update machine status (Active, Inactive, Alarm, Maintenance)
- Raise event
- Audit trail

#### 2. API Controller
```
Endpoint: PATCH /api/machines/{machineId}/status
```

### Implementation Checklist
- [ ] UpdateMachineStatusCommand & Validator
- [ ] UpdateMachineStatusCommandHandler
- [ ] PATCH endpoint
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 8: Dashboard & Statistics
**Estymacja:** 3-4 dni  
**Status:** 🔴 NOT STARTED  
**Zależności:** ✅ Faza 3 (queries)  

## 8.1 Dashboard Stats (US-001 to US-009)

### Pliki do Stworzenia

#### 1. Application Layer - Queries
```
src/backend/Core/Flowertrack.Application/Dashboard/Queries/
├── GetDashboardStatsQuery.cs
├── GetDashboardStatsQueryHandler.cs
├── DashboardStatsDto.cs
├── GetTicketTrendsQuery.cs
├── GetTicketTrendsQueryHandler.cs
└── TicketTrendsDto.cs
```

**Dashboard Stats includes:**
- Total tickets count
- Tickets by status (breakdown)
- Tickets by priority (breakdown)
- Active machines count
- Machines by status
- Overdue tickets
- High priority tickets
- Recent activities (last 24h)

**Ticket Trends includes:**
- Line chart: Open, Resolved, Closed trends (last 7/30/90 days)
- Pie chart: Distribution by priority
- Average resolution time
- Average time to close

#### 2. API Controller
```
src/backend/Presentation/Flowertrack.Api/Controllers/
└── DashboardController.cs

Endpoints:
  GET /api/dashboard/stats
  GET /api/dashboard/ticket-trends?period=7d|30d|90d
```

### Implementation Checklist
- [ ] GetDashboardStatsQuery
- [ ] GetDashboardStatsQueryHandler
- [ ] GetTicketTrendsQuery
- [ ] GetTicketTrendsQueryHandler
- [ ] DTOs
- [ ] DashboardController
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 9: Authentication & Authorization
**Estymacja:** 2-3 dni  
**Status:** 🟡 PARTIAL (Supabase configured, needs middleware)  
**Zależności:** ✅ Supabase client configured  

## 9.1 JWT Token Validation Middleware

### Pliki do Modyfikacji/Stworzenia

#### 1. Create Authentication Middleware
```
src/backend/Presentation/Flowertrack.Api/Middleware/
└── AuthenticationMiddleware.cs
```

**Details:**
- Extract JWT from Authorization header
- Validate token with Supabase
- Extract user claims
- Set HttpContext.User

#### 2. Update Program.cs
```
Program.cs
- Add authentication services
- Add middleware
- Configure JWT options
```

### Implementation Checklist
- [ ] AuthenticationMiddleware
- [ ] Update Program.cs
- [ ] Configure Supabase JWT validation
- [ ] Add [Authorize] attributes to controllers
- [ ] Unit tests
- [ ] Integration tests

---

## 9.2 Authorization Policies

### Pliki do Stworzenia

#### 1. Authorization Policies
```
src/backend/Presentation/Flowertrack.Api/Extensions/
└── AuthorizationPoliciesExtension.cs
```

**Policies to implement:**
- `IsServiceTeam` - User is in service organization
- `IsServiceAdmin` - User is service admin
- `IsOrganizationUser` - User belongs to client organization
- `IsOrganizationAdmin` - User is client organization admin
- `CanAccessTicket` - User can access specific ticket (ownership/team)

#### 2. Apply Policies
```
Controllers should use:
[Authorize(Policy = "IsServiceTeam")]
[Authorize(Policy = "CanAccessTicket")]
```

### Implementation Checklist
- [ ] AuthorizationPoliciesExtension
- [ ] Policy definitions
- [ ] Apply policies to endpoints
- [ ] Unit tests
- [ ] Integration tests

---

# FAZA 10: Testing & Documentation
**Estymacja:** 3-5 dni  
**Status:** 🔴 NOT STARTED  

## 10.1 Unit Tests

### Coverage Target: 80%+

**Test Projects:**
```
Flowertrack.Application.Tests/
├── Tickets/
│   ├── Commands/
│   │   ├── CreateTicketCommandHandlerTests.cs
│   │   ├── UpdateTicketStatusCommandHandlerTests.cs
│   │   ├── AssignTicketCommandHandlerTests.cs
│   │   └── ...
│   └── Queries/
│       ├── GetTicketsQueryHandlerTests.cs
│       ├── GetTicketTimelineQueryHandlerTests.cs
│       └── ...
└── Machines/
    ├── Commands/
    └── Queries/
```

### Testing Strategy
- Test business logic (commands/queries)
- Test validators
- Test domain entity methods
- Mock repositories and services
- Use Moq for mocking
- Use Fluent Assertions for assertions

### Implementation Checklist
- [ ] Unit tests for all Commands (validators, handlers)
- [ ] Unit tests for all Queries (handlers)
- [ ] Unit tests for Validators
- [ ] Reach 80%+ code coverage

---

## 10.2 Integration Tests

### Test Projects:
```
Flowertrack.Api.IntegrationTests/
├── Tickets/
│   ├── CreateTicketIntegrationTests.cs
│   ├── UpdateTicketStatusIntegrationTests.cs
│   ├── GetTicketsIntegrationTests.cs
│   └── ...
└── Machines/
    └── ...
```

### Testing Strategy
- Use WebApplicationFactory for API testing
- Test full request/response cycle
- Test database transactions
- Test error scenarios (404, 403, 400)
- Use in-memory database or Testcontainers

### Implementation Checklist
- [ ] Integration tests for each API endpoint
- [ ] Test happy path scenarios
- [ ] Test error scenarios (validation, authorization, not found)
- [ ] Test pagination and filtering
- [ ] Test sorting

---

## 10.3 API Documentation

### Swagger/OpenAPI

#### 1. Configure Swagger in Program.cs
```csharp
// Already configured, needs enhancement
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FLOWerTRACK API",
        Version = "v1",
        Description = "Service ticket management system API"
    });
    
    // Add JWT authentication scheme
    // Add XML comments
});
```

#### 2. Add XML Documentation Comments
- All public methods should have /// comments
- Controllers should have summary
- Endpoints should have description
- Parameters should be documented

### Implementation Checklist
- [ ] Complete Swagger configuration
- [ ] Add XML documentation to all endpoints
- [ ] Add examples to DTOs
- [ ] Configure authentication scheme in Swagger
- [ ] Document error responses (400, 404, 403, 500)

---

# Summary Table

| Faza | Tytuł | Estymacja | Zależności | Status |
|------|-------|-----------|-----------|--------|
| 1 | Tickets CRUD | 2-3 dni | ✅ | 🔴 |
| 2 | Status Management | 2 dni | ✅ Faza 1 | 🔴 |
| 3 | Queries & Filtering | 2-3 dni | ✅ Faza 1 | 🔴 |
| 4 | Timeline & Audit | 2 dni | ✅ Faza 2 | 🔴 |
| 5 | Comments & Notes | 2-3 dni | ✅ Faza 1 | 🔴 |
| 6 | Export & Attachments | 3-4 dni | ✅ Faza 5 | 🔴 |
| 7 | Machines Enhancement | 1-2 dni | ✅ | 🔴 |
| 8 | Dashboard & Stats | 3-4 dni | ✅ Faza 3 | 🔴 |
| 9 | Auth & Authorization | 2-3 dni | ✅ | 🟡 |
| 10 | Testing & Docs | 3-5 dni | ✅ All | 🔴 |

**Total Estimation:** 22-30 dni (~3-4 tygodnie pracy)

---

# Starting Point

## Next Immediate Steps (Today)

1. **Create branch:** `feature/tickets-crud-operations`
2. **Start with Faza 1.1:**
   - [ ] CreateTicketCommand & Validator
   - [ ] CreateTicketCommandHandler
   - [ ] DTOs (CreateTicketRequest, TicketResponse)
   - [ ] POST endpoint in TicketsController

3. **Write tests as you go:**
   - Unit tests for command/validator
   - Integration test for API endpoint

4. **Commit strategy:**
   - One commit per feature
   - Clear commit messages following convention

---

**Plan Created:** 09.11.2025  
**Version:** 1.0  
**Author:** Development Team
