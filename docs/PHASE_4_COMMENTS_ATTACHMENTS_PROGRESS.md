# Phase 4 Progress Summary: Comments & Attachments Module

**Date:** 2025-11-09  
**Status:** Partially Complete (Domain Layer + Infrastructure Complete)  
**Branch:** feature/issue-41-backend-mvp-implementation  

## Overview

Phase 4 implements the Comments and Attachments functionality for the FLOWerTRACK ticket system. This enables both service technicians and organization users to add comments and file attachments to tickets, with proper access control and security policies.

## Completed Work

### ✅ Phase 3B: Authorization Policy Enhancement (Commit: 767fa3c)

**Changes:**
- Modified `TicketsController.cs` to use granular authorization policies
- Controller-level: `[Authorize(Policy = "RequireAuthenticatedUser")]`
- `UpdateTicketStatus` endpoint: `[Authorize(Policy = "RequireServiceUser")]`
- `AssignTicket` endpoint: `[Authorize(Policy = "RequireServiceUser")]`
- Added XML remarks documenting authorization requirements

**Result:** Enhanced security with policy-based authorization matching RLS implementation.

---

### ✅ Phase 4.1-4.3: Domain Entities & Events (Commit: f1c089a)

**Created Files:**
- `TicketComment.cs` - Comment entity with full business logic
- `TicketAttachment.cs` - Attachment entity with file validation
- `TicketCommentAddedEvent.cs`
- `TicketCommentUpdatedEvent.cs`
- `TicketCommentDeletedEvent.cs`
- `TicketAttachmentUploadedEvent.cs`
- `TicketAttachmentDeletedEvent.cs`

**TicketComment Entity Features:**
- **Properties:** Id, TicketId, UserId, Content (max 5000 chars), IsInternal
- **Methods:** 
  - `Create()` - Factory method for new comments
  - `Update(content, userId)` - Author-only updates with validation
  - `Delete(userId)` - Soft delete by author
- **Validation:**
  - Non-empty content required
  - Maximum 5000 characters
  - Author verification for updates
  - IsInternal flag for service-only comments

**TicketAttachment Entity Features:**
- **Properties:** Id, TicketId, UploadedBy, FileName (max 255), StoragePath, ContentType, FileSizeBytes
- **Methods:**
  - `Create()` - Factory method for new attachments
  - `Delete(userId)` - Soft delete by uploader
- **Validation:**
  - Non-empty filename (max 255 characters)
  - Valid storage path required
  - File size must be > 0 and <= 50 MB (52,428,800 bytes)
  - MIME type defaults to "application/octet-stream"

**Domain Events:**
All events extend `DomainEvent` base class with:
- `EventId` (auto-generated GUID)
- `OccurredOn` (UTC timestamp)
- `AggregateId` (TicketId for audit trail)

---

### ✅ Phase 4.4-4.6: EF Core Configuration & Migration (Commit: a179256)

**Created Files:**
- `TicketCommentConfiguration.cs`
- `TicketAttachmentConfiguration.cs`
- `20251109185504_AddTicketCommentsAndAttachments.cs` (EF migration)

**Database Schema:**

**TicketComments Table:**
```sql
CREATE TABLE "TicketComments" (
    "Id" UUID PRIMARY KEY,
    "TicketId" UUID NOT NULL,
    "UserId" UUID NOT NULL,
    "Content" VARCHAR(5000) NOT NULL,
    "IsInternal" BOOLEAN NOT NULL DEFAULT false,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "CreatedBy" UUID NOT NULL,
    "UpdatedAt" TIMESTAMPTZ NULL,
    "UpdatedBy" UUID NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT false,
    "DeletedAt" TIMESTAMPTZ NULL,
    "DeletedBy" UUID NULL,
    CONSTRAINT "FK_TicketComments_Tickets" 
        FOREIGN KEY ("TicketId") REFERENCES "Tickets"("Id") ON DELETE RESTRICT
);
```

**Indexes:**
- `IX_TicketComments_TicketId`
- `IX_TicketComments_UserId`
- `IX_TicketComments_TicketId_IsDeleted` (composite)
- `IX_TicketComments_TicketId_IsInternal_IsDeleted` (composite)

**TicketAttachments Table:**
```sql
CREATE TABLE "TicketAttachments" (
    "Id" UUID PRIMARY KEY,
    "TicketId" UUID NOT NULL,
    "UploadedBy" UUID NOT NULL,
    "FileName" VARCHAR(255) NOT NULL,
    "StoragePath" VARCHAR(1000) NOT NULL,
    "ContentType" VARCHAR(100) NOT NULL DEFAULT 'application/octet-stream',
    "FileSizeBytes" BIGINT NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "CreatedBy" UUID NOT NULL,
    "UpdatedAt" TIMESTAMPTZ NULL,
    "UpdatedBy" UUID NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT false,
    "DeletedAt" TIMESTAMPTZ NULL,
    "DeletedBy" UUID NULL,
    CONSTRAINT "FK_TicketAttachments_Tickets" 
        FOREIGN KEY ("TicketId") REFERENCES "Tickets"("Id") ON DELETE RESTRICT
);
```

**Indexes:**
- `IX_TicketAttachments_TicketId`
- `IX_TicketAttachments_UploadedBy`
- `IX_TicketAttachments_StoragePath`
- `IX_TicketAttachments_TicketId_IsDeleted` (composite)

**DbContext Updates:**
- Added `DbSet<TicketComment> TicketComments` to `ApplicationDbContext`
- Added `DbSet<TicketAttachment> TicketAttachments` to `ApplicationDbContext`
- Updated `IApplicationDbContext` interface with same properties
- EF configurations automatically discovered via `ApplyConfigurationsFromAssembly()`

---

### ✅ Phase 4.7: Row Level Security Policies (Commit: e7cf5c0)

**Updated File:** `20251109_rls_comments_attachments.sql`

**TicketComments Policies (6 total):**

1. **ServiceUsers_SelectAll_TicketComments**
   - Service users can view ALL comments (internal + public)

2. **OrgUsers_SelectAccessible_TicketComments**
   - Org users can view public comments on accessible tickets
   - Internal comments (`IsInternal = true`) hidden from org users

3. **ServiceUsers_InsertAny_TicketComments**
   - Service users can add comments to any ticket
   - Can mark comments as internal

4. **OrgUsers_InsertAccessible_TicketComments**
   - Org users can add public comments to accessible tickets
   - Cannot create internal comments (`IsInternal` must be false)

5. **Users_UpdateOwn_TicketComments**
   - Authors can update their own comments
   - Time limit enforcement delegated to application layer

6. **Users_DeleteOwn_TicketComments**
   - Authors can soft delete their own comments
   - Sets `IsDeleted = true`

**TicketAttachments Policies (6 total):**

7. **ServiceUsers_SelectAll_TicketAttachments**
   - Service users can view all attachments

8. **OrgUsers_SelectAccessible_TicketAttachments**
   - Org users can view attachments on accessible tickets

9. **ServiceUsers_InsertAny_TicketAttachments**
   - Service users can upload to any ticket

10. **OrgUsers_InsertAccessible_TicketAttachments**
    - Org users can upload to accessible tickets
    - Must match `UploadedBy = auth.uid()`

11. **Users_DeleteOwn_TicketAttachments**
    - Uploaders can soft delete their own attachments

12. **ServiceAdmins_DeleteAny_TicketAttachments**
    - Service admins can soft delete any attachment

**Access Control Summary:**

| Action | Service User | Org User |
|--------|-------------|----------|
| View Comments | All (internal + public) | Public only, accessible tickets |
| Add Comments | Any ticket, can mark internal | Accessible tickets, public only |
| Edit Comments | Own comments | Own comments |
| Delete Comments | Own comments | Own comments |
| View Attachments | All | Accessible tickets |
| Upload Attachments | Any ticket | Accessible tickets |
| Delete Attachments | Own + admin delete | Own only |

**Security Features:**
- Leverages `user_has_ticket_access()` helper function
- Internal comments visible only to service users
- Soft delete protection (no hard deletes via RLS)
- Storage cleanup delegated to application layer
- Authenticated-only access

---

## Remaining Work (Application Layer)

### 🔄 Phase 4.8: Create Commands for Comments

**Files to Create:**
- `AddCommentCommand.cs` + Handler
- `UpdateCommentCommand.cs` + Handler
- `DeleteCommentCommand.cs` + Handler
- Validators for each command

**Business Logic:**
- Validate `UserId` matches authenticated user
- Enforce 15-minute time limit for comment edits
- Validate ticket exists and user has access
- Enforce `IsInternal` flag permissions (service users only)

---

### 🔄 Phase 4.9: Create Queries for Comments

**Files to Create:**
- `GetCommentsForTicketQuery.cs` + Handler
- Pagination support (PageNumber, PageSize)
- Filtering by `IsInternal` based on user role

**Returns:**
- List of comments with author information
- Separate internal and public comments for service users
- Only public comments for organization users

---

### 🔄 Phase 4.10: Create DTOs (Contracts)

**Files to Create:**
- `CommentDto.cs` - Response DTO with author details
- `AddCommentRequest.cs` - Request for POST /comments
- `UpdateCommentRequest.cs` - Request for PUT /comments/{id}
- `GetCommentsResponse.cs` - Paginated list response

**DTO Structure:**
```csharp
public record CommentDto(
    Guid Id,
    Guid TicketId,
    Guid UserId,
    string AuthorName,  // Joined from Users table
    string Content,
    bool IsInternal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    bool CanEdit,  // Calculated based on time limit
    bool CanDelete  // Calculated based on ownership
);
```

---

### 🔄 Phase 4.11: Create API Endpoints

**Endpoints to Implement:**

```csharp
// TicketsController or new TicketCommentsController

[HttpPost("{ticketId}/comments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<CommentDto>> AddComment(Guid ticketId, AddCommentRequest request);

[HttpGet("{ticketId}/comments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<GetCommentsResponse>> GetComments(Guid ticketId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20);

[HttpPut("comments/{commentId}")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<CommentDto>> UpdateComment(Guid commentId, UpdateCommentRequest request);

[HttpDelete("comments/{commentId}")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult> DeleteComment(Guid commentId);
```

**Authorization:**
- All endpoints require authentication
- No additional policy restrictions (RLS handles access control)
- Application layer validates edit time limits

---

### 🔄 Phase 4.12: Implement Attachments Functionality

**Supabase Storage Integration:**
- Create `IStorageService` interface
- Implement `SupabaseStorageService`
- Handle file uploads to Supabase Storage buckets
- Generate signed URLs for file downloads
- Implement cleanup for deleted attachments

**Commands:**
- `UploadAttachmentCommand` + Handler
- `DeleteAttachmentCommand` + Handler
- `GetAttachmentDownloadUrlQuery` + Handler

**Endpoints:**
```csharp
[HttpPost("{ticketId}/attachments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<AttachmentDto>> UploadAttachment(Guid ticketId, IFormFile file);

[HttpGet("{ticketId}/attachments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<List<AttachmentDto>>> GetAttachments(Guid ticketId);

[HttpGet("attachments/{attachmentId}/download")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult<string>> GetDownloadUrl(Guid attachmentId);

[HttpDelete("attachments/{attachmentId}")]
[Authorize(Policy = "RequireAuthenticatedUser")]
Task<ActionResult> DeleteAttachment(Guid attachmentId);
```

**File Validation:**
- Max file size: 50 MB
- Allowed types: Images (jpg, png, gif), PDFs, Office docs, text files
- Generate unique storage paths: `tickets/{ticketId}/{guid}-{filename}`
- Store metadata in `TicketAttachments` table

---

## Build & Migration Status

**✅ Build Status:** All projects compile successfully
- 0 errors
- 2 pre-existing nullable reference warnings (unrelated to Phase 4)

**✅ Migration Status:** EF migration created and ready
- Migration: `20251109185504_AddTicketCommentsAndAttachments`
- Not yet applied to database (pending Supabase setup)

**✅ Git Commits:**
1. `767fa3c` - Phase 3B: Authorization policies for Tickets endpoints
2. `f1c089a` - Phase 4.1-4.3: Domain entities and events
3. `a179256` - Phase 4.4-4.6: EF Core configurations and migration
4. `e7cf5c0` - Phase 4.7: RLS policies for Comments & Attachments

---

## Next Steps

1. **Implement Commands (Phase 4.8)**
   - Start with `AddCommentCommand` as it's the most critical
   - Add validators for content length and permissions
   - Test with RLS policies enabled

2. **Implement Queries (Phase 4.9)**
   - `GetCommentsForTicketQuery` with pagination
   - Filter internal comments based on user role
   - Include author information via joins

3. **Create DTOs (Phase 4.10)**
   - Define contract models in `Flowertrack.Contracts`
   - Include computed properties (CanEdit, CanDelete)
   - Add proper XML documentation

4. **Create API Endpoints (Phase 4.11)**
   - Add to `TicketsController` or create new `CommentsController`
   - Apply authorization policies
   - Test with Postman/Swagger

5. **Implement Attachments (Phase 4.12)**
   - Set up Supabase Storage service
   - Create upload/download commands
   - Add file validation and size limits
   - Implement storage cleanup for deletions

---

## Testing Checklist (For Application Layer)

**Comments:**
- [ ] Service user can create internal comments
- [ ] Org user cannot create internal comments
- [ ] Org user cannot see internal comments
- [ ] Service user can see all comments
- [ ] Author can edit own comment within time limit
- [ ] Non-author cannot edit comment
- [ ] Author can delete own comment
- [ ] Edit time limit properly enforced (15 minutes)

**Attachments:**
- [ ] File upload succeeds with valid file (<50MB)
- [ ] File upload rejected if >50MB
- [ ] Storage path properly generated
- [ ] Download URL requires authentication
- [ ] User can delete own attachment
- [ ] Service admin can delete any attachment
- [ ] Storage cleanup executed on delete

**RLS Integration:**
- [ ] RLS policies applied after migration
- [ ] PostgreSQL policies match application logic
- [ ] Verify with `SELECT * FROM pg_policies WHERE tablename IN ('TicketComments', 'TicketAttachments')`

---

## Architecture Decisions

1. **Soft Deletes:** Both comments and attachments use `IsDeleted` flag
   - Preserves audit trail
   - RLS policies hide deleted records
   - Application layer handles storage cleanup for attachments

2. **Internal Comments:** Implemented via `IsInternal` boolean flag
   - Service users can create and view
   - Organization users cannot create or view
   - Enforced at both RLS and application layer

3. **Edit Time Limit:** Enforced at application layer (not database)
   - More flexible than database triggers
   - Can be adjusted via configuration
   - Suggested default: 15 minutes

4. **Author-Only Updates:** Enforced by RLS policies
   - `UserId = auth.uid()` check in USING clause
   - Application layer validates as secondary check
   - Service admins have no special update privileges (by design)

5. **Storage Integration:** Supabase Storage for file attachments
   - Separate from PostgreSQL database
   - Signed URLs for secure downloads
   - Application handles cleanup on delete

---

## Documentation

All RLS policies documented with:
- Access control matrices for each role
- Verification queries for testing
- Policy summaries explaining security guarantees
- Application layer responsibilities clearly defined

Phase 4 foundation is **solid and ready** for application layer implementation.
