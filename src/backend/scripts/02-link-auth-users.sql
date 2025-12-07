-- =====================================================
-- KROK 2: Powiąż użytkowników auth.users z tabelami aplikacji
-- =====================================================
-- Uruchom ten SQL w Supabase Studio (http://127.0.0.1:56323)
-- SQL Editor → New Query → Wklej i uruchom (Run)
-- 
-- WAŻNE: Uruchom NAJPIERW skrypt 01-create-auth-users.sql
-- =====================================================

-- Sprawdź użytkowników w auth.users
SELECT id, email FROM auth.users 
WHERE email IN ('admin@flowertrack.dev', 'tech@flowertrack.dev', 'client@test.com', 'operator@test.com');

-- =====================================================
-- 1. Powiąż Organization Users z auth.users
-- =====================================================

-- Aktualizuj client@test.com
UPDATE "OrganizationUsers" ou
SET "SupabaseUserId" = au.id,
    "IsActivated" = true,
    "UpdatedAt" = NOW()
FROM auth.users au
WHERE au.email = 'client@test.com'
  AND ou."Email" = 'client@test.com';

-- Aktualizuj operator@test.com  
UPDATE "OrganizationUsers" ou
SET "SupabaseUserId" = au.id,
    "IsActivated" = true,
    "UpdatedAt" = NOW()
FROM auth.users au
WHERE au.email = 'operator@test.com'
  AND ou."Email" = 'operator@test.com';

-- =====================================================
-- 2. Powiąż Service Users z auth.users
-- =====================================================

-- Aktualizuj admin@flowertrack.dev
UPDATE "ServiceUsers" su
SET "SupabaseUserId" = au.id,
    "UpdatedAt" = NOW()
FROM auth.users au
WHERE au.email = 'admin@flowertrack.dev'
  AND su."Email" = 'admin@flowertrack.dev';

-- Aktualizuj tech@flowertrack.dev
UPDATE "ServiceUsers" su
SET "SupabaseUserId" = au.id,
    "UpdatedAt" = NOW()
FROM auth.users au
WHERE au.email = 'tech@flowertrack.dev'
  AND su."Email" = 'tech@flowertrack.dev';

-- =====================================================
-- 3. Sprawdź powiązania
-- =====================================================

SELECT 'OrganizationUsers' as "Table", "Email", "SupabaseUserId", "IsActivated"
FROM "OrganizationUsers"
WHERE "Email" IN ('client@test.com', 'operator@test.com')

UNION ALL

SELECT 'ServiceUsers' as "Table", "Email", "SupabaseUserId", NULL as "IsActivated"
FROM "ServiceUsers"
WHERE "Email" IN ('admin@flowertrack.dev', 'tech@flowertrack.dev');
