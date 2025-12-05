-- =====================================================
-- FLOWerTRACK - Create Test Users for Development
-- =====================================================
-- This script creates test users for development and testing purposes
-- 
-- Service Portal Test Users:
-- - Email: admin@flowertrack.dev | Password: Admin123!
-- - Email: tech@flowertrack.dev  | Password: Tech123!
--
-- Client Portal Test Users:
-- - Email: client@test.com | Password: Client123!
-- - Email: operator@test.com | Password: Operator123!
--
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
    "ApiKey",
    "ServiceStatus",
    "CreatedAt",
    "UpdatedAt"
)
VALUES (
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Test Organization',
    'contact@test.com',
    '+48 123 456 789',
    'test-api-key-12345',
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("Id") DO UPDATE SET
    "Name" = EXCLUDED."Name",
    "UpdatedAt" = NOW();

-- =====================================================
-- 2. CREATE SERVICE USERS (Admin & Technician)
-- =====================================================

-- Service Admin User
-- Email: admin@flowertrack.dev
-- Password: Admin123! (hashed with BCrypt)
INSERT INTO "ServiceUsers" (
    "Id",
    "Email",
    "PasswordHash",
    "FullName",
    "Role",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES (
    '660e8400-e29b-41d4-a716-446655440001'::uuid,
    'admin@flowertrack.dev',
    '$2a$11$LQ3h5g3hKZQj5VxZ0xGZJeYvZqKr7jHqYvZqKr7jHqYvZqKr7jHqY', -- Admin123!
    'Admin User',
    'Admin',
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("Email") DO UPDATE SET
    "FullName" = EXCLUDED."FullName",
    "Role" = EXCLUDED."Role",
    "Status" = EXCLUDED."Status",
    "UpdatedAt" = NOW();

-- Service Technician User
-- Email: tech@flowertrack.dev
-- Password: Tech123!
INSERT INTO "ServiceUsers" (
    "Id",
    "Email",
    "PasswordHash",
    "FullName",
    "Role",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES (
    '660e8400-e29b-41d4-a716-446655440002'::uuid,
    'tech@flowertrack.dev',
    '$2a$11$LQ3h5g3hKZQj5VxZ0xGZJeYvZqKr7jHqYvZqKr7jHqYvZqKr7jHqY', -- Tech123!
    'Tech Support',
    'Technician',
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("Email") DO UPDATE SET
    "FullName" = EXCLUDED."FullName",
    "Role" = EXCLUDED."Role",
    "Status" = EXCLUDED."Status",
    "UpdatedAt" = NOW();

-- =====================================================
-- 3. CREATE ORGANIZATION USERS (Admin & Operator)
-- =====================================================

-- Organization Admin User
-- Email: client@test.com
-- Password: Client123!
INSERT INTO "OrganizationUsers" (
    "Id",
    "Email",
    "PasswordHash",
    "FirstName",
    "LastName",
    "OrganizationId",
    "IsAdmin",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES (
    '770e8400-e29b-41d4-a716-446655440001'::uuid,
    'client@test.com',
    '$2a$11$LQ3h5g3hKZQj5VxZ0xGZJeYvZqKr7jHqYvZqKr7jHqYvZqKr7jHqY', -- Client123!
    'Client',
    'Admin',
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    true,
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("Email") DO UPDATE SET
    "FirstName" = EXCLUDED."FirstName",
    "LastName" = EXCLUDED."LastName",
    "OrganizationId" = EXCLUDED."OrganizationId",
    "IsAdmin" = EXCLUDED."IsAdmin",
    "Status" = EXCLUDED."Status",
    "UpdatedAt" = NOW();

-- Organization Operator User
-- Email: operator@test.com
-- Password: Operator123!
INSERT INTO "OrganizationUsers" (
    "Id",
    "Email",
    "PasswordHash",
    "FirstName",
    "LastName",
    "OrganizationId",
    "IsAdmin",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES (
    '770e8400-e29b-41d4-a716-446655440002'::uuid,
    'operator@test.com',
    '$2a$11$LQ3h5g3hKZQj5VxZ0xGZJeYvZqKr7jHqYvZqKr7jHqYvZqKr7jHqY', -- Operator123!
    'John',
    'Operator',
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    false,
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("Email") DO UPDATE SET
    "FirstName" = EXCLUDED."FirstName",
    "LastName" = EXCLUDED."LastName",
    "OrganizationId" = EXCLUDED."OrganizationId",
    "IsAdmin" = EXCLUDED."IsAdmin",
    "Status" = EXCLUDED."Status",
    "UpdatedAt" = NOW();

-- =====================================================
-- 4. CREATE SAMPLE MACHINES
-- =====================================================

INSERT INTO "Machines" (
    "Id",
    "Brand",
    "Model",
    "SerialNumber",
    "OrganizationId",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES 
(
    '880e8400-e29b-41d4-a716-446655440001'::uuid,
    'Baumalog',
    'FlowMaster 3000',
    'FM3000-001',
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Active',
    NOW(),
    NOW()
),
(
    '880e8400-e29b-41d4-a716-446655440002'::uuid,
    'Baumalog',
    'FlowMaster 3000',
    'FM3000-002',
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Active',
    NOW(),
    NOW()
)
ON CONFLICT ("SerialNumber") DO UPDATE SET
    "Brand" = EXCLUDED."Brand",
    "Model" = EXCLUDED."Model",
    "Status" = EXCLUDED."Status",
    "UpdatedAt" = NOW();

COMMIT;

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================

SELECT '=== SERVICE USERS ===' as info;
SELECT "Email", "FullName", "Role", "Status" 
FROM "ServiceUsers" 
WHERE "Email" IN ('admin@flowertrack.dev', 'tech@flowertrack.dev');

SELECT '=== ORGANIZATION USERS ===' as info;
SELECT "Email", "FirstName", "LastName", "IsAdmin", "Status", "OrganizationId"
FROM "OrganizationUsers" 
WHERE "Email" IN ('client@test.com', 'operator@test.com');

SELECT '=== ORGANIZATIONS ===' as info;
SELECT "Id", "Name", "Email", "ServiceStatus" 
FROM "Organizations" 
WHERE "Id" = '550e8400-e29b-41d4-a716-446655440001'::uuid;

SELECT '=== MACHINES ===' as info;
SELECT "Id", "Brand", "Model", "SerialNumber", "Status" 
FROM "Machines" 
WHERE "OrganizationId" = '550e8400-e29b-41d4-a716-446655440001'::uuid;
