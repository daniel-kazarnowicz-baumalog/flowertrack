# ✅ First Sprint Checklist - WEEK 1

## Sprint Goal: Foundation for Ticket Management System

**Sprint Duration:** Week 1 (09-13 November 2025)  
**Team:** Backend Developers  
**Deliverables:** Tickets CRUD + Status Management APIs  

---

## 📦 DELIVERABLES

### D1: Create Ticket Functionality (US-015 partial)
- [ ] **Backend Command**
  - [ ] CreateTicketCommand.cs
  - [ ] CreateTicketCommandValidator.cs
  - [ ] CreateTicketCommandValidator test
  - [ ] CreateTicketCommandHandler.cs
  - [ ] CreateTicketCommandHandler test

- [ ] **API Layer**
  - [ ] CreateTicketRequest DTO
  - [ ] CreateTicketResponse DTO
  - [ ] POST /api/tickets endpoint
  - [ ] TicketsController created

- [ ] **Tests**
  - [ ] Unit test: CreateTicketCommandHandler
  - [ ] Unit test: CreateTicketCommandValidator
  - [ ] Integration test: POST /api/tickets
  - [ ] Tests pass locally ✓
  - [ ] Code coverage > 80%

- [ ] **Documentation**
  - [ ] XML comments on CreateTicketCommand
  - [ ] XML comments on DTOs
  - [ ] Swagger endpoint documented
  - [ ] Example request/response in Swagger

- [ ] **Quality Checklist**
  - [ ] Code compiles without warnings
  - [ ] Follows project naming conventions
  - [ ] Follows CQRS pattern
  - [ ] No hardcoded values
  - [ ] Proper error handling
  - [ ] PR description complete

**Estimate:** 1 day  
**Status:** 🔴 NOT STARTED

---

### D2: Get Ticket & Update Ticket (US-015 partial)
- [ ] **Get Ticket Query**
  - [ ] GetTicketQuery.cs
  - [ ] GetTicketQueryHandler.cs
  - [ ] TicketDetailDto.cs
  - [ ] GetTicketQuery test
  - [ ] GetTicketQueryHandler test

- [ ] **Get Ticket API**
  - [ ] GET /api/tickets/{ticketId} endpoint
  - [ ] Integration test
  - [ ] Swagger documented

- [ ] **Update Ticket Command**
  - [ ] UpdateTicketCommand.cs
  - [ ] UpdateTicketCommandValidator.cs
  - [ ] UpdateTicketCommandValidator test
  - [ ] UpdateTicketCommandHandler.cs
  - [ ] UpdateTicketCommandHandler test

- [ ] **Update Ticket API**
  - [ ] UpdateTicketRequest DTO
  - [ ] PATCH /api/tickets/{ticketId} endpoint
  - [ ] Integration test
  - [ ] Swagger documented

- [ ] **Tests**
  - [ ] All unit tests pass
  - [ ] All integration tests pass
  - [ ] Code coverage > 80%

- [ ] **Documentation**
  - [ ] XML comments added
  - [ ] Swagger updated

**Estimate:** 1 day  
**Status:** 🔴 NOT STARTED

---

### D3: Delete Ticket (US-015 partial)
- [ ] **Backend Command**
  - [ ] DeleteTicketCommand.cs
  - [ ] DeleteTicketCommandValidator.cs
  - [ ] DeleteTicketCommandValidator test
  - [ ] DeleteTicketCommandHandler.cs (soft delete)
  - [ ] DeleteTicketCommandHandler test

- [ ] **API Layer**
  - [ ] DELETE /api/tickets/{ticketId} endpoint
  - [ ] Integration test
  - [ ] Swagger documented

- [ ] **Quality**
  - [ ] All tests pass
  - [ ] Code coverage > 80%
  - [ ] Soft delete working correctly
  - [ ] Audit trail recorded

**Estimate:** 0.5 day  
**Status:** 🔴 NOT STARTED

---

### D4: Update Ticket Status (US-017)
- [ ] **Backend Command**
  - [ ] UpdateTicketStatusCommand.cs
  - [ ] UpdateTicketStatusCommandValidator.cs
    - [ ] Validate all status transitions
    - [ ] Validate "Reason" field requirements
    - [ ] Test: New → Accepted ✓
    - [ ] Test: New → Closed ✓
    - [ ] Test: Accepted → InProgress ✓
    - [ ] Test: Accepted → Closed ✓
    - [ ] Test: InProgress → Resolved ✓
    - [ ] Test: Resolved → Reopened ✓ (within 14 days)
    - [ ] Test: Resolved → Closed ✓
    - [ ] Test: Closed → (blocked) ✓
  - [ ] UpdateTicketStatusCommandHandler.cs
    - [ ] Call Ticket domain method
    - [ ] Raise TicketStatusChangedEvent
    - [ ] Set UpdatedAt/UpdatedBy
    - [ ] Create audit log entry

- [ ] **API Layer**
  - [ ] UpdateTicketStatusRequest DTO
  - [ ] PATCH /api/tickets/{ticketId}/status endpoint
  - [ ] Integration tests (all transitions)
  - [ ] Swagger documented

- [ ] **Tests**
  - [ ] Unit tests for ALL valid transitions
  - [ ] Unit tests for all INVALID transitions
  - [ ] Unit tests for authorization
  - [ ] Integration tests for all scenarios
  - [ ] Error scenarios (404, 403, 400)

- [ ] **Quality**
  - [ ] Code coverage > 80%
  - [ ] Event raised correctly
  - [ ] Audit trail recorded

**Estimate:** 1.5 days  
**Status:** 🔴 NOT STARTED

---

### D5: Assign Ticket (US-019)
- [ ] **Backend Command**
  - [ ] AssignTicketCommand.cs
  - [ ] AssignTicketCommandValidator.cs
    - [ ] Validate ticket exists
    - [ ] Validate target user is service member
    - [ ] Validate target user is active
    - [ ] Validate permission
  - [ ] AssignTicketCommandHandler.cs
    - [ ] Update Ticket.AssignedTo
    - [ ] Raise TicketAssignedEvent
    - [ ] Create audit entry

- [ ] **API Layer**
  - [ ] AssignTicketRequest DTO
  - [ ] PATCH /api/tickets/{ticketId}/assign endpoint
  - [ ] Integration test
  - [ ] Swagger documented

- [ ] **Tests**
  - [ ] Unit tests
  - [ ] Integration tests
  - [ ] Authorization test
  - [ ] Error scenarios

**Estimate:** 1 day  
**Status:** 🔴 NOT STARTED

---

### D6: Get Tickets List with Filtering (US-010, US-011, US-012)
- [ ] **Backend Query**
  - [ ] GetTicketsQuery.cs
    - [ ] Pagination: pageNumber, pageSize
    - [ ] SearchTerm: title, description, ticket number
    - [ ] Status filter (multiple)
    - [ ] Priority filter (multiple)
    - [ ] OrganizationId filter
    - [ ] MachineId filter
    - [ ] AssignedTo filter
    - [ ] Date range filter (CreatedFrom, CreatedTo)
    - [ ] SortBy: CreatedAt, UpdatedAt, Status, Priority
    - [ ] SortOrder: Asc, Desc
  - [ ] GetTicketsQueryHandler.cs (optimized LINQ)
  - [ ] TicketListDto.cs
  - [ ] GetTicketsQuery test

- [ ] **API Layer**
  - [ ] Query parameter model/binding
  - [ ] GET /api/tickets endpoint
  - [ ] Integration test (multiple filter combinations)
  - [ ] Swagger documented with all parameters

- [ ] **Tests**
  - [ ] Test each filter individually
  - [ ] Test filter combinations
  - [ ] Test pagination
  - [ ] Test sorting (all fields)
  - [ ] Test authorization
  - [ ] Performance test (large dataset)

- [ ] **Performance**
  - [ ] Use projection (select specific fields)
  - [ ] Add indexes if needed
  - [ ] Test with 1000+ records
  - [ ] Query execution time < 500ms

**Estimate:** 2 days  
**Status:** 🔴 NOT STARTED

---

### D7: Get Tickets Grouped by Status (US-013)
- [ ] **Backend Query**
  - [ ] GetTicketsGroupedByStatusQuery.cs
  - [ ] GetTicketsGroupedByStatusQueryHandler.cs
  - [ ] TicketsGroupDto.cs

- [ ] **API Layer**
  - [ ] GET /api/tickets/grouped-by-status endpoint
  - [ ] Integration test
  - [ ] Swagger documented

- [ ] **Tests**
  - [ ] Unit tests
  - [ ] Integration tests
  - [ ] Grouping validation

**Estimate:** 0.5 day  
**Status:** 🔴 NOT STARTED

---

## 🧪 TESTING REQUIREMENTS

### Unit Test Coverage
```
Target: 80%+ coverage for all new code

Required:
✓ Command handlers (all scenarios)
✓ Validators (valid + invalid inputs)
✓ Query handlers
✓ Domain methods
✓ Value objects

Tools:
- xUnit for tests
- Moq for mocking
- Fluent Assertions
```

### Integration Test Coverage
```
For each API endpoint:
✓ Happy path (200/201)
✓ Validation errors (400)
✓ Authorization errors (403)
✓ Not found errors (404)
✓ Business rule violations (409)
✓ Database transaction integrity

Tools:
- WebApplicationFactory
- In-memory database or TestContainers
```

### Performance Testing
```
Query Performance:
✓ GetTickets with filters < 500ms (1000 records)
✓ GetTicket < 100ms
✓ List pagination works correctly

Memory:
✓ No memory leaks
✓ Proper disposal of resources
```

---

## 📋 DATABASE REQUIREMENTS

### Migrations
- [ ] No new migrations needed (Ticket entity already exists)
- [ ] Verify TicketRepository queries work
- [ ] Test database schema integrity

### Indexes
- [ ] Index on Ticket.OrganizationId
- [ ] Index on Ticket.MachineId
- [ ] Index on Ticket.AssignedTo
- [ ] Index on Ticket.Status
- [ ] Composite index on (OrganizationId, Status, CreatedAt)

---

## 🏗️ CODE STRUCTURE

### File Structure to Create
```
src/backend/Core/Flowertrack.Application/
├── Tickets/
│   ├── Commands/
│   │   ├── CreateTicket/
│   │   │   ├── CreateTicketCommand.cs
│   │   │   ├── CreateTicketCommandValidator.cs
│   │   │   └── CreateTicketCommandHandler.cs
│   │   ├── UpdateTicket/
│   │   │   ├── UpdateTicketCommand.cs
│   │   │   ├── UpdateTicketCommandValidator.cs
│   │   │   └── UpdateTicketCommandHandler.cs
│   │   ├── DeleteTicket/
│   │   │   ├── DeleteTicketCommand.cs
│   │   │   ├── DeleteTicketCommandValidator.cs
│   │   │   └── DeleteTicketCommandHandler.cs
│   │   ├── UpdateTicketStatus/
│   │   │   ├── UpdateTicketStatusCommand.cs
│   │   │   ├── UpdateTicketStatusCommandValidator.cs
│   │   │   └── UpdateTicketStatusCommandHandler.cs
│   │   └── AssignTicket/
│   │       ├── AssignTicketCommand.cs
│   │       ├── AssignTicketCommandValidator.cs
│   │       └── AssignTicketCommandHandler.cs
│   └── Queries/
│       ├── GetTicket/
│       │   ├── GetTicketQuery.cs
│       │   ├── GetTicketQueryHandler.cs
│       │   └── TicketDetailDto.cs
│       ├── GetTickets/
│       │   ├── GetTicketsQuery.cs
│       │   ├── GetTicketsQueryHandler.cs
│       │   ├── TicketListDto.cs
│       │   └── TicketFilterDto.cs
│       └── GetTicketsGroupedByStatus/
│           ├── GetTicketsGroupedByStatusQuery.cs
│           ├── GetTicketsGroupedByStatusQueryHandler.cs
│           └── TicketsGroupDto.cs

src/backend/Presentation/Flowertrack.Contracts/
├── Tickets/
│   ├── Requests/
│   │   ├── CreateTicketRequest.cs
│   │   ├── UpdateTicketRequest.cs
│   │   ├── UpdateTicketStatusRequest.cs
│   │   └── AssignTicketRequest.cs
│   └── Responses/
│       ├── TicketResponse.cs
│       ├── CreateTicketResponse.cs
│       ├── TicketListItemDto.cs
│       └── TimelineEventDto.cs

src/backend/Presentation/Flowertrack.Api/
├── Controllers/
│   └── TicketsController.cs

Tests/Flowertrack.Application.Tests/
├── Tickets/
│   ├── Commands/
│   │   ├── CreateTicketCommandHandlerTests.cs
│   │   ├── UpdateTicketCommandHandlerTests.cs
│   │   ├── UpdateTicketStatusCommandHandlerTests.cs
│   │   └── AssignTicketCommandHandlerTests.cs
│   └── Queries/
│       ├── GetTicketsQueryHandlerTests.cs
│       └── GetTicketQueryHandlerTests.cs

Tests/Flowertrack.Api.IntegrationTests/
├── Tickets/
│   ├── CreateTicketIntegrationTests.cs
│   ├── GetTicketsIntegrationTests.cs
│   ├── UpdateTicketStatusIntegrationTests.cs
│   └── AssignTicketIntegrationTests.cs
```

---

## 📝 COMMIT STRATEGY

### Commit 1 (EOD Day 1)
```
feat(tickets): implement create ticket command

- Add CreateTicketCommand, CreateTicketCommandValidator
- Add CreateTicketCommandHandler with business logic
- Add CreateTicketRequest/Response DTOs
- Add POST /api/tickets endpoint
- Add unit tests (command, validator, handler)
- Add integration test for endpoint
- Update Swagger documentation
```

### Commit 2 (EOD Day 1)
```
feat(tickets): implement get ticket query

- Add GetTicketQuery, GetTicketQueryHandler
- Add TicketDetailDto with all ticket details
- Add GET /api/tickets/{ticketId} endpoint
- Add unit and integration tests
- Update Swagger documentation
```

### Commit 3 (EOD Day 1)
```
feat(tickets): implement update ticket command

- Add UpdateTicketCommand, UpdateTicketCommandValidator
- Add UpdateTicketCommandHandler
- Add UpdateTicketRequest DTO
- Add PATCH /api/tickets/{ticketId} endpoint
- Add unit and integration tests
- Update Swagger documentation
```

### Commit 4 (EOD Day 2)
```
feat(tickets): implement delete ticket command

- Add DeleteTicketCommand with soft delete
- Add DeleteTicketCommandValidator
- Add DeleteTicketCommandHandler
- Add DELETE /api/tickets/{ticketId} endpoint
- Add unit and integration tests
- Update Swagger documentation
```

### Commit 5 (EOD Day 2)
```
feat(tickets): implement ticket status management

- Add UpdateTicketStatusCommand with transition validation
- Add UpdateTicketStatusCommandValidator (all transitions)
- Add UpdateTicketStatusCommandHandler with event raising
- Add PATCH /api/tickets/{ticketId}/status endpoint
- Add comprehensive unit tests for all transitions
- Add integration tests
- Update Swagger documentation
```

### Commit 6 (EOD Day 3)
```
feat(tickets): implement ticket assignment

- Add AssignTicketCommand, AssignTicketCommandValidator
- Add AssignTicketCommandHandler
- Add PATCH /api/tickets/{ticketId}/assign endpoint
- Add unit and integration tests
- Update Swagger documentation
```

### Commit 7 (EOD Day 4-5)
```
feat(tickets): implement advanced queries and filtering

- Add GetTicketsQuery with comprehensive filtering
- Add GetTicketsQueryHandler (optimized LINQ)
- Add TicketListDto and filter models
- Add GET /api/tickets endpoint with query parameters
- Add GetTicketsGroupedByStatusQuery
- Add GET /api/tickets/grouped-by-status endpoint
- Add extensive unit tests (filtering combinations)
- Add integration tests (pagination, sorting, filters)
- Add performance tests
- Update Swagger documentation with examples
```

---

## ✅ DEFINITION OF DONE

For the entire sprint to be marked complete:

### Code Quality
- [ ] All code compiles without errors/warnings
- [ ] All code follows naming conventions
- [ ] No code duplication (DRY principle)
- [ ] SOLID principles applied
- [ ] CQRS pattern properly implemented

### Testing
- [ ] Unit test coverage > 80%
- [ ] Integration test coverage 100% of endpoints
- [ ] All tests passing locally
- [ ] Edge cases tested
- [ ] Error scenarios tested
- [ ] Performance requirements met

### Documentation
- [ ] XML documentation on all public APIs
- [ ] Swagger/OpenAPI fully updated
- [ ] Example requests/responses provided
- [ ] Database schema documented
- [ ] README updated (if needed)

### Deliverables
- [ ] All 7 deliverables completed
- [ ] No outstanding TODOs or FIXMEs
- [ ] Database migrations applied
- [ ] Indexes created
- [ ] Feature branch ready for merge

### PR Review
- [ ] PR created with clear description
- [ ] PR links to user stories
- [ ] CI/CD pipeline passes
- [ ] Code reviewed (min 1 approval)
- [ ] All comments resolved
- [ ] Ready to merge to develop

---

## 🎯 SUCCESS METRICS

### By End of Sprint:
1. ✅ **Functionality**: All 7 deliverables 100% complete
2. ✅ **Quality**: 80%+ test coverage
3. ✅ **Performance**: Queries execute < 500ms
4. ✅ **Documentation**: Full Swagger coverage
5. ✅ **Code Review**: All PRs merged to develop
6. ✅ **Ready for**: Frontend integration testing

### Metrics Dashboard:
| Metric | Target | Status |
|--------|--------|--------|
| Commits | 7 | 🔴 0/7 |
| Tests Written | 30+ | 🔴 0/30 |
| Test Coverage | 80%+ | 🔴 0% |
| Code Review Comments | < 5 | - |
| Build Time | < 2min | - |
| Critical Bugs | 0 | - |

---

## 🚨 RISKS & MITIGATION

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Status transition logic bugs | Medium | High | Comprehensive unit tests, state machine diagram |
| Performance issues with filters | Low | Medium | Load testing, index optimization |
| Database migration issues | Low | High | Test in staging, have rollback plan |
| Authorization bugs | Low | High | Test all role scenarios |

---

## 📞 COMMUNICATION

### Daily
- [ ] 10:00 AM - Team standup (15 min)
  - What completed yesterday?
  - What today?
  - Any blockers?

### Weekly
- [ ] Friday EOD - Sprint review + retrospective
  - Demo deliverables
  - Review metrics
  - Discuss improvements

---

**Sprint Created:** 09.11.2025  
**Sprint Start:** 09.11.2025 (Day 1 Monday)  
**Sprint End:** 13.11.2025 (Day 5 Friday)  
**Total Estimate:** 8-9 person days  
**Team:** 1 developer (adjustable)

---

## Quick Reference

### Start Here (Day 1 Morning)
1. Create branch: `git checkout -b feature/tickets-crud-operations develop`
2. Create folder structure (see above)
3. Start with CreateTicketCommand
4. Follow commit strategy (one feature = one commit)
5. Run tests after each commit
6. Push to remote for backup

### Key Files to Reference
- `/src/backend/Core/Flowertrack.Domain/Entities/Ticket.cs` - Domain model
- `/src/backend/Core/Flowertrack.Application/Organizations/Commands/` - Reference implementation
- `/src/backend/Presentation/Flowertrack.Api/Controllers/OrganizationsController.cs` - Reference controller

### Common Tasks
```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter "FileName=CreateTicketCommandHandlerTests"

# Build project
dotnet build

# Format code
dotnet format

# Check code quality
dotnet test --collect:"XPlat Code Coverage"
```

---

**Version:** 1.0  
**Last Updated:** 09.11.2025
