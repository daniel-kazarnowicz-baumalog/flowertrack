-- =====================================================
-- FLOWerTRACK - Seed Test Data for Development
-- =====================================================
-- This script creates test organizations and users linked to auth.users
-- 
-- Prerequisites: Run 01-create-auth-users.sql first to create Supabase auth users
--
-- Test Users:
-- - client@test.com / Client123! (Organization Admin)
-- - operator@test.com / Operator123! (Organization Operator)
-- =====================================================

BEGIN;

-- =====================================================
-- 1. CREATE TEST ORGANIZATION
-- =====================================================

INSERT INTO "Organizations" (
    "Id",
    "Name",
    "Email",
    "Phone",
    "Address",
    "City",
    "PostalCode",
    "Country",
    "ServiceStatus",
    "ContractStartDate",
    "ContractEndDate",
    "Notes",
    "CreatedAt",
    "UpdatedAt",
    "IsDeleted"
)
SELECT
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Test Organization',
    'contact@testorg.com',
    '+48 123 456 789',
    'ul. Testowa 123',
    'Warszawa',
    '00-001',
    'Poland',
    0, -- ServiceStatus: Active (enum value)
    NOW() - INTERVAL '30 days',
    NOW() + INTERVAL '365 days',
    'Test organization for development purposes',
    NOW(),
    NOW(),
    false
WHERE NOT EXISTS (
    SELECT 1 FROM "Organizations" WHERE "Id" = '550e8400-e29b-41d4-a716-446655440001'::uuid
);

-- =====================================================
-- 2. CREATE ORGANIZATION USERS LINKED TO AUTH.USERS
-- =====================================================

-- Get Supabase User IDs from auth.users
DO $$
DECLARE
    v_client_supabase_id uuid;
    v_operator_supabase_id uuid;
BEGIN
    -- Get client@test.com Supabase ID
    SELECT id INTO v_client_supabase_id
    FROM auth.users
    WHERE email = 'client@test.com';
    
    -- Get operator@test.com Supabase ID
    SELECT id INTO v_operator_supabase_id
    FROM auth.users
    WHERE email = 'operator@test.com';
    
    -- Insert client@test.com as Organization Admin
    IF v_client_supabase_id IS NOT NULL THEN
        INSERT INTO "OrganizationUsers" (
            "Id",
            "FirstName",
            "LastName",
            "Email",
            "OrganizationId",
            "PhoneNumber",
            "Status",
            "Role",
            "CreatedAt",
            "UpdatedAt",
            "IsActivated",
            "IsDeleted",
            "SupabaseUserId"
        )
        SELECT
            '770e8400-e29b-41d4-a716-446655440001'::uuid,
            'Client',
            'Admin',
            'client@test.com',
            '550e8400-e29b-41d4-a716-446655440001'::uuid,
            '+48 111 222 333',
            'Active',
            'Admin',
            NOW(),
            NOW(),
            true,
            false,
            v_client_supabase_id
        WHERE NOT EXISTS (
            SELECT 1 FROM "OrganizationUsers" WHERE "Email" = 'client@test.com'
        );
        
        RAISE NOTICE 'Created client@test.com with SupabaseUserId: %', v_client_supabase_id;
    ELSE
        RAISE WARNING 'client@test.com not found in auth.users! Run 01-create-auth-users.sql first.';
    END IF;
    
    -- Insert operator@test.com as Organization Operator
    IF v_operator_supabase_id IS NOT NULL THEN
        INSERT INTO "OrganizationUsers" (
            "Id",
            "FirstName",
            "LastName",
            "Email",
            "OrganizationId",
            "PhoneNumber",
            "Status",
            "Role",
            "CreatedAt",
            "UpdatedAt",
            "IsActivated",
            "IsDeleted",
            "SupabaseUserId"
        )
        SELECT
            '770e8400-e29b-41d4-a716-446655440002'::uuid,
            'John',
            'Operator',
            'operator@test.com',
            '550e8400-e29b-41d4-a716-446655440001'::uuid,
            '+48 444 555 666',
            'Active',
            'Operator',
            NOW(),
            NOW(),
            true,
            false,
            v_operator_supabase_id
        WHERE NOT EXISTS (
            SELECT 1 FROM "OrganizationUsers" WHERE "Email" = 'operator@test.com'
        );
        
        RAISE NOTICE 'Created operator@test.com with SupabaseUserId: %', v_operator_supabase_id;
    ELSE
        RAISE WARNING 'operator@test.com not found in auth.users! Run 01-create-auth-users.sql first.';
    END IF;
END $$;

-- =====================================================
-- 3. VERIFY CREATED DATA
-- =====================================================

SELECT 'Organizations:' as info;
SELECT "Id", "Name", "Email", "ServiceStatus" FROM "Organizations";

SELECT 'OrganizationUsers:' as info;
SELECT "Id", "Email", "FirstName", "LastName", "Role", "IsActivated", "SupabaseUserId" 
FROM "OrganizationUsers";

SELECT 'Auth Users:' as info;
SELECT id, email FROM auth.users WHERE email LIKE '%test.com%';

COMMIT;
