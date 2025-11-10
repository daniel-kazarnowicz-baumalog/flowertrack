-- =====================================================
-- KROK 1: Utwórz użytkowników w Supabase Auth
-- =====================================================
-- Uruchom ten SQL w Supabase Studio (http://127.0.0.1:54323)
-- SQL Editor → New Query → Wklej i uruchom (Run)

-- Najpierw sprawdź czy użytkownicy już istnieją
SELECT id, email, confirmed_at, created_at 
FROM auth.users 
WHERE email IN ('admin@flowertrack.dev', 'tech@flowertrack.dev', 'client@test.com', 'operator@test.com');

-- Jeśli powyższe zapytanie zwraca puste wyniki, wykonaj poniższy kod:

-- UWAGA: Supabase lokalny nie ma funkcji auth.create_user dostępnej przez SQL
-- Użytkownicy muszą być utworzeni przez Supabase Dashboard UI lub Auth API

-- Alternatywnie, możesz ręcznie dodać do tabeli auth.users (tylko dla lokalnego developmentu):

-- Service Admin
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
) VALUES (
    '00000000-0000-0000-0000-000000000000',
    gen_random_uuid(),
    'authenticated',
    'authenticated',
    'admin@flowertrack.dev',
    crypt('Admin123!', gen_salt('bf')), -- BCrypt hash
    NOW(),
    NOW(),
    NOW(),
    '{"provider":"email","providers":["email"]}',
    '{}',
    NOW(),
    NOW(),
    '',
    '',
    '',
    ''
) ON CONFLICT (email) DO NOTHING
RETURNING id, email;

-- Service Tech
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
) VALUES (
    '00000000-0000-0000-0000-000000000000',
    gen_random_uuid(),
    'authenticated',
    'authenticated',
    'tech@flowertrack.dev',
    crypt('Tech123!', gen_salt('bf')),
    NOW(),
    NOW(),
    NOW(),
    '{"provider":"email","providers":["email"]}',
    '{}',
    NOW(),
    NOW(),
    '',
    '',
    '',
    ''
) ON CONFLICT (email) DO NOTHING
RETURNING id, email;

-- Client Admin
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
) VALUES (
    '00000000-0000-0000-0000-000000000000',
    gen_random_uuid(),
    'authenticated',
    'authenticated',
    'client@test.com',
    crypt('Client123!', gen_salt('bf')),
    NOW(),
    NOW(),
    NOW(),
    '{"provider":"email","providers":["email"]}',
    '{}',
    NOW(),
    NOW(),
    '',
    '',
    '',
    ''
) ON CONFLICT (email) DO NOTHING
RETURNING id, email;

-- Client Operator
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
) VALUES (
    '00000000-0000-0000-0000-000000000000',
    gen_random_uuid(),
    'authenticated',
    'authenticated',
    'operator@test.com',
    crypt('Operator123!', gen_salt('bf')),
    NOW(),
    NOW(),
    NOW(),
    '{"provider":"email","providers":["email"]}',
    '{}',
    NOW(),
    NOW(),
    '',
    '',
    '',
    ''
) ON CONFLICT (email) DO NOTHING
RETURNING id, email;
