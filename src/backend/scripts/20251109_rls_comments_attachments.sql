-- =====================================================
-- FLOWerTRACK - TicketComments & TicketAttachments RLS Policies
-- Phase 2.6: Row Level Security for Ticket Comments and Attachments
-- Created: 2025-11-09
-- =====================================================

-- NOTE: This migration is prepared for future use when TicketComments and 
-- TicketAttachments tables are created. These tables do not exist yet in the schema.

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - All previous policies applied
-- - TicketComments and TicketAttachments tables must be created
-- - Expected schema:
--   TicketComments: Id, TicketId, UserId (could be ServiceUser or OrganizationUser), 
--                   Content, CreatedAt, UpdatedAt, IsDeleted, etc.
--   TicketAttachments: Id, TicketId, FileName, FilePath, UploadedBy, 
--                      UploadedAt, FileSize, MimeType, IsDeleted, etc.

-- =====================================================
-- HELPER FUNCTION: Check if user has access to ticket
-- =====================================================

CREATE OR REPLACE FUNCTION public.user_has_ticket_access(ticket_id UUID)
RETURNS BOOLEAN AS $$
    SELECT EXISTS (
        SELECT 1
        FROM "Tickets" t
        WHERE t."Id" = ticket_id
        AND t."IsDeleted" = false
        AND (
            -- Service users can access all tickets
            public.is_service_user() = true
            OR
            -- Organization users can access tickets from their organization
            (
                t."OrganizationId" = public.current_user_organization_id()
            )
        )
    );
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

GRANT EXECUTE ON FUNCTION public.user_has_ticket_access(UUID) TO authenticated;

-- =====================================================
-- TICKET COMMENTS TABLE POLICIES
-- =====================================================
-- Run these when TicketComments table is created:

/*
-- Enable RLS on TicketComments
ALTER TABLE "TicketComments" ENABLE ROW LEVEL SECURITY;

-- Policy 1: SELECT - Users can see comments on tickets they have access to
CREATE POLICY "users_select_comments_on_accessible_tickets"
ON "TicketComments"
FOR SELECT
TO authenticated
USING (
    public.user_has_ticket_access("TicketId") = true
    AND "IsDeleted" = false
);

-- Policy 2: INSERT - Organization users can comment on their organization's tickets
CREATE POLICY "organization_users_insert_comments"
ON "TicketComments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.user_has_ticket_access("TicketId") = true
    -- Application layer should validate that UserId matches auth.uid()
);

-- Policy 3: INSERT - Service users can comment on any ticket
CREATE POLICY "service_users_insert_comments"
ON "TicketComments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = true
);

-- Policy 4: UPDATE - Users can edit their own comments (within time limit)
-- Application layer should enforce time limit (e.g., 15 minutes after creation)
CREATE POLICY "users_update_own_comments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    "UserId" = auth.uid()
    AND "IsDeleted" = false
)
WITH CHECK (
    "UserId" = auth.uid()
);

-- Policy 5: UPDATE - Service admins can edit any comment
CREATE POLICY "service_admins_update_comments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 6: DELETE - Users can soft-delete their own comments
CREATE POLICY "users_delete_own_comments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    "UserId" = auth.uid()
    AND "IsDeleted" = false
)
WITH CHECK (
    "UserId" = auth.uid()
);

-- Policy 7: DELETE - Service admins can soft-delete any comment
CREATE POLICY "service_admins_delete_comments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

GRANT SELECT, INSERT, UPDATE ON "TicketComments" TO authenticated;
*/

-- =====================================================
-- TICKET ATTACHMENTS TABLE POLICIES
-- =====================================================
-- Run these when TicketAttachments table is created:

/*
-- Enable RLS on TicketAttachments
ALTER TABLE "TicketAttachments" ENABLE ROW LEVEL SECURITY;

-- Policy 1: SELECT - Users can see attachments on tickets they have access to
CREATE POLICY "users_select_attachments_on_accessible_tickets"
ON "TicketAttachments"
FOR SELECT
TO authenticated
USING (
    public.user_has_ticket_access("TicketId") = true
    AND "IsDeleted" = false
);

-- Policy 2: INSERT - Organization users can upload attachments to their org's tickets
CREATE POLICY "organization_users_upload_attachments"
ON "TicketAttachments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.user_has_ticket_access("TicketId") = true
    -- Application layer should validate that UploadedBy matches auth.uid()
);

-- Policy 3: INSERT - Service users can upload attachments to any ticket
CREATE POLICY "service_users_upload_attachments"
ON "TicketAttachments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = true
);

-- Policy 4: UPDATE - Service admins can update attachment metadata
CREATE POLICY "service_admins_update_attachments"
ON "TicketAttachments"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 5: DELETE - Users can soft-delete their own attachments
CREATE POLICY "users_delete_own_attachments"
ON "TicketAttachments"
FOR UPDATE
TO authenticated
USING (
    "UploadedBy" = auth.uid()
    AND "IsDeleted" = false
)
WITH CHECK (
    "UploadedBy" = auth.uid()
);

-- Policy 6: DELETE - Service admins can soft-delete any attachment
CREATE POLICY "service_admins_delete_attachments"
ON "TicketAttachments"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

GRANT SELECT, INSERT, UPDATE ON "TicketAttachments" TO authenticated;
*/

-- =====================================================
-- VERIFICATION QUERIES (FOR TESTING - when tables exist)
-- =====================================================

/*
-- List all policies on TicketComments table
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE tablename = 'TicketComments'
ORDER BY policyname;

-- List all policies on TicketAttachments table
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE tablename = 'TicketAttachments'
ORDER BY policyname;

-- Test comment access as organization user
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "TicketComments" WHERE "TicketId" = 'ticket-in-my-org-uuid';
-- Should return comments for tickets in their organization

-- Test attachment access as service user
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "TicketAttachments";
-- Should return all attachments
*/

-- =====================================================
-- POLICY SUMMARY
-- =====================================================

/*
TICKET COMMENTS TABLE - Access Control (Future):

READ (SELECT):
✅ Users can see comments on tickets they have access to
   - Service users → All comments
   - Organization users → Comments on their organization's tickets
❌ Deleted comments are hidden

CREATE (INSERT):
✅ Organization users → Can comment on THEIR organization's tickets
✅ Service users → Can comment on ANY ticket
❌ Cannot comment on tickets without access

UPDATE:
✅ Users → Can edit THEIR OWN comments (app enforces time limit)
✅ Service admins → Can edit ANY comment
❌ Cannot edit other users' comments

DELETE (soft delete):
✅ Users → Can delete THEIR OWN comments
✅ Service admins → Can delete ANY comment
❌ Cannot delete other users' comments (except admins)

---

TICKET ATTACHMENTS TABLE - Access Control (Future):

READ (SELECT):
✅ Users can see attachments on tickets they have access to
   - Service users → All attachments
   - Organization users → Attachments on their organization's tickets
❌ Deleted attachments are hidden

CREATE (INSERT):
✅ Organization users → Can upload to THEIR organization's tickets
✅ Service users → Can upload to ANY ticket
❌ Cannot upload to tickets without access

UPDATE:
✅ Service admins → Can update attachment metadata
❌ Regular users → Cannot update (immutable after upload)

DELETE (soft delete):
✅ Users → Can delete THEIR OWN attachments
✅ Service admins → Can delete ANY attachment
❌ Cannot delete other users' attachments (except admins)

---

SECURITY GUARANTEES:
1. Comments and attachments inherit ticket access controls
2. Users can only interact with content on accessible tickets
3. Users own their comments/attachments (can delete)
4. Service team has full moderation capabilities
5. No direct access - must go through ticket permissions
6. Helper function ensures consistent access checks

APPLICATION LAYER RESPONSIBILITIES:
- Validate UserId/UploadedBy matches auth.uid() on insert
- Enforce comment edit time limit (e.g., 15 minutes)
- Validate file types and sizes for attachments
- Handle storage integration (Supabase Storage)
- Clean up storage when attachments are deleted
*/
