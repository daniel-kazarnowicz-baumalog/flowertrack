-- =====================================================
-- Development Seed: Test Users for Local Development
-- =====================================================
-- This migration seeds test users for development purposes only
-- It's safe to run multiple times (uses ON CONFLICT)
-- 
-- IMPORTANT: This should only run in Development environment!
-- 
-- Service Portal Test Users:
-- - Email: admin@flowertrack.dev | Password: Admin123!
-- - Email: tech@flowertrack.dev  | Password: Tech123!
--
-- Client Portal Test Users:
-- - Email: client@test.com | Password: Client123!
-- - Email: operator@test.com | Password: Operator123!
-- =====================================================

-- Only run in development (check can be added via application logic)
DO $$
BEGIN
    -- Log execution
    RAISE NOTICE 'Seeding test users for development environment...';
    
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
        "ServiceStatus" = EXCLUDED."ServiceStatus",
        "UpdatedAt" = NOW();
    
    RAISE NOTICE 'Test organization created/updated';
    
    -- =====================================================
    -- 2. CREATE SERVICE USERS (via Supabase Auth)
    -- =====================================================
    -- NOTE: These INSERTs will fail if Supabase Auth integration is active
    -- In that case, use the Supabase Dashboard to create users manually
    -- or use the Supabase Auth API
    
    -- For now, we'll create placeholder records that can be linked later
    -- Real production would use Supabase Auth to create these users first
    
    -- Service Admin User
    -- Email: admin@flowertrack.dev
    -- Password: Admin123! 
    INSERT INTO "ServiceUsers" (
        "Id",
        "SupabaseUserId",
        "FirstName",
        "LastName",
        "Email",
        "PhoneNumber",
        "Status",
        "Specialization",
        "IsAvailable",
        "CreatedAt",
        "UpdatedAt"
    )
    VALUES (
        '660e8400-e29b-41d4-a716-446655440001'::uuid,
        NULL, -- Will be linked when Supabase Auth user is created
        'Admin',
        'User',
        'admin@flowertrack.dev',
        '+48 123 456 001',
        'Pending', -- Will be activated after Supabase Auth link
        'Administrator',
        false,
        NOW(),
        NOW()
    )
    ON CONFLICT (("Email"->'Value')) DO UPDATE SET
        "FirstName" = EXCLUDED."FirstName",
        "LastName" = EXCLUDED."LastName",
        "UpdatedAt" = NOW();
    
    -- Service Technician User
    -- Email: tech@flowertrack.dev
    -- Password: Tech123!
    INSERT INTO "ServiceUsers" (
        "Id",
        "SupabaseUserId",
        "FirstName",
        "LastName",
        "Email",
        "PhoneNumber",
        "Status",
        "Specialization",
        "IsAvailable",
        "CreatedAt",
        "UpdatedAt"
    )
    VALUES (
        '660e8400-e29b-41d4-a716-446655440002'::uuid,
        NULL,
        'Tech',
        'Support',
        'tech@flowertrack.dev',
        '+48 123 456 002',
        'Pending',
        'Technical Support',
        false,
        NOW(),
        NOW()
    )
    ON CONFLICT (("Email"->'Value')) DO UPDATE SET
        "FirstName" = EXCLUDED."FirstName",
        "LastName" = EXCLUDED."LastName",
        "UpdatedAt" = NOW();
    
    RAISE NOTICE 'Service users created/updated (pending Supabase Auth link)';
    
    -- =====================================================
    -- 3. CREATE ORGANIZATION USERS
    -- =====================================================
    
    -- Organization Admin User
    -- Email: client@test.com
    -- Password: Client123!
    INSERT INTO "OrganizationUsers" (
        "Id",
        "SupabaseUserId",
        "Email",
        "FirstName",
        "LastName",
        "OrganizationId",
        "Status",
        "CreatedAt",
        "UpdatedAt"
    )
    VALUES (
        '770e8400-e29b-41d4-a716-446655440001'::uuid,
        NULL,
        'client@test.com',
        'Client',
        'Admin',
        '550e8400-e29b-41d4-a716-446655440001'::uuid,
        'Pending',
        NOW(),
        NOW()
    )
    ON CONFLICT (("Email"->'Value')) DO UPDATE SET
        "FirstName" = EXCLUDED."FirstName",
        "LastName" = EXCLUDED."LastName",
        "OrganizationId" = EXCLUDED."OrganizationId",
        "UpdatedAt" = NOW();
    
    -- Organization Operator User
    -- Email: operator@test.com
    -- Password: Operator123!
    INSERT INTO "OrganizationUsers" (
        "Id",
        "SupabaseUserId",
        "Email",
        "FirstName",
        "LastName",
        "OrganizationId",
        "Status",
        "CreatedAt",
        "UpdatedAt"
    )
    VALUES (
        '770e8400-e29b-41d4-a716-446655440002'::uuid,
        NULL,
        'operator@test.com',
        'John',
        'Operator',
        '550e8400-e29b-41d4-a716-446655440001'::uuid,
        'Pending',
        NOW(),
        NOW()
    )
    ON CONFLICT (("Email"->'Value')) DO UPDATE SET
        "FirstName" = EXCLUDED."FirstName",
        "LastName" = EXCLUDED."LastName",
        "OrganizationId" = EXCLUDED."OrganizationId",
        "UpdatedAt" = NOW();
    
    RAISE NOTICE 'Organization users created/updated (pending Supabase Auth link)';
    
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
    
    RAISE NOTICE 'Sample machines created/updated';
    RAISE NOTICE 'Development seed completed successfully!';
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE WARNING 'Error during development seed: %', SQLERRM;
        -- Don't fail the migration, just log the error
END $$;
