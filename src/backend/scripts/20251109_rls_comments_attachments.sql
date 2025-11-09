-- =====================================================
-- FLOWerTRACK - TicketComments & TicketAttachments RLS Policies
-- Phase 4.7: Row Level Security for Ticket Comments and Attachments
-- Created: 2025-11-09
-- Updated: 2025-11-09 (Phase 4 implementation)
-- =====================================================

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - All previous policies applied
-- - TicketComments and TicketAttachments tables created (Phase 4.6 migration)

-- =====================================================
-- TICKET COMMENTS TABLE POLICIES
-- =====================================================

-- Policy 1: Service users can SELECT all comments
CREATE POLICY "ServiceUsers_SelectAll_TicketComments"
ON "TicketComments"
FOR SELECT
TO authenticated
USING (public.is_service_user() = true);

-- Policy 2: Organization users can SELECT comments on tickets they have access to
-- They can see all public comments (IsInternal = false) and internal comments if they are service users
CREATE POLICY "OrgUsers_SelectAccessible_TicketComments"
ON "TicketComments"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = false
    AND public.user_has_ticket_access("TicketId") = true
    AND (
        "IsInternal" = false  -- Public comments visible to all
        OR public.is_service_user() = true  -- Internal comments only for service users
    )
    AND "IsDeleted" = false
);

-- Policy 3: Service users can INSERT comments on any ticket
CREATE POLICY "ServiceUsers_InsertAny_TicketComments"
ON "TicketComments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = true
    AND public.user_has_ticket_access("TicketId") = true
);

-- Policy 4: Organization users can INSERT comments on accessible tickets (only public comments)
CREATE POLICY "OrgUsers_InsertAccessible_TicketComments"
ON "TicketComments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = false
    AND public.user_has_ticket_access("TicketId") = true
    AND "IsInternal" = false  -- Organization users cannot create internal comments
    AND "UserId" = auth.uid()  -- Must be their own user ID
);

-- Policy 5: Users can UPDATE their own comments (within time limit enforced by app layer)
CREATE POLICY "Users_UpdateOwn_TicketComments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    "UserId" = auth.uid()
    AND "IsDeleted" = false
    AND public.user_has_ticket_access("TicketId") = true
)
WITH CHECK (
    "UserId" = auth.uid()
    AND public.user_has_ticket_access("TicketId") = true
);

-- Policy 6: Users can DELETE (soft) their own comments
CREATE POLICY "Users_DeleteOwn_TicketComments"
ON "TicketComments"
FOR UPDATE
TO authenticated
USING (
    "UserId" = auth.uid()
    AND "IsDeleted" = false
    AND public.user_has_ticket_access("TicketId") = true
)
WITH CHECK (
    "UserId" = auth.uid()
    AND "IsDeleted" = true  -- Soft delete by setting IsDeleted = true
);

-- =====================================================
-- TICKET ATTACHMENTS TABLE POLICIES
-- =====================================================

-- Policy 7: Service users can SELECT all attachments
CREATE POLICY "ServiceUsers_SelectAll_TicketAttachments"
ON "TicketAttachments"
FOR SELECT
TO authenticated
USING (public.is_service_user() = true);

-- Policy 8: Organization users can SELECT attachments on accessible tickets
CREATE POLICY "OrgUsers_SelectAccessible_TicketAttachments"
ON "TicketAttachments"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = false
    AND public.user_has_ticket_access("TicketId") = true
    AND "IsDeleted" = false
);

-- Policy 9: Service users can INSERT attachments on any ticket
CREATE POLICY "ServiceUsers_InsertAny_TicketAttachments"
ON "TicketAttachments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = true
    AND public.user_has_ticket_access("TicketId") = true
);

-- Policy 10: Organization users can INSERT attachments on accessible tickets
CREATE POLICY "OrgUsers_InsertAccessible_TicketAttachments"
ON "TicketAttachments"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = false
    AND public.user_has_ticket_access("TicketId") = true
    AND "UploadedBy" = auth.uid()
);

-- Policy 11: Users can DELETE (soft) their own attachments
CREATE POLICY "Users_DeleteOwn_TicketAttachments"
ON "TicketAttachments"
FOR UPDATE
TO authenticated
USING (
    "UploadedBy" = auth.uid()
    AND "IsDeleted" = false
    AND public.user_has_ticket_access("TicketId") = true
)
WITH CHECK (
    "UploadedBy" = auth.uid()
    AND "IsDeleted" = true  -- Soft delete
);

-- Policy 12: Service admins can DELETE any attachment
CREATE POLICY "ServiceAdmins_DeleteAny_TicketAttachments"
ON "TicketAttachments"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
    AND "IsDeleted" = true
);

-- =====================================================
-- ACCESS CONTROL SUMMARY
-- =====================================================

/*

TICKET COMMENTS ACCESS MATRIX:
┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ SELECT  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Service User     │ ✓       │ All comments (internal & public)             │
│ Org User         │ ✓       │ Public comments on accessible tickets        │
│                  │         │ (IsInternal = false, ticket accessible)      │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ INSERT  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Service User     │ ✓       │ Any ticket, can mark as internal             │
│ Org User         │ ✓       │ Accessible tickets, public comments only     │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ UPDATE  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Comment Author   │ ✓       │ Own comments (time limit in app layer)       │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ DELETE  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Comment Author   │ ✓       │ Own comments (soft delete)                   │
└──────────────────┴─────────┴──────────────────────────────────────────────┘


TICKET ATTACHMENTS ACCESS MATRIX:
┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ SELECT  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Service User     │ ✓       │ All attachments                              │
│ Org User         │ ✓       │ Attachments on accessible tickets            │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ INSERT  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Service User     │ ✓       │ Any accessible ticket                        │
│ Org User         │ ✓       │ Accessible tickets, own uploads              │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

┌──────────────────┬─────────┬──────────────────────────────────────────────┐
│ User Role        │ DELETE  │ Scope & Conditions                           │
├──────────────────┼─────────┼──────────────────────────────────────────────┤
│ Uploader         │ ✓       │ Own attachments (soft delete)                │
│ Service Admin    │ ✓       │ Any attachment (soft delete)                 │
└──────────────────┴─────────┴──────────────────────────────────────────────┘

IMPORTANT NOTES:
1. user_has_ticket_access() function must be created in 20251109_enable_rls.sql
2. Internal comments (IsInternal = true) are only visible to service users
3. Organization users cannot create internal comments
4. Soft deletes: IsDeleted flag set to true, actual deletion prevented
5. Storage cleanup for deleted attachments handled by application layer
6. All policies require user to be authenticated

*/

-- =====================================================
-- VERIFICATION QUERIES
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
SELECT * FROM "TicketComments" WHERE "TicketId" = 'ticket-in-my-org-uuid' AND "IsDeleted" = false;
-- Should return only public comments (IsInternal = false)

-- Test attachment access as service user
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "TicketAttachments" WHERE "IsDeleted" = false;
-- Should return all non-deleted attachments
*/

