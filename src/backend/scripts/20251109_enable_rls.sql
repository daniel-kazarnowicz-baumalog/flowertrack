-- =====================================================
-- FLOWerTRACK - Enable Row Level Security (RLS)
-- Phase 2.1: Enable RLS on all tables
-- Created: 2025-11-09
-- =====================================================

-- This migration enables Row Level Security on all application tables.
-- Policies will be implemented in subsequent migrations (Phase 2.2-2.6).

-- =====================================================
-- 1. ENABLE ROW LEVEL SECURITY
-- =====================================================

-- Organizations table
ALTER TABLE "Organizations" ENABLE ROW LEVEL SECURITY;

-- Organization Users table
ALTER TABLE "OrganizationUsers" ENABLE ROW LEVEL SECURITY;

-- Service Users table
ALTER TABLE "ServiceUsers" ENABLE ROW LEVEL SECURITY;

-- Machines table
ALTER TABLE "Machines" ENABLE ROW LEVEL SECURITY;

-- Tickets table
ALTER TABLE "Tickets" ENABLE ROW LEVEL SECURITY;

-- Note: TicketComments and TicketAttachments tables will be created in future migrations
-- When those tables are created, RLS should be enabled immediately with:
-- ALTER TABLE "TicketComments" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "TicketAttachments" ENABLE ROW LEVEL SECURITY;

-- =====================================================
-- 2. CREATE HELPER FUNCTIONS FOR RLS POLICIES
-- =====================================================

-- Function to get current user's role from JWT claims
CREATE OR REPLACE FUNCTION auth.user_role()
RETURNS TEXT AS $$
    SELECT NULLIF(current_setting('request.jwt.claims', true)::json->>'role', '')::text;
$$ LANGUAGE SQL STABLE;

-- Function to get current user's Supabase UID from JWT
CREATE OR REPLACE FUNCTION auth.uid()
RETURNS UUID AS $$
    SELECT NULLIF(current_setting('request.jwt.claims', true)::json->>'sub', '')::uuid;
$$ LANGUAGE SQL STABLE;

-- Function to check if user is a service user
CREATE OR REPLACE FUNCTION public.is_service_user()
RETURNS BOOLEAN AS $$
    SELECT EXISTS (
        SELECT 1 
        FROM "ServiceUsers" 
        WHERE "SupabaseUserId" = auth.uid()
        AND "IsDeleted" = false
        AND "Status" = 'Active'
    );
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

-- Function to check if user is a service admin
CREATE OR REPLACE FUNCTION public.is_service_admin()
RETURNS BOOLEAN AS $$
    SELECT auth.user_role() = 'service_admin';
$$ LANGUAGE SQL STABLE;

-- Function to get current organization user's organization ID
CREATE OR REPLACE FUNCTION public.current_user_organization_id()
RETURNS UUID AS $$
    SELECT "OrganizationId"
    FROM "OrganizationUsers"
    WHERE "SupabaseUserId" = auth.uid()
    AND "IsDeleted" = false
    AND "IsActivated" = true
    LIMIT 1;
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

-- Function to check if user is an organization admin
CREATE OR REPLACE FUNCTION public.is_organization_admin()
RETURNS BOOLEAN AS $$
    SELECT EXISTS (
        SELECT 1
        FROM "OrganizationUsers"
        WHERE "SupabaseUserId" = auth.uid()
        AND "IsDeleted" = false
        AND "IsActivated" = true
        AND "Role" IN ('Owner', 'Admin')
    );
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

-- Function to check if user is an organization owner
CREATE OR REPLACE FUNCTION public.is_organization_owner()
RETURNS BOOLEAN AS $$
    SELECT EXISTS (
        SELECT 1
        FROM "OrganizationUsers"
        WHERE "SupabaseUserId" = auth.uid()
        AND "IsDeleted" = false
        AND "IsActivated" = true
        AND "Role" = 'Owner'
    );
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

-- Function to check if user belongs to a specific organization
CREATE OR REPLACE FUNCTION public.user_belongs_to_organization(org_id UUID)
RETURNS BOOLEAN AS $$
    SELECT EXISTS (
        SELECT 1
        FROM "OrganizationUsers"
        WHERE "SupabaseUserId" = auth.uid()
        AND "OrganizationId" = org_id
        AND "IsDeleted" = false
        AND "IsActivated" = true
    );
$$ LANGUAGE SQL STABLE SECURITY DEFINER;

-- =====================================================
-- 3. CREATE INDEXES FOR RLS PERFORMANCE
-- =====================================================

-- Index on SupabaseUserId for ServiceUsers (if not exists)
-- Already exists from migrations: IX_ServiceUsers_SupabaseUserId

-- Index on SupabaseUserId for OrganizationUsers (if not exists)
-- Already exists from migrations: IX_OrganizationUsers_SupabaseUserId

-- Index on OrganizationId for OrganizationUsers
-- Already exists from migrations: IX_OrganizationUsers_OrganizationId

-- Index on OrganizationId for Machines
-- Already exists from migrations: IX_Machines_OrganizationId

-- Index on OrganizationId for Tickets
-- Already exists from migrations: IX_Tickets_OrganizationId

-- =====================================================
-- 4. GRANT PERMISSIONS
-- =====================================================

-- Grant usage on schema to authenticated users
GRANT USAGE ON SCHEMA public TO authenticated;

-- Grant execute on helper functions
GRANT EXECUTE ON FUNCTION public.is_service_user() TO authenticated;
GRANT EXECUTE ON FUNCTION public.is_service_admin() TO authenticated;
GRANT EXECUTE ON FUNCTION public.current_user_organization_id() TO authenticated;
GRANT EXECUTE ON FUNCTION public.is_organization_admin() TO authenticated;
GRANT EXECUTE ON FUNCTION public.is_organization_owner() TO authenticated;
GRANT EXECUTE ON FUNCTION public.user_belongs_to_organization(UUID) TO authenticated;

-- =====================================================
-- 5. VERIFICATION QUERIES (FOR TESTING)
-- =====================================================

-- Verify RLS is enabled on all tables
-- Run this manually to verify:
/*
SELECT tablename, rowsecurity 
FROM pg_tables 
WHERE schemaname = 'public' 
AND tablename IN ('Organizations', 'OrganizationUsers', 'ServiceUsers', 'Machines', 'Tickets')
ORDER BY tablename;
*/

-- Expected output: All tables should have rowsecurity = true

-- =====================================================
-- NOTES
-- =====================================================

/*
RLS is now enabled on all tables but NO POLICIES are created yet.
This means:
- All queries will return ZERO rows for authenticated users
- Only roles with BYPASS RLS (postgres, service_role) can access data
- This is intentional and safe

Next steps (Phase 2.2-2.6):
1. Create policies for Organizations table
2. Create policies for OrganizationUsers table
3. Create policies for Machines table
4. Create policies for Tickets table
5. Create policies for TicketComments and TicketAttachments (when tables exist)

Policy design principles:
- Service users (service_admin, service_user) see ALL data
- Organization users see ONLY their organization's data
- Organization admins/owners can modify their organization's data
- Regular organization users have read-only access (except ticket creation)
*/
