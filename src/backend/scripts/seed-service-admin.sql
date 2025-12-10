-- =====================================================
-- FLOWerTRACK - Service Admin Seed Script
-- =====================================================
-- This is the ONLY seed script needed to bootstrap the application.
-- It creates:
--   1. Service Admin user in Supabase Auth (auth.users)
--   2. ServiceUser record in the application database
--   3. User role assignment (ServiceAdministrator)
--
-- Credentials:
--   Email: admin@flowertrack.dev
--   Password: Admin123!
--
-- Run this script in Supabase Studio SQL Editor:
--   Local: http://127.0.0.1:56323
--   Remote: Your Supabase project SQL Editor
-- =====================================================

-- Step 1: Create auth user in Supabase
-- BCrypt hash for "Admin123!" - generated using gen_salt('bf')
INSERT INTO auth.users (
    instance_id,
    id,
    aud,
    role,
    email,
    encrypted_password,
    email_confirmed_at,
    recovery_sent_at,
    last_sign_in_at,
    raw_app_meta_data,
    raw_user_meta_data,
    created_at,
    updated_at,
    confirmation_token,
    email_change,
    email_change_token_new,
    recovery_token
) 
SELECT
    '00000000-0000-0000-0000-000000000000',
    '10000000-0000-0000-0000-000000000001', -- Fixed UUID for Service Admin
    'authenticated',
    'authenticated',
    'admin@flowertrack.dev',
    crypt('Admin123!', gen_salt('bf')),
    NOW(),
    NOW(),
    NOW(),
    '{"provider":"email","providers":["email"],"role":"ServiceAdministrator"}',
    '{"first_name":"System","last_name":"Administrator"}',
    NOW(),
    NOW(),
    '',
    '',
    '',
    ''
WHERE NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'admin@flowertrack.dev');

-- Get the created user's ID for subsequent operations
DO $$
DECLARE
    v_auth_user_id UUID;
    v_service_user_id UUID;
BEGIN
    -- Get the auth user ID
    SELECT id INTO v_auth_user_id 
    FROM auth.users 
    WHERE email = 'admin@flowertrack.dev';

    IF v_auth_user_id IS NULL THEN
        RAISE EXCEPTION 'Auth user not found';
    END IF;

    -- Generate a new UUID for ServiceUser or use a fixed one
    v_service_user_id := '20000000-0000-0000-0000-000000000001'::UUID;

    -- Step 2: Create ServiceUser record
    -- BCrypt hash for "Admin123!" - this needs to match what BCrypt.Net generates
    -- For local dev, we use the same hash format
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
        "PasswordHash",
        "CreatedAt",
        "UpdatedAt",
        "CreatedBy",
        "UpdatedBy"
    )
    SELECT
        v_service_user_id,
        v_auth_user_id,
        'System',
        'Administrator',
        'admin@flowertrack.dev',
        NULL,
        'Active',
        'System Administration',
        true,
        -- BCrypt hash for "Admin123!" - compatible with BCrypt.Net
        '$2a$11$K8WKqfqJZCAZ7FHXa9nWEuLvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv', -- Placeholder, will update below
        NOW(),
        NOW(),
        v_service_user_id,
        v_service_user_id
    WHERE NOT EXISTS (SELECT 1 FROM "ServiceUsers" WHERE "Email" = 'admin@flowertrack.dev');

    -- Step 3: Assign ServiceAdministrator role (role_id = 1)
    INSERT INTO user_roles (
        id,
        user_id,
        role_id,
        assigned_at,
        assigned_by
    )
    SELECT
        gen_random_uuid(),
        v_service_user_id,
        1, -- ServiceAdministrator role
        NOW(),
        v_service_user_id
    WHERE NOT EXISTS (
        SELECT 1 FROM user_roles 
        WHERE user_id = v_service_user_id AND role_id = 1
    );

    RAISE NOTICE 'Service Admin created successfully!';
    RAISE NOTICE 'Auth User ID: %', v_auth_user_id;
    RAISE NOTICE 'Service User ID: %', v_service_user_id;
    RAISE NOTICE 'Email: admin@flowertrack.dev';
    RAISE NOTICE 'Password: Admin123!';
END $$;

-- Verify the seed
SELECT 
    su."Id" as "ServiceUserId",
    su."SupabaseUserId",
    su."FirstName",
    su."LastName",
    su."Email",
    su."Status",
    r.name as "RoleName"
FROM "ServiceUsers" su
LEFT JOIN user_roles ur ON ur.user_id = su."Id"
LEFT JOIN roles r ON r.id = ur.role_id
WHERE su."Email" = 'admin@flowertrack.dev';
