# Tworzenie użytkowników testowych - Development Environment

## Problem

System używa Supabase Auth do autentykacji, więc nie możemy bezpośrednio dodawać użytkowników do bazy danych. Musimy najpierw utworzyć ich w Supabase Auth, a potem system automatycznie utworzy powiązane rekordy w tabelach domenowych.

## Rozwiązanie: Użyj Supabase Dashboard

### Opcja 1: Przez Supabase Dashboard UI (Zalecane dla Developmentu)

#### 1. Otwórz Supabase Local Studio
- URL: http://127.0.0.1:54323
- Lub: `npx supabase start` aby uruchomić lokalnie

#### 2. Utwórz użytkowników testowych

##### Service Portal Users:

**Admin User:**
1. Przejdź do **Authentication** → **Users**
2. Kliknij **Add user** → **Create new user**
3. Wypełnij:
   - Email: `admin@flowertrack.dev`
   - Password: `Admin123!`
   - Auto-confirm: **✓ YES**
4. Kliknij **Create user**

**Technician User:**
1. Kliknij **Add user** → **Create new user**
2. Wypełnij:
   - Email: `tech@flowertrack.dev`
   - Password: `Tech123!`
   - Auto-confirm: **✓ YES**
3. Kliknij **Create user**

##### Client Portal Users:

**Client Admin:**
1. Kliknij **Add user** → **Create new user**
2. Wypełnij:
   - Email: `client@test.com`
   - Password: `Client123!`
   - Auto-confirm: **✓ YES**
3. Kliknij **Create user**

**Client Operator:**
1. Kliknij **Add user** → **Create new user**
2. Wypełnij:
   - Email: `operator@test.com`
   - Password: `Operator123!`
   - Auto-confirm: **✓ YES**
3. Kliknij **Create user**

#### 3. Powiąż z domeną (SQL)

Po utworzeniu użytkowników w Supabase Auth, uruchom ten SQL w **SQL Editor**:

```sql
-- Pobierz ID użytkowników z Supabase Auth
DO $$
DECLARE
    admin_supabase_id uuid;
    tech_supabase_id uuid;
    client_supabase_id uuid;
    operator_supabase_id uuid;
    test_org_id uuid := '550e8400-e29b-41d4-a716-446655440001'::uuid;
BEGIN
    -- Najpierw utwórz organizację testową
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
        test_org_id,
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

    -- Pobierz Supabase User IDs
    SELECT id INTO admin_supabase_id FROM auth.users WHERE email = 'admin@flowertrack.dev';
    SELECT id INTO tech_supabase_id FROM auth.users WHERE email = 'tech@flowertrack.dev';
    SELECT id INTO client_supabase_id FROM auth.users WHERE email = 'client@test.com';
    SELECT id INTO operator_supabase_id FROM auth.users WHERE email = 'operator@test.com';

    -- Sprawdź czy znaleziono użytkowników
    IF admin_supabase_id IS NULL THEN
        RAISE WARNING 'Admin user not found in auth.users. Please create admin@flowertrack.dev in Supabase Auth first.';
    END IF;
    IF tech_supabase_id IS NULL THEN
        RAISE WARNING 'Tech user not found in auth.users. Please create tech@flowertrack.dev in Supabase Auth first.';
    END IF;
    IF client_supabase_id IS NULL THEN
        RAISE WARNING 'Client user not found in auth.users. Please create client@test.com in Supabase Auth first.';
    END IF;
    IF operator_supabase_id IS NULL THEN
        RAISE WARNING 'Operator user not found in auth.users. Please create operator@test.com in Supabase Auth first.';
    END IF;

    -- Utwórz Service Users (jeśli znaleziono w auth.users)
    IF admin_supabase_id IS NOT NULL THEN
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
            gen_random_uuid(),
            admin_supabase_id,
            'Admin',
            'User',
            'admin@flowertrack.dev',
            '+48 123 456 001',
            'Active',
            'Administrator',
            true,
            NOW(),
            NOW()
        )
        ON CONFLICT ("SupabaseUserId") DO UPDATE SET
            "Status" = 'Active',
            "IsAvailable" = true,
            "UpdatedAt" = NOW();
        RAISE NOTICE 'Admin user linked successfully';
    END IF;

    IF tech_supabase_id IS NOT NULL THEN
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
            gen_random_uuid(),
            tech_supabase_id,
            'Tech',
            'Support',
            'tech@flowertrack.dev',
            '+48 123 456 002',
            'Active',
            'Technical Support',
            true,
            NOW(),
            NOW()
        )
        ON CONFLICT ("SupabaseUserId") DO UPDATE SET
            "Status" = 'Active',
            "IsAvailable" = true,
            "UpdatedAt" = NOW();
        RAISE NOTICE 'Tech user linked successfully';
    END IF;

    -- Utwórz Organization Users (jeśli znaleziono w auth.users)
    IF client_supabase_id IS NOT NULL THEN
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
            gen_random_uuid(),
            client_supabase_id,
            'client@test.com',
            'Client',
            'Admin',
            test_org_id,
            'Active',
            NOW(),
            NOW()
        )
        ON CONFLICT ("SupabaseUserId") DO UPDATE SET
            "Status" = 'Active',
            "UpdatedAt" = NOW();
        RAISE NOTICE 'Client admin linked successfully';
    END IF;

    IF operator_supabase_id IS NOT NULL THEN
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
            gen_random_uuid(),
            operator_supabase_id,
            'operator@test.com',
            'John',
            'Operator',
            test_org_id,
            'Active',
            NOW(),
            NOW()
        )
        ON CONFLICT ("SupabaseUserId") DO UPDATE SET
            "Status" = 'Active',
            "UpdatedAt" = NOW();
        RAISE NOTICE 'Operator linked successfully';
    END IF;

    -- Utwórz przykładowe maszyny
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
        test_org_id,
        'Active',
        NOW(),
        NOW()
    ),
    (
        '880e8400-e29b-41d4-a716-446655440002'::uuid,
        'Baumalog',
        'FlowMaster 3000',
        'FM3000-002',
        test_org_id,
        'Active',
        NOW(),
        NOW()
    )
    ON CONFLICT ("SerialNumber") DO UPDATE SET
        "Brand" = EXCLUDED."Brand",
        "Model" = EXCLUDED."Model",
        "Status" = EXCLUDED."Status",
        "UpdatedAt" = NOW();
    RAISE NOTICE 'Sample machines created successfully';

    RAISE NOTICE '✅ Test environment setup complete!';
END $$;
```

---

### Opcja 2: Przez Supabase CLI (dla automatyzacji)

```powershell
# Utwórz użytkowników przez Supabase Auth API
# TODO: Dodać skrypt automatyzujący gdy będzie potrzebny
```

---

## Weryfikacja

Po wykonaniu powyższych kroków, sprawdź czy użytkownicy są dostępni:

```sql
-- Sprawdź użytkowników w Supabase Auth
SELECT id, email, confirmed_at, created_at 
FROM auth.users 
WHERE email IN ('admin@flowertrack.dev', 'tech@flowertrack.dev', 'client@test.com', 'operator@test.com');

-- Sprawdź ServiceUsers
SELECT "Id", "SupabaseUserId", "FirstName", "LastName", "Email", "Status", "IsAvailable"
FROM "ServiceUsers";

-- Sprawdź OrganizationUsers
SELECT "Id", "SupabaseUserId", "FirstName", "LastName", "Email", "Status", "OrganizationId"
FROM "OrganizationUsers";

-- Sprawdź Machines
SELECT "Id", "Brand", "Model", "SerialNumber", "Status" 
FROM "Machines";
```

---

## Dane logowania

Po poprawnym utworzeniu użytkowników, możesz się zalogować:

### Service Portal (http://localhost:5173/service/login)
| Email | Hasło | Rola |
|-------|-------|------|
| admin@flowertrack.dev | Admin123! | Administrator |
| tech@flowertrack.dev | Tech123! | Technical Support |

### Client Portal (http://localhost:5173/client/login)
| Email | Hasło | Rola |
|-------|-------|------|
| client@test.com | Client123! | Client Admin |
| operator@test.com | Operator123! | Operator |

---

## Troubleshooting

### "Service user not found. Please contact administrator."
- Użytkownik istnieje w Supabase Auth ale nie ma powiązanego rekordu w `ServiceUsers` lub `OrganizationUsers`
- Rozwiązanie: Uruchom ponownie SQL z kroku 3

### "Your account is not active. Please contact administrator."
- Status użytkownika to 'Pending' zamiast 'Active'
- Rozwiązanie: Zaktualizuj status przez SQL:
```sql
UPDATE "ServiceUsers" SET "Status" = 'Active', "IsAvailable" = true WHERE "Email"->'Value' = 'admin@flowertrack.dev';
```

### Lokalny Supabase nie działa
```powershell
cd src/backend
npx supabase start
```

Jeśli port jest zajęty:
```powershell
npx supabase stop
npx supabase start
```
