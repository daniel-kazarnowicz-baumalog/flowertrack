# Schemat Bazy Danych PostgreSQL dla FLOWerTRACK

## 1. Lista Tabel

### 1.1. Użytkownicy i Role

#### `Users` (Tabela bazowa - koncepcyjna, implementowana przez `auth.users` w Supabase/PostgreSQL)
*   **id**: `UUID` (Primary Key) - Klucz główny, zgodny z `auth.users.id`.
*   **email**: `VARCHAR(255)` (Unique, Not Null)
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

#### `Roles` (Tabela słownikowa ról)
*   **id**: `SERIAL` (Primary Key)
*   **name**: `VARCHAR(50)` (Unique, Not Null) - Np. 'Serwisant', 'Serwisant-Admin', 'Operator', 'Administrator'

#### `UserRoles` (Tabela łącząca użytkowników z rolami)
*   **user_id**: `UUID` (Foreign Key to `Users(id)`)
*   **role_id**: `INTEGER` (Foreign Key to `Roles(id)`)
*   **Primary Key**: (`user_id`, `role_id`)

#### `ServiceUsers` (Profil użytkowników serwisu)
*   **user_id**: `UUID` (Primary Key, Foreign Key to `Users(id)`)
*   **first_name**: `VARCHAR(100)` (Not Null)
*   **last_name**: `VARCHAR(100)` (Not Null)
*   **phone_number**: `VARCHAR(50)`
*   **status**: `VARCHAR(50)` (Not Null) - Np. 'Aktywny', 'Nieaktywny'
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **updated_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

#### `OrganizationUsers` (Profil użytkowników klienta)
*   **user_id**: `UUID` (Primary Key, Foreign Key to `Users(id)`)
*   **first_name**: `VARCHAR(100)` (Not Null)
*   **last_name**: `VARCHAR(100)` (Not Null)
*   **organization_id**: `UUID` (Foreign Key to `Organizations(id)`, Not Null)
*   **status**: `VARCHAR(50)` (Not Null) - Np. 'Oczekuje na aktywację', 'Aktywny', 'Nieaktywny'
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **updated_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

### 1.2. Organizacje i Maszyny

#### `Organizations`
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **name**: `VARCHAR(255)` (Not Null)
*   **street**: `VARCHAR(255)`
*   **city**: `VARCHAR(100)`
*   **postal_code**: `VARCHAR(20)`
*   **country**: `VARCHAR(100)`
*   **service_status**: `VARCHAR(50)` (Not Null) - Np. 'Aktywny', 'Wstrzymany', 'Wygasły'
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **updated_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

#### `OrganizationContacts` (Kontakty biznesowe, niebędące użytkownikami)
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **organization_id**: `UUID` (Foreign Key to `Organizations(id)`, Not Null)
*   **first_name**: `VARCHAR(100)` (Not Null)
*   **last_name**: `VARCHAR(100)` (Not Null)
*   **email**: `VARCHAR(255)`
*   **phone**: `VARCHAR(50)`
*   **is_primary**: `BOOLEAN` (Not Null, Default: `false`)

#### `Machines`
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **organization_id**: `UUID` (Foreign Key to `Organizations(id)`, Not Null)
*   **serial_number**: `VARCHAR(255)` (Unique, Not Null)
*   **brand**: `VARCHAR(100)`
*   **model**: `VARCHAR(100)`
*   **location**: `VARCHAR(255)`
*   **status**: `VARCHAR(50)` (Not Null) - Np. 'Aktywna', 'Nieaktywna', 'Konserwacja', 'Alarm'
*   **api_token**: `VARCHAR(255)` (Unique, Not Null)
*   **last_maintenance_date**: `DATE`
*   **next_maintenance_date**: `DATE`
*   **maintenance_interval_id**: `INTEGER` (Foreign Key to `MaintenanceIntervals(id)`)
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **updated_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

### 1.3. Zgłoszenia (Tickets)

#### `Tickets`
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **ticket_number**: `SERIAL` (Unique, Not Null) - Numer widoczny dla użytkownika, np. TICK-1001
*   **title**: `VARCHAR(255)` (Not Null)
*   **description**: `TEXT`
*   **organization_id**: `UUID` (Foreign Key to `Organizations(id)`, Not Null)
*   **machine_id**: `UUID` (Foreign Key to `Machines(id)`, Not Null)
*   **status_id**: `INTEGER` (Foreign Key to `TicketStatuses(id)`, Not Null)
*   **priority_id**: `INTEGER` (Foreign Key to `TicketPriorities(id)`, Not Null)
*   **created_by_user_id**: `UUID` (Foreign Key to `Users(id)`, Not Null)
*   **assigned_to_user_id**: `UUID` (Foreign Key to `Users(id)`)
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **updated_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **resolved_at**: `TIMESTAMPTZ`
*   **closed_at**: `TIMESTAMPTZ`

#### `TicketHistory` (Oś czasu zgłoszenia)
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **ticket_id**: `UUID` (Foreign Key to `Tickets(id)`, Not Null)
*   **user_id**: `UUID` (Foreign Key to `Users(id)`) - Użytkownik, który wykonał akcję (może być `NULL` dla akcji systemowych)
*   **event_type**: `VARCHAR(50)` (Not Null) - Np. 'STATUS_CHANGE', 'COMMENT', 'ASSIGNMENT', 'ATTACHMENT_ADDED', 'NOTE'
*   **details**: `JSONB` - Szczegóły zdarzenia, np. `{ "from_status_id": 1, "to_status_id": 2, "reason": "..." }` lub `{ "comment_text": "..." }`
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

#### `Attachments` (Metadane załączników)
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **ticket_id**: `UUID` (Foreign Key to `Tickets(id)`, Not Null)
*   **uploaded_by_user_id**: `UUID` (Foreign Key to `Users(id)`, Not Null)
*   **file_name**: `VARCHAR(255)` (Not Null)
*   **file_path**: `VARCHAR(1024)` (Not Null) - Ścieżka w zewnętrznym storage
*   **file_size_bytes**: `BIGINT` (Not Null)
*   **mime_type**: `VARCHAR(100)` (Not Null)
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

### 1.4. Dane Słownikowe

#### `TicketStatuses`
*   **id**: `SERIAL` (Primary Key)
*   **name**: `VARCHAR(50)` (Unique, Not Null) - Np. 'Nowy', 'Przyjęty', 'W trakcie', 'Rozwiązany', 'Wznowiony', 'Zamknięty'

#### `TicketPriorities`
*   **id**: `SERIAL` (Primary Key)
*   **name**: `VARCHAR(50)` (Unique, Not Null) - Np. 'Niski', 'Średni', 'Wysoki', 'Krytyczny'

#### `MaintenanceIntervals`
*   **id**: `SERIAL` (Primary Key)
*   **name**: `VARCHAR(50)` (Unique, Not Null) - Np. 'Miesiąc', 'Kwartał', 'Pół roku', 'Rok'
*   **interval_days**: `INTEGER` (Not Null) - Liczba dni dla interwału

### 1.5. Logi i Audyt

#### `MachineLogs`
*   **id**: `UUID` (Primary Key, Default: `gen_random_uuid()`)
*   **machine_id**: `UUID` (Foreign Key to `Machines(id)`, Not Null)
*   **received_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)
*   **log_content**: `JSONB` (Not Null) - Surowa treść logu z maszyny

#### `AuditLog`
*   **id**: `BIGSERIAL` (Primary Key)
*   **user_id**: `UUID` (Foreign Key to `Users(id)`)
*   **action_type**: `VARCHAR(100)` (Not Null) - Np. 'USER_LOGIN', 'TICKET_CREATE', 'ORGANIZATION_UPDATE'
*   **target_resource_id**: `VARCHAR(255)` - ID zasobu, którego dotyczy akcja (np. ID ticketu, ID użytkownika)
*   **details**: `JSONB` - Szczegóły, np. `{"old_value": ..., "new_value": ...}`
*   **ip_address**: `INET`
*   **user_agent**: `TEXT`
*   **created_at**: `TIMESTAMPTZ` (Not Null, Default: `now()`)

---

## 2. Relacje Między Tabelami

*   **Users ↔ Roles**: Wiele-do-wielu przez tabelę `UserRoles`.
*   **Users → ServiceUsers**: Jeden-do-jednego (lub zero).
*   **Users → OrganizationUsers**: Jeden-do-jednego (lub zero).
*   **Organizations → OrganizationUsers**: Jeden-do-wielu.
*   **Organizations → OrganizationContacts**: Jeden-do-wielu.
*   **Organizations → Machines**: Jeden-do-wielu.
*   **Organizations → Tickets**: Jeden-do-wielu.
*   **Machines → Tickets**: Jeden-do-wielu.
*   **Machines → MachineLogs**: Jeden-do-wielu.
*   **MaintenanceIntervals → Machines**: Jeden-do-wielu.
*   **Users → Tickets**:
    *   Jeden-do-wielu (jako `created_by_user_id`).
    *   Jeden-do-wielu (jako `assigned_to_user_id`).
*   **TicketStatuses → Tickets**: Jeden-do-wielu.
*   **TicketPriorities → Tickets**: Jeden-do-wielu.
*   **Tickets → TicketHistory**: Jeden-do-wielu.
*   **Tickets → Attachments**: Jeden-do-wielu.
*   **Users → TicketHistory**: Jeden-do-wielu.
*   **Users → Attachments**: Jeden-do-wielu.
*   **Users → AuditLog**: Jeden-do-wielu.

---

## 3. Indeksy

### `Tickets`
*   `idx_tickets_organization_id`: w kolumnie `organization_id`
*   `idx_tickets_machine_id`: w kolumnie `machine_id`
*   `idx_tickets_status_id`: w kolumnie `status_id`
*   `idx_tickets_priority_id`: w kolumnie `priority_id`
*   `idx_tickets_assigned_to_user_id`: w kolumnie `assigned_to_user_id`
*   `idx_tickets_created_at`: w kolumnie `created_at` (dla sortowania i filtrowania po dacie)

### `TicketHistory`
*   `idx_tickethistory_ticket_id`: w kolumnie `ticket_id`
*   `idx_tickethistory_created_at`: w kolumnie `created_at`

### `MachineLogs`
*   `idx_machinelogs_machine_id`: w kolumnie `machine_id`
*   `idx_machinelogs_received_at`: w kolumnie `received_at` (kluczowe dla filtrowania po 90 dniach)

### `AuditLog`
*   `idx_auditlog_user_id`: w kolumnie `user_id`
*   `idx_auditlog_action_type`: w kolumnie `action_type`
*   `idx_auditlog_created_at`: w kolumnie `created_at`

### Inne
*   `idx_machines_organization_id`: w kolumnie `organization_id`
*   `idx_organizationusers_organization_id`: w kolumnie `organization_id`

---

## 4. Zasady PostgreSQL (Row-Level Security - RLS)

*RLS powinien być włączony (`ENABLED`) dla wszystkich tabel przechowujących dane biznesowe.*

### `Tickets`
*   **Zasada dla Operatora Klienta**: Użytkownik z rolą 'Operator' może odczytać (`SELECT`) zgłoszenie, jeśli `created_by_user_id` jest równe jego `id`.
*   **Zasada dla Administratora Klienta**: Użytkownik z rolą 'Administrator' może odczytać (`SELECT`) zgłoszenie, jeśli `organization_id` jest zgodne z `id` jego organizacji. Może modyfikować (`UPDATE`) zgłoszenia swojej organizacji.
*   **Zasada dla Serwisanta/Admina Serwisu**: Użytkownik z rolą 'Serwisant' lub 'Serwisant-Admin' ma pełny dostęp (`ALL`) do wszystkich zgłoszeń.

### `Organizations`
*   **Zasada dla Użytkowników Klienta**: Użytkownik (Operator/Administrator) może odczytać (`SELECT`) tylko dane swojej organizacji (`id` organizacji zgodne z `organization_id` w jego profilu `OrganizationUsers`).
*   **Zasada dla Serwisantów**: Pełny dostęp (`ALL`).

### `Machines`
*   **Zasada dla Użytkowników Klienta**: Użytkownik może odczytać (`SELECT`) maszyny należące do jego organizacji.
*   **Zasada dla Serwisantów**: Pełny dostęp (`ALL`).

### `Attachments`, `TicketHistory`
*   Dostęp do tych tabel powinien być dziedziczony na podstawie dostępu do powiązanego zgłoszenia (`Tickets`). Użytkownik może odczytać (`SELECT`) wpis, jeśli ma dostęp do `ticket_id`.

---

## 5. Dodatkowe Uwagi

1.  **Podejście Code-First**: Schemat został zaprojektowany z myślą o podejściu "code-first" z użyciem Entity Framework Core. Tabele słownikowe (`TicketStatuses`, `TicketPriorities`, `MaintenanceIntervals`, `Roles`) powinny być inicjowane danymi (data seeding) poprzez migracje EF Core.
2.  **Uwierzytelnianie**: Schemat zakłada wykorzystanie zewnętrznego dostawcy uwierzytelniania (jak Supabase Auth), który zarządza tabelą `auth.users`. Tabele `ServiceUsers` i `OrganizationUsers` pełnią rolę tabel profilowych, rozszerzając dane o użytkownikach. Relacje z innymi tabelami powinny używać klucza `user_id` typu `UUID`.
3.  **Kolejka Wiadomości**: Przetwarzanie logów z maszyn (`MachineLogs`) powinno być realizowane asynchronicznie z użyciem kolejki wiadomości, aby odciążyć główną aplikację. Endpoint API jedynie przyjmuje log i umieszcza go w kolejce, a osobny proces (worker) przetwarza go i zapisuje do bazy.
4.  **Denormalizacja**: Zgodnie z decyzjami, schemat unika denormalizacji. Wartości takie jak liczba zgłoszeń serwisanta będą obliczane dynamicznie. Jedynym wyjątkiem jest pole `next_maintenance_date` w tabeli `Machines`, które jest obliczane, ale jego przechowywanie jest uzasadnione dla uproszczenia zapytań o nadchodzące przeglądy.
5.  **Typ `JSONB`**: Użycie typu `JSONB` dla `details` i `log_content` zapewnia elastyczność oraz możliwość wydajnego indeksowania i przeszukiwania danych w formacie JSON w przyszłości.