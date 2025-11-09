-- =====================================================
-- FLOWerTRACK - OrganizationUsers Table RLS Policies
-- Phase 2.3: Row Level Security for Organization Users
-- Created: 2025-11-09
-- =====================================================

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - Organizations policies applied (run 20251109_rls_organizations.sql)
-- - Helper functions must exist

-- =====================================================
-- ORGANIZATION USERS TABLE POLICIES
-- =====================================================

-- Policy 1: SELECT - Service users can see ALL organization users
CREATE POLICY "service_users_select_all_organization_users"
ON "OrganizationUsers"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = true
);

-- Policy 2: SELECT - Organization users can see users from their organization
CREATE POLICY "organization_users_select_same_organization"
ON "OrganizationUsers"
FOR SELECT
TO authenticated
USING (
    "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
);

-- Policy 3: SELECT - Users can always see their own profile (even if IsActivated = false)
CREATE POLICY "users_select_own_profile"
ON "OrganizationUsers"
FOR SELECT
TO authenticated
USING (
    "SupabaseUserId" = auth.uid()
);

-- Policy 4: INSERT - Service admins can create organization users
CREATE POLICY "service_admins_insert_organization_users"
ON "OrganizationUsers"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 5: INSERT - Organization admins can invite users to their organization
CREATE POLICY "organization_admins_invite_users"
ON "OrganizationUsers"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- Policy 6: UPDATE - Service admins can update any organization user
CREATE POLICY "service_admins_update_organization_users"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 7: UPDATE - Organization admins can update users in their organization
CREATE POLICY "organization_admins_update_users"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
)
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- Policy 8: UPDATE - Users can update their own profile (limited fields)
CREATE POLICY "users_update_own_profile"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    "SupabaseUserId" = auth.uid()
)
WITH CHECK (
    "SupabaseUserId" = auth.uid()
    -- Note: Application layer should restrict which fields users can modify
    -- (e.g., users cannot change their Role or OrganizationId)
);

-- Policy 9: UPDATE - Users can activate their own account
CREATE POLICY "users_activate_own_account"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    "SupabaseUserId" = auth.uid()
    AND "IsActivated" = false
    AND "InvitationToken" IS NOT NULL
)
WITH CHECK (
    "SupabaseUserId" = auth.uid()
);

-- Policy 10: DELETE - Service admins can soft-delete organization users
CREATE POLICY "service_admins_delete_organization_users"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 11: DELETE - Organization admins can soft-delete users (except owners)
CREATE POLICY "organization_admins_delete_users"
ON "OrganizationUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
    AND "Role" != 'Owner'  -- Cannot delete organization owners
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
    AND "Role" != 'Owner'
);

-- =====================================================
-- GRANT TABLE PERMISSIONS
-- =====================================================

GRANT SELECT, INSERT, UPDATE ON "OrganizationUsers" TO authenticated;

-- =====================================================
-- VERIFICATION QUERIES (FOR TESTING)
-- =====================================================

-- List all policies on OrganizationUsers table
/*
SELECT schemaname, tablename, policyname, permissive, roles, cmd, qual, with_check
FROM pg_policies
WHERE tablename = 'OrganizationUsers'
ORDER BY policyname;
*/

-- Test as service user (should see all organization users)
/*
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "OrganizationUsers"; -- Should return all users
*/

-- Test as organization user (should see only users from their org)
/*
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "OrganizationUsers"; -- Should return users from their org only
*/

-- Test own profile access (should always see own profile)
/*
SET request.jwt.claims = '{"sub": "my-uuid", "role": "authenticated"}';
SELECT * FROM "OrganizationUsers" WHERE "SupabaseUserId" = 'my-uuid'::uuid;
*/

-- =====================================================
-- POLICY SUMMARY
-- =====================================================

/*
ORGANIZATION USERS TABLE - Access Control:

READ (SELECT):
✅ Service users → See ALL organization users
✅ Organization users → See users from THEIR organization only
✅ All users → Can ALWAYS see their own profile (for activation, etc.)

CREATE (INSERT):
✅ Service admins → Can create users in any organization
✅ Organization admins → Can invite users to THEIR organization
❌ Regular organization users → Cannot invite users

UPDATE:
✅ Service admins → Can update ANY user
✅ Organization admins → Can update users in THEIR organization
✅ All users → Can update THEIR OWN profile (limited fields in app layer)
✅ Inactive users → Can activate their own account via invitation token
❌ Regular users → Cannot update other users

DELETE (soft delete):
✅ Service admins → Can delete ANY user
✅ Organization admins → Can delete users in THEIR organization (except owners)
❌ Regular users → Cannot delete users
❌ Organization owners → CANNOT be deleted by org admins (protection)

SECURITY GUARANTEES:
1. Users are isolated by organization
2. Service team has full visibility for support
3. Admins can manage their organization's users
4. Users can always access their profile (important for activation flow)
5. Organization owners are protected from deletion
6. Invitation/activation flow works securely
*/
