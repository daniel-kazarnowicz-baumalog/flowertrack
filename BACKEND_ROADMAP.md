# 🗺️ Backend Development Roadmap - FLOWerTRACK

## Visual Timeline

```
WEEK 1
├─ Faza 1: Tickets CRUD Operations
│  ├─ Day 1-2: CreateTicket, GetTicket, UpdateTicket
│  ├─ Day 3: DeleteTicket, Tests
│  └─ ✅ Ready for: Faza 2, Faza 3
│
├─ Faza 2: Status Management (parallel)
│  ├─ Day 2-3: UpdateTicketStatus, AssignTicket
│  └─ ✅ Ready for: Faza 4
│
└─ Faza 3: Queries & Filtering (parallel)
   ├─ Day 2-3: GetTickets (list + filters)
   ├─ Day 3: GetTicketsByStatus
   └─ ✅ Ready for: Faza 8

WEEK 2
├─ Faza 4: Timeline & Audit
│  ├─ Day 1-2: TicketAuditLog entity, GetTicketTimeline
│  └─ ✅ Ready for: Front-end timeline
│
├─ Faza 5: Comments & Notes
│  ├─ Day 1-2: TicketComment entity, AddComment, GetComments
│  ├─ Day 2: AddNote
│  └─ ✅ Ready for: Front-end comments
│
└─ Faza 7: Machines Enhancement (quick)
   ├─ Day 3: GetMachines, GetMachine, UpdateMachineStatus
   └─ ✅ Ready for: Front-end machines

WEEK 3
├─ Faza 6: Export & Attachments
│  ├─ Day 1-2: TicketAttachment entity, UploadAttachment
│  ├─ Day 2-3: ExportTicket (PDF/CSV/JSON)
│  └─ ✅ Ready for: Front-end attachments
│
└─ Faza 8: Dashboard & Statistics
   ├─ Day 3-4: GetDashboardStats, GetTicketTrends
   └─ ✅ Ready for: Front-end dashboard

WEEK 4
├─ Faza 9: Authentication & Authorization
│  ├─ Day 1: JWT validation middleware
│  ├─ Day 1-2: Authorization policies
│  └─ ✅ Ready for: Full security
│
└─ Faza 10: Testing & Documentation
   ├─ Day 2-4: Unit tests (80%+ coverage)
   ├─ Day 4-5: Integration tests
   └─ Day 5: API documentation (Swagger)

DEPLOYED TO PRODUCTION ✨
```

---

## Implementation Priority Matrix

```
┌─────────────────────────────────────────────────────────┐
│  HIGH IMPACT + LOW EFFORT (DO FIRST)                   │
│  ├─ Faza 1: Tickets CRUD (foundation)                  │
│  ├─ Faza 2: Status Management (core workflow)          │
│  ├─ Faza 3: Queries & Filtering (essential for UI)     │
│  ├─ Faza 7: Machines Enhancement (quick wins)          │
│  └─ Faza 9: Auth & Authorization (security critical)   │
├─────────────────────────────────────────────────────────┤
│  HIGH IMPACT + HIGH EFFORT (DO SECOND)                 │
│  ├─ Faza 4: Timeline & Audit (complex but valuable)    │
│  ├─ Faza 5: Comments & Notes (user-facing)             │
│  ├─ Faza 8: Dashboard & Statistics (analytics)         │
│  └─ Faza 10: Testing & Documentation (quality gate)    │
├─────────────────────────────────────────────────────────┤
│  MEDIUM IMPACT + MEDIUM EFFORT (DO THIRD)              │
│  └─ Faza 6: Export & Attachments (nice-to-have)        │
└─────────────────────────────────────────────────────────┘
```

---

## Phase Dependencies Graph

```
                          ┌─────────────────────┐
                          │  Domain Layer ✅    │
                          │  Entities Ready     │
                          └──────────┬──────────┘
                                     │
                    ┌────────────────┼────────────────┐
                    │                │                │
            ┌───────▼───────┐  ┌────▼─────┐  ┌──────▼─────┐
            │  Faza 1       │  │ Faza 7   │  │ Faza 9     │
            │  CRUD Ops ✅  │  │ Machines │  │ Auth ✅    │
            └───────┬───────┘  └────┬─────┘  └──────┬─────┘
                    │               │               │
            ┌───────▼──────────┐    │               │
            │  Faza 2          │    │               │
            │  Status Mgmt ←───┴────┘               │
            └───────┬──────────┘                     │
                    │                               │
            ┌───────▼────────────────────────────────┘
            │
      ┌─────▼──────┐      ┌──────────────┐
      │  Faza 3    │      │  Faza 4      │
      │  Queries   │      │  Timeline    │
      └─────┬──────┘      └──────┬───────┘
            │                    │
            │            ┌───────▼────────┐
            │            │  Faza 5        │
            │            │  Comments ←────┘
            │            │
            └─────┬──────┴────────┐
                  │               │
            ┌─────▼──────┐   ┌────▼──────┐
            │  Faza 6    │   │  Faza 8   │
            │  Export ←──┼───┤ Dashboard │
            │ Attachment │   │           │
            └───────────┘    └────┬──────┘
                                  │
                            ┌─────▼──────────┐
                            │  Faza 10       │
                            │  Testing & Doc │
                            └────────────────┘
```

---

## Detailed Task Breakdown

### WEEK 1: Foundation

#### Day 1: Create Ticket Command
**Branch:** `feature/tickets-crud-operations`

**Morning Tasks:**
```
1. [ ] CreateTicketCommand.cs
2. [ ] CreateTicketCommandValidator.cs
3. [ ] CreateTicketCommandHandler.cs
4. [ ] CreateTicketRequest DTO
5. [ ] CreateTicketResponse DTO
```

**Afternoon Tasks:**
```
6. [ ] Unit tests (command, validator, handler)
7. [ ] POST /api/tickets endpoint
8. [ ] Integration test
9. [ ] Commit: "feat(tickets): implement create ticket command"
```

**Definition of Done:**
- [ ] Code compiles without warnings
- [ ] All unit tests pass
- [ ] Integration test passes
- [ ] PR created and reviewed
- [ ] Swagger updated

---

#### Day 2: Get Ticket & Update Ticket
**Continuation:** `feature/tickets-crud-operations`

**Morning Tasks:**
```
1. [ ] GetTicketQuery.cs
2. [ ] GetTicketQueryHandler.cs
3. [ ] TicketDetailDto.cs
4. [ ] GET /api/tickets/{ticketId} endpoint
5. [ ] Unit & Integration tests
```

**Afternoon Tasks:**
```
6. [ ] UpdateTicketCommand.cs & Validator
7. [ ] UpdateTicketCommandHandler.cs
8. [ ] UpdateTicketRequest DTO
9. [ ] PATCH /api/tickets/{ticketId} endpoint
10. [ ] Unit & Integration tests
11. [ ] Commit: "feat(tickets): implement get and update ticket"
```

---

#### Day 3: Delete Ticket & Phase 2 Start
**Tasks:**
```
1. [ ] DeleteTicketCommand.cs & Validator
2. [ ] DeleteTicketCommandHandler.cs
3. [ ] DELETE /api/tickets/{ticketId} endpoint
4. [ ] Unit & Integration tests
5. [ ] Commit: "feat(tickets): implement delete ticket"

6. [ ] UpdateTicketStatusCommand.cs & Validator (status transitions)
7. [ ] UpdateTicketStatusCommandHandler.cs
8. [ ] UpdateTicketStatusRequest DTO
9. [ ] PATCH /api/tickets/{ticketId}/status endpoint
10. [ ] Unit tests (focus on transition validation)
11. [ ] Commit: "feat(tickets): implement status management"
```

---

### WEEK 2: Core Features

#### Day 4-5: Queries & Filtering + Machines
**Tasks:**
```
1. [ ] GetTicketsQuery.cs (with all filters)
2. [ ] GetTicketsQueryHandler.cs (optimized LINQ)
3. [ ] TicketListDto.cs
4. [ ] GET /api/tickets endpoint (with query params)
5. [ ] Unit & Integration tests (multiple filter combinations)
6. [ ] Commit: "feat(tickets): implement list with filtering"

7. [ ] AssignTicketCommand.cs & Validator
8. [ ] AssignTicketCommandHandler.cs
9. [ ] PATCH /api/tickets/{ticketId}/assign endpoint
10. [ ] Unit & Integration tests
11. [ ] Commit: "feat(tickets): implement ticket assignment"

12. [ ] GetMachinesQuery.cs
13. [ ] GetMachineQuery.cs
14. [ ] GET /api/machines endpoints
15. [ ] Unit & Integration tests
16. [ ] Commit: "feat(machines): implement list and get machine"
```

---

#### Day 6-7: Timeline & Comments
**Tasks:**
```
1. [ ] Create TicketAuditLog entity
2. [ ] TicketAuditLogConfiguration.cs
3. [ ] Database migration
4. [ ] GetTicketTimelineQuery.cs
5. [ ] GET /api/tickets/{ticketId}/timeline endpoint
6. [ ] Unit & Integration tests
7. [ ] Commit: "feat(tickets): implement timeline and audit logs"

8. [ ] Create TicketComment entity
9. [ ] TicketCommentConfiguration.cs
10. [ ] Database migration
11. [ ] AddTicketCommentCommand.cs & Validator
12. [ ] AddTicketCommentCommandHandler.cs
13. [ ] GetTicketCommentsQuery.cs
14. [ ] POST/GET /api/tickets/{ticketId}/comments endpoints
15. [ ] Unit & Integration tests
16. [ ] Commit: "feat(tickets): implement comments"

17. [ ] AddTicketNoteCommand.cs (IsInternal = true)
18. [ ] POST /api/tickets/{ticketId}/notes endpoint
19. [ ] Unit & Integration tests
20. [ ] Commit: "feat(tickets): implement internal notes"
```

---

### WEEK 3: Advanced Features

#### Day 8-10: Attachments & Export
**Tasks:**
```
1. [ ] Create TicketAttachment entity
2. [ ] TicketAttachmentConfiguration.cs
3. [ ] Database migration
4. [ ] FileStorageService.cs (S3 or local)
5. [ ] UploadTicketAttachmentCommand.cs & Validator
6. [ ] UploadTicketAttachmentCommandHandler.cs
7. [ ] POST /api/tickets/{ticketId}/attachments endpoint
8. [ ] Unit & Integration tests
9. [ ] Commit: "feat(tickets): implement file attachments"

10. [ ] ExportTicketQuery.cs
11. [ ] ExportTicketQueryHandler.cs (PDF/CSV/JSON generation)
12. [ ] Get PDF/CSV/JSON templates
13. [ ] GET /api/tickets/{ticketId}/export endpoint
14. [ ] Unit & Integration tests
15. [ ] Commit: "feat(tickets): implement export functionality"

16. [ ] GetDashboardStatsQuery.cs
17. [ ] GetTicketTrendsQuery.cs
18. [ ] DashboardController.cs
19. [ ] GET /api/dashboard/stats endpoint
20. [ ] GET /api/dashboard/ticket-trends endpoint
21. [ ] Unit & Integration tests
22. [ ] Commit: "feat(dashboard): implement statistics"
```

---

### WEEK 4: Polish & Quality

#### Day 11-12: Authentication & Authorization
**Tasks:**
```
1. [ ] AuthenticationMiddleware.cs
2. [ ] Update Program.cs for JWT validation
3. [ ] AuthorizationPoliciesExtension.cs
4. [ ] Add [Authorize] attributes to controllers
5. [ ] Add [Authorize(Policy = "...")] to sensitive endpoints
6. [ ] Unit tests
7. [ ] Integration tests
8. [ ] Commit: "feat(auth): implement JWT validation and authorization policies"
```

---

#### Day 13-15: Testing & Documentation
**Tasks:**
```
1. [ ] Add XML documentation to all public methods
2. [ ] Update Swagger configuration
3. [ ] Add examples to DTOs
4. [ ] Write comprehensive unit tests (80%+ coverage target)
5. [ ] Write integration tests for all endpoints
6. [ ] Run code coverage analysis
7. [ ] Fix any coverage gaps
8. [ ] Performance testing (load tests for filtering)
9. [ ] Security testing (SQL injection, XSS in comments)
10. [ ] Documentation: API endpoint guide
11. [ ] Documentation: Database schema
12. [ ] Final review and merge
13. [ ] Commit: "test(all): comprehensive testing and documentation"
```

---

## Git Workflow

### Branch Strategy
```
master (production)
  └─ develop (main development branch)
     ├─ feature/tickets-crud-operations (Week 1)
     ├─ feature/tickets-advanced (Week 2)
     ├─ feature/attachments-export (Week 3)
     ├─ feature/dashboard (Week 3)
     └─ feature/auth-security (Week 4)
```

### Commit Message Convention
```
Format: <type>(<scope>): <subject>

Types:
  - feat: A new feature
  - fix: A bug fix
  - docs: Documentation only changes
  - test: Adding or updating tests
  - refactor: Code change that neither fixes a bug nor adds a feature
  - perf: Code change that improves performance
  - chore: Changes to build process, dependencies, etc.

Scope:
  - tickets, machines, dashboard, auth, etc.

Subject:
  - Use imperative mood (add not added)
  - Don't capitalize first letter
  - No period (.) at the end
  - Limit to 50 characters

Examples:
  feat(tickets): implement create ticket command
  fix(tickets): fix status transition validation
  test(tickets): add unit tests for create ticket
  docs(api): update swagger documentation
```

---

## Pull Request Template

```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Documentation update

## Related Issues
Fixes #(issue number)

## Testing Done
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code compiles without warnings
- [ ] All tests pass
- [ ] Code follows style guidelines
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No breaking changes (or documented)

## Screenshots (if applicable)
Add screenshots of UI changes or API responses

## Performance Impact
- [ ] No impact
- [ ] Minimal impact
- [ ] Needs optimization (describe)

## Security Review
- [ ] No security concerns
- [ ] Security implications reviewed
```

---

## Definition of Done

For each task/user story:

```
IMPLEMENTATION
✓ Code written following project standards
✓ Code compiles without errors or warnings
✓ Follows architecture patterns (CQRS, DDD)
✓ Follows naming conventions

TESTING
✓ Unit tests written (min 80% coverage)
✓ Integration tests written
✓ All tests passing locally
✓ Edge cases covered
✓ Error scenarios tested

DOCUMENTATION
✓ Code comments added (where needed)
✓ XML documentation on public APIs
✓ Swagger/OpenAPI updated
✓ Database schema documented (if changed)
✓ README updated (if needed)

CODE REVIEW
✓ PR created with clear description
✓ Code reviewed by team member
✓ Comments addressed
✓ CI/CD pipeline passes

QUALITY GATES
✓ No SonarQube issues
✓ Test coverage > 75%
✓ No performance regressions
✓ Security scan passed
```

---

## Success Metrics

### By End of Week 1:
- ✅ Tickets CRUD operations complete
- ✅ Basic status management working
- ✅ 50+ unit tests passing
- ✅ All endpoints documented

### By End of Week 2:
- ✅ All queries and filters working
- ✅ Timeline and audit logs functional
- ✅ Comments and notes system working
- ✅ Machines queries implemented
- ✅ 100+ unit tests passing

### By End of Week 3:
- ✅ Export functionality complete
- ✅ Dashboard statistics ready
- ✅ File attachments working
- ✅ Integration tests for all endpoints

### By End of Week 4:
- ✅ Full authentication and authorization
- ✅ 80%+ code coverage
- ✅ Complete API documentation
- ✅ Ready for frontend integration

---

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Database migrations fail | High | Test migrations in staging first, have rollback plans |
| Performance issues with large datasets | Medium | Add indexes, optimize queries, load test early |
| Authentication issues | High | Test with real Supabase tokens, have fallback |
| Status transition logic bugs | Medium | Comprehensive unit tests, visual state machine testing |
| File storage failures | Medium | Use cloud storage with failover, test error handling |
| API versioning issues | Low | Plan for v2 from start, maintain backward compatibility |

---

## Communication

### Daily Standup Topics:
1. What did you complete yesterday?
2. What are you working on today?
3. Any blockers?
4. Any dependencies on other tasks?

### Weekly Sync:
- Review progress against timeline
- Discuss any blockers
- Adjust estimates if needed
- Plan next week tasks

---

**Version:** 1.0  
**Created:** 09.11.2025  
**Last Updated:** 09.11.2025  
**Next Review:** End of Week 1
