-- =====================================================
-- FLOWerTRACK - Tickets Table RLS Policies
-- Phase 2.5: Row Level Security for Tickets
-- Created: 2025-11-09
-- =====================================================

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - All previous policies applied (Organizations, OrganizationUsers, ServiceUsers, Machines)

-- =====================================================
-- TICKETS TABLE POLICIES
-- =====================================================

-- Policy 1: SELECT - Service users can see ALL tickets
CREATE POLICY "service_users_select_all_tickets"
ON "Tickets"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = true
);

-- Policy 2: SELECT - Organization users can see tickets from their organization
CREATE POLICY "organization_users_select_own_tickets"
ON "Tickets"
FOR SELECT
TO authenticated
USING (
    "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
);

-- Policy 3: INSERT - Organization users can create tickets for their organization
CREATE POLICY "organization_users_insert_tickets"
ON "Tickets"
FOR INSERT
TO authenticated
WITH CHECK (
    public.user_belongs_to_organization("OrganizationId") = true
    -- Note: Application layer should validate that CreatedByUserId matches auth.uid()
);

-- Policy 4: INSERT - Service users can create tickets for any organization
CREATE POLICY "service_users_insert_tickets"
ON "Tickets"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_user() = true
);

-- Policy 5: UPDATE - Service users can update any ticket (status, assignment, etc.)
CREATE POLICY "service_users_update_tickets"
ON "Tickets"
FOR UPDATE
TO authenticated
USING (
    public.is_service_user() = true
)
WITH CHECK (
    public.is_service_user() = true
);

-- Policy 6: UPDATE - Organization users can update their organization's tickets (limited)
-- Note: Application layer should restrict updates to specific fields only
-- (e.g., organization users cannot change Status or AssignedToUserId)
CREATE POLICY "organization_users_update_own_tickets"
ON "Tickets"
FOR UPDATE
TO authenticated
USING (
    "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
)
WITH CHECK (
    "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
);

-- Policy 7: DELETE - Service admins can soft-delete any ticket
CREATE POLICY "service_admins_delete_tickets"
ON "Tickets"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 8: DELETE - Organization admins can soft-delete tickets from their organization
-- Only if ticket status is New (not yet in progress)
CREATE POLICY "organization_admins_delete_new_tickets"
ON "Tickets"
FOR UPDATE
TO authenticated
USING (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
    AND "Status" = 'New'
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- =====================================================
-- GRANT TABLE PERMISSIONS
-- =====================================================

GRANT SELECT, INSERT, UPDATE ON "Tickets" TO authenticated;

-- =====================================================
-- VERIFICATION QUERIES (FOR TESTING)
-- =====================================================

-- List all policies on Tickets table
/*
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE tablename = 'Tickets'
ORDER BY policyname;
*/

-- Test as service user (should see all tickets)
/*
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "Tickets"; -- Should return all tickets
*/

-- Test as organization user (should see only their org's tickets)
/*
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "Tickets"; -- Should return tickets from their org only
*/

-- Test ticket creation
/*
-- As organization user
INSERT INTO "Tickets" (
    "Id", "OrganizationId", "MachineId", "CreatedByUserId",
    "Title", "Description", "Priority", "Status"
) VALUES (
    gen_random_uuid(),
    public.current_user_organization_id(), -- Their org
    'machine-uuid',
    auth.uid(), -- Their user ID
    'Test Ticket',
    'Test Description',
    'Medium',
    'New'
); -- Should succeed
*/

-- =====================================================
-- POLICY SUMMARY
-- =====================================================

/*
TICKETS TABLE - Access Control:

READ (SELECT):
✅ Service users → See ALL tickets (all organizations)
✅ Organization users → See ONLY tickets from THEIR organization
❌ Deleted tickets are hidden from organization users

CREATE (INSERT):
✅ Organization users → Can create tickets for THEIR organization
✅ Service users → Can create tickets for ANY organization
❌ Cannot create tickets for other organizations

UPDATE:
✅ Service users → Can update ANY ticket (status, assignment, priority, etc.)
✅ Organization users → Can update tickets from THEIR organization
   ⚠️  Application layer MUST restrict which fields org users can modify
   ⚠️  Typically, org users can only add comments or update description
   ⚠️  Service users control Status, AssignedToUserId, Priority changes

DELETE (soft delete):
✅ Service admins → Can delete ANY ticket
✅ Organization admins → Can delete THEIR NEW tickets only (Status = 'New')
❌ Regular organization users → Cannot delete tickets
❌ Cannot delete tickets that are in progress

BUSINESS RULES ENFORCED:
1. Tickets are isolated by organization
2. Organization users can only work with their tickets
3. Service team has full visibility and control
4. Ticket creation is open to organization users (self-service)
5. Status management is controlled by service team
6. Organization admins can cancel new tickets before work begins
7. Once ticket is in progress, only service can delete it

APPLICATION LAYER RESPONSIBILITIES:
- Validate CreatedByUserId matches auth.uid() on insert
- Restrict organization users from modifying: Status, AssignedToUserId, Priority
- Allow organization users to update: Description, add info via comments
- Enforce ticket workflow (New → InProgress → Resolved → Closed)
*/
