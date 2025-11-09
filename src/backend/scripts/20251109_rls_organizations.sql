-- =====================================================
-- FLOWerTRACK - Organizations Table RLS Policies
-- Phase 2.2: Row Level Security for Organizations
-- Created: 2025-11-09
-- =====================================================

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - Helper functions must exist (is_service_user, current_user_organization_id, etc.)

-- =====================================================
-- ORGANIZATIONS TABLE POLICIES
-- =====================================================

-- Policy 1: SELECT - Service users can see ALL organizations
CREATE POLICY "service_users_select_all_organizations"
ON "Organizations"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = true
);

-- Policy 2: SELECT - Organization users can see ONLY their organization
CREATE POLICY "organization_users_select_own_organization"
ON "Organizations"
FOR SELECT
TO authenticated
USING (
    "Id" = public.current_user_organization_id()
);

-- Policy 3: INSERT - Only service admins can create organizations
CREATE POLICY "service_admins_insert_organizations"
ON "Organizations"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 4: UPDATE - Service admins can update any organization
CREATE POLICY "service_admins_update_all_organizations"
ON "Organizations"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 5: UPDATE - Organization owners can update their own organization
CREATE POLICY "organization_owners_update_own_organization"
ON "Organizations"
FOR UPDATE
TO authenticated
USING (
    "Id" = public.current_user_organization_id()
    AND public.is_organization_owner() = true
)
WITH CHECK (
    "Id" = public.current_user_organization_id()
    AND public.is_organization_owner() = true
);

-- Policy 6: DELETE - Only service admins can delete organizations (soft delete)
CREATE POLICY "service_admins_delete_organizations"
ON "Organizations"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Note: We use UPDATE for soft deletes, not DELETE statement
-- The application sets IsDeleted = true instead of actually deleting rows

-- =====================================================
-- GRANT TABLE PERMISSIONS
-- =====================================================

-- Grant basic permissions to authenticated users
GRANT SELECT, INSERT, UPDATE ON "Organizations" TO authenticated;

-- Note: DELETE is not granted because we use soft deletes (UPDATE IsDeleted)

-- =====================================================
-- VERIFICATION QUERIES (FOR TESTING)
-- =====================================================

-- List all policies on Organizations table
/*
SELECT schemaname, tablename, policyname, permissive, roles, cmd, qual, with_check
FROM pg_policies
WHERE tablename = 'Organizations'
ORDER BY policyname;
*/

-- Test as service user (should see all organizations)
/*
-- Set session to simulate service user
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "Organizations"; -- Should return all organizations
*/

-- Test as organization user (should see only their organization)
/*
-- Set session to simulate organization user
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "Organizations"; -- Should return 1 (their organization only)
*/

-- =====================================================
-- POLICY SUMMARY
-- =====================================================

/*
ORGANIZATIONS TABLE - Access Control:

READ (SELECT):
✅ Service users → See ALL organizations
✅ Organization users → See ONLY their organization

CREATE (INSERT):
✅ Service admins → Can create organizations
❌ Organization users → Cannot create organizations

UPDATE:
✅ Service admins → Can update ANY organization
✅ Organization owners → Can update ONLY their organization
❌ Organization admins → Cannot update (only owners)
❌ Regular organization users → Cannot update

DELETE:
✅ Service admins → Can soft-delete organizations
❌ Everyone else → Cannot delete

SECURITY GUARANTEES:
1. Organizations are isolated - users can only see their own
2. Service team has full visibility for support
3. Only owners can modify their organization (prevents rogue admins)
4. Service admins have full control for management
*/
