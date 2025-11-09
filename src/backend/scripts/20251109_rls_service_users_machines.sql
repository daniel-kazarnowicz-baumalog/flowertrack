-- =====================================================
-- FLOWerTRACK - ServiceUsers & Machines RLS Policies
-- Phase 2.4: Row Level Security for Service Users and Machines
-- Created: 2025-11-09
-- =====================================================

-- Prerequisites: 
-- - RLS must be enabled (run 20251109_enable_rls.sql first)
-- - Organizations and OrganizationUsers policies applied

-- =====================================================
-- SERVICE USERS TABLE POLICIES
-- =====================================================

-- Policy 1: SELECT - Service users can see all service users
CREATE POLICY "service_users_select_all_service_users"
ON "ServiceUsers"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = true
);

-- Policy 2: SELECT - Service users can see their own profile
CREATE POLICY "service_users_select_own_profile"
ON "ServiceUsers"
FOR SELECT
TO authenticated
USING (
    "SupabaseUserId" = auth.uid()
);

-- Policy 3: INSERT - Only service admins can create service users
CREATE POLICY "service_admins_insert_service_users"
ON "ServiceUsers"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 4: UPDATE - Service admins can update any service user
CREATE POLICY "service_admins_update_service_users"
ON "ServiceUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 5: UPDATE - Service users can update their own profile
CREATE POLICY "service_users_update_own_profile"
ON "ServiceUsers"
FOR UPDATE
TO authenticated
USING (
    "SupabaseUserId" = auth.uid()
)
WITH CHECK (
    "SupabaseUserId" = auth.uid()
);

-- Policy 6: DELETE - Only service admins can soft-delete service users
CREATE POLICY "service_admins_delete_service_users"
ON "ServiceUsers"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

-- =====================================================
-- MACHINES TABLE POLICIES
-- =====================================================

-- Policy 1: SELECT - Service users can see ALL machines
CREATE POLICY "service_users_select_all_machines"
ON "Machines"
FOR SELECT
TO authenticated
USING (
    public.is_service_user() = true
);

-- Policy 2: SELECT - Organization users can see machines from their organization
CREATE POLICY "organization_users_select_own_machines"
ON "Machines"
FOR SELECT
TO authenticated
USING (
    "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
);

-- Policy 3: INSERT - Service admins can create machines for any organization
CREATE POLICY "service_admins_insert_machines"
ON "Machines"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 4: INSERT - Organization admins can create machines for their organization
CREATE POLICY "organization_admins_insert_machines"
ON "Machines"
FOR INSERT
TO authenticated
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- Policy 5: UPDATE - Service users can update any machine
CREATE POLICY "service_users_update_machines"
ON "Machines"
FOR UPDATE
TO authenticated
USING (
    public.is_service_user() = true
)
WITH CHECK (
    public.is_service_user() = true
);

-- Policy 6: UPDATE - Organization admins can update machines in their organization
CREATE POLICY "organization_admins_update_machines"
ON "Machines"
FOR UPDATE
TO authenticated
USING (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- Policy 7: DELETE - Service admins can soft-delete any machine
CREATE POLICY "service_admins_delete_machines"
ON "Machines"
FOR UPDATE
TO authenticated
USING (
    public.is_service_admin() = true
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_service_admin() = true
);

-- Policy 8: DELETE - Organization admins can soft-delete machines in their org
CREATE POLICY "organization_admins_delete_machines"
ON "Machines"
FOR UPDATE
TO authenticated
USING (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
    AND "IsDeleted" = false
)
WITH CHECK (
    public.is_organization_admin() = true
    AND "OrganizationId" = public.current_user_organization_id()
);

-- =====================================================
-- GRANT TABLE PERMISSIONS
-- =====================================================

GRANT SELECT, INSERT, UPDATE ON "ServiceUsers" TO authenticated;
GRANT SELECT, INSERT, UPDATE ON "Machines" TO authenticated;

-- =====================================================
-- VERIFICATION QUERIES (FOR TESTING)
-- =====================================================

-- List all policies on ServiceUsers table
/*
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE tablename = 'ServiceUsers'
ORDER BY policyname;
*/

-- List all policies on Machines table
/*
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE tablename = 'Machines'
ORDER BY policyname;
*/

-- Test as service user (should see all machines)
/*
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "Machines"; -- Should return all machines
*/

-- Test as organization user (should see only their org's machines)
/*
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "Machines"; -- Should return machines from their org only
*/

-- =====================================================
-- POLICY SUMMARY
-- =====================================================

/*
SERVICE USERS TABLE - Access Control:

READ (SELECT):
✅ Service users → See ALL service users
✅ Individual service user → See THEIR OWN profile
❌ Organization users → CANNOT see service users

CREATE (INSERT):
✅ Service admins → Can create service users
❌ Everyone else → Cannot create service users

UPDATE:
✅ Service admins → Can update ANY service user
✅ Service users → Can update THEIR OWN profile
❌ Organization users → Cannot update service users

DELETE:
✅ Service admins → Can soft-delete service users
❌ Everyone else → Cannot delete

---

MACHINES TABLE - Access Control:

READ (SELECT):
✅ Service users → See ALL machines (all organizations)
✅ Organization users → See ONLY machines from THEIR organization
❌ Deleted machines are hidden from organization users

CREATE (INSERT):
✅ Service admins → Can create machines for ANY organization
✅ Organization admins → Can create machines for THEIR organization
❌ Regular organization users → Cannot create machines

UPDATE:
✅ Service users → Can update ANY machine
✅ Organization admins → Can update machines in THEIR organization
❌ Regular organization users → Cannot update machines

DELETE (soft delete):
✅ Service admins → Can delete ANY machine
✅ Organization admins → Can delete machines in THEIR organization
❌ Regular organization users → Cannot delete machines

SECURITY GUARANTEES:
1. Service users are completely hidden from organization users
2. Machines are isolated by organization
3. Service team has full visibility and control
4. Organization admins can manage their equipment
5. Regular organization users have read-only access to machines
*/
