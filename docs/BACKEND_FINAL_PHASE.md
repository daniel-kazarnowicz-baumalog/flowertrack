# Backend Final Phase - Kompletny Plan Ukończenia

**Data utworzenia:** 2025-12-05  
**Status:** Plan ukończenia backendu  
**Cel:** Zdefiniowanie wszystkich brakujących elementów wymaganych do uznania backendu za gotowy do produkcji

---

## 📊 Podsumowanie Stanu Obecnego

### ✅ Ukończone Komponenty

| Warstwa | Komponent | Status |
|---------|-----------|--------|
| Domain | Ticket Entity + Events | ✅ Gotowe |
| Domain | Machine Entity + Events | ✅ Gotowe |
| Domain | Organization Entity + Events | ✅ Gotowe |
| Domain | ServiceUser/OrganizationUser Entities | ✅ Gotowe |
| Domain | TicketComment Entity | ✅ Gotowe |
| Domain | TicketAttachment Entity | ✅ Gotowe |
| Domain | Value Objects (Email, TicketNumber, MachineApiKey) | ✅ Gotowe |
| Domain | Repository Interfaces | ✅ Gotowe |
| Infrastructure | ApplicationDbContext + Migrations | ✅ Gotowe |
| Infrastructure | Repository Implementations | ✅ Gotowe |
| Infrastructure | Supabase Auth Integration | ✅ Gotowe |
| Infrastructure | Email Service | ✅ Gotowe (stub) |
| Infrastructure | File Storage Service | ✅ Gotowe |
| Application | Ticket Commands/Queries | ✅ Gotowe |
| Application | Machine Commands/Queries | ✅ Gotowe |
| Application | Organization Commands/Queries | ✅ Gotowe |
| Application | User Management Commands/Queries | ✅ Gotowe |
| Application | Comments Commands/Queries | ✅ Gotowe |
| Application | Attachments Commands/Queries | ✅ Gotowe |
| API | TicketsController (8 endpoints) | ✅ Gotowe |
| API | MachinesController | ✅ Gotowe |
| API | OrganizationsController | ✅ Gotowe |
| API | UsersController | ✅ Gotowe |
| API | AuthController | ✅ Gotowe |
| API | TicketAttachmentsController | ✅ Gotowe |
| API | Health Checks | ✅ Gotowe |
| API | Global Exception Handler | ✅ Gotowe |
| API | Serilog Logging | ✅ Gotowe |
| API | CORS Configuration | ✅ Gotowe |
| Security | JWT Authentication | ✅ Gotowe |
| Security | Authorization Policies | ✅ Gotowe |
| Security | RLS Policies (SQL Scripts) | ✅ Gotowe |

---

## 🚨 Brakujące Komponenty (Must Have dla MVP)

### 1. Domain Layer

#### 1.1. MachineLogs Entity ❌
**Priorytet:** 🔴 Krytyczny  
**Opis:** Encja do przechowywania logów z maszyn wysyłanych przez API token  
**Pliki do utworzenia:**
- `Flowertrack.Domain/Entities/MachineLog.cs`
- `Flowertrack.Domain/Events/MachineLogReceivedEvent.cs`
- `Flowertrack.Domain/Repositories/IMachineLogRepository.cs`

```csharp
// MachineLog.cs
public sealed class MachineLog : AuditableEntity<Guid>
{
    public Guid MachineId { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public string LogContent { get; private set; } // JSON
    public string? Status { get; private set; } // ALARM, NORMAL, WARNING
    public string? AlarmCode { get; private set; }
}
```

**Wymagania z PRD:**
- US-028: Generowanie tokenu API dla maszyn
- US-029: Przeglądanie statusów maszyn i alertów

---

#### 1.2. AuditLog Entity ❌
**Priorytet:** 🔴 Krytyczny (wymagane przez US-054)  
**Opis:** Encja do pełnego audytu wszystkich akcji użytkowników  
**Pliki do utworzenia:**
- `Flowertrack.Domain/Entities/AuditLog.cs`
- `Flowertrack.Domain/Repositories/IAuditLogRepository.cs`

```csharp
// AuditLog.cs
public sealed class AuditLog
{
    public long Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string ActionType { get; private set; }
    public string? TargetResourceId { get; private set; }
    public string? OldValue { get; private set; }  // JSON
    public string? NewValue { get; private set; }  // JSON
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
```

---

#### 1.3. OrganizationContact Entity ❌
**Priorytet:** 🟡 Średni  
**Opis:** Kontakty biznesowe organizacji (nie będące użytkownikami systemu)  
**Pliki do utworzenia:**
- `Flowertrack.Domain/Entities/OrganizationContact.cs`

---

### 2. Infrastructure Layer

#### 2.1. MachineLog Repository & Configuration ❌
**Priorytet:** 🔴 Krytyczny  
**Pliki do utworzenia:**
- `Flowertrack.Infrastructure/Persistence/Configurations/MachineLogConfiguration.cs`
- `Flowertrack.Infrastructure/Persistence/Repositories/MachineLogRepository.cs`
- Migracja EF Core

---

#### 2.2. AuditLog Repository & Service ❌
**Priorytet:** 🔴 Krytyczny  
**Pliki do utworzenia:**
- `Flowertrack.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs`
- `Flowertrack.Infrastructure/Persistence/Repositories/AuditLogRepository.cs`
- `Flowertrack.Infrastructure/Services/AuditService.cs`
- `Flowertrack.Application/Common/Interfaces/IAuditService.cs`

**Funkcjonalność:**
- Automatyczne logowanie wszystkich akcji użytkowników
- Middleware do zbierania IP/UserAgent
- Możliwość filtrowania logów audytu

---

#### 2.3. Produkcyjna implementacja EmailService ❌
**Priorytet:** 🟡 Średni (dla MVP można użyć stub)  
**Opis:** Obecna implementacja EmailService jest stubem - loguje tylko do konsoli  
**Wymagania:**
- Integracja z SMTP lub usługą jak SendGrid/Resend
- Szablony email dla:
  - Zaproszenie serwisanta (US-032)
  - Zaproszenie operatora organizacji (US-050)
  - Reset hasła (US-002)
  - Aktywacja konta (US-005)

---

### 3. Application Layer

#### 3.1. Machine Logs Ingestion Command ❌
**Priorytet:** 🔴 Krytyczny  
**Pliki do utworzenia:**
- `Flowertrack.Application/MachineLogs/Commands/IngestMachineLog/IngestMachineLogCommand.cs`
- `Flowertrack.Application/MachineLogs/Commands/IngestMachineLog/IngestMachineLogCommandHandler.cs`
- `Flowertrack.Application/MachineLogs/Queries/GetMachineLogs/GetMachineLogsQuery.cs` (już istnieje, weryfikacja)

**Logika:**
1. Walidacja tokenu API maszyny
2. Parsowanie JSON logu
3. Aktualizacja statusu maszyny jeśli zawiera alarm
4. Zapis logu do bazy

---

#### 3.2. Audit Log Queries ❌
**Priorytet:** 🔴 Krytyczny  
**Pliki do utworzenia:**
- `Flowertrack.Application/Audit/Queries/GetAuditLogs/GetAuditLogsQuery.cs`
- `Flowertrack.Application/Audit/Queries/GetAuditLogs/GetAuditLogsQueryHandler.cs`

**Filtry:**
- Po użytkowniku
- Po typie akcji
- Po zasobie
- Po dacie

---

#### 3.3. Ticket History Query Enhancement ❌
**Priorytet:** 🟡 Średni  
**Opis:** Pełna oś czasu zgłoszenia (US-016)  
**Wymagania:**
- Wszystkie zmiany statusów z uzasadnieniami
- Komentarze i notatki wewnętrzne
- Załączniki z informacją o pobraniach
- Przypisania

---

#### 3.4. Dashboard KPI Queries ❌
**Priorytet:** 🔴 Krytyczny (US-006, US-007, US-037)  
**Pliki do utworzenia:**
- `Flowertrack.Application/Dashboard/Queries/GetServiceDashboard/`
- `Flowertrack.Application/Dashboard/Queries/GetOrganizationDashboard/`

**Dane zwracane:**
```csharp
public record ServiceDashboardDto
{
    public int TotalActiveTickets { get; init; }
    public int CriticalTickets { get; init; }
    public int HighPriorityTickets { get; init; }
    public int ResolvedLast24h { get; init; }
    public int ClosedLast24h { get; init; }
    public List<RecentActivityDto> RecentActivities { get; init; }
    public List<OrganizationAlarmDto> OrganizationsWithAlarms { get; init; }
    public List<UpcomingMaintenanceDto> UpcomingMaintenances { get; init; }
}

public record OrganizationDashboardDto
{
    public int ActiveMachines { get; init; }
    public int MachinesWithAlarms { get; init; }
    public int MachinesInMaintenance { get; init; }
    public int MyActiveTickets { get; init; }
    public int AllOrganizationTickets { get; init; }
    public List<RecentActivityDto> RecentActivities { get; init; }
}
```

---

#### 3.5. Grouped Tickets Enhancement ❌
**Priorytet:** 🟢 Niski (opcjonalne dla MVP)  
**Opis:** US-013 - Grupowanie zgłoszeń wg statusu ze zwijaniem sekcji  
**Query już istnieje, weryfikacja czy spełnia wymagania**

---

#### 3.6. Mass Actions Commands ❌
**Priorytet:** 🟡 Średni (US-014)  
**Pliki do utworzenia:**
- `Flowertrack.Application/Tickets/Commands/BulkAssignTickets/`
- `Flowertrack.Application/Tickets/Commands/BulkChangeStatus/`

---

#### 3.7. Export Ticket History ❌
**Priorytet:** 🟢 Niski (US-022)  
**Pliki do utworzenia:**
- `Flowertrack.Application/Tickets/Queries/ExportTicketHistory/`

**Formaty:**
- PDF (z logo, formatowanie)
- CSV
- JSON

---

### 4. Presentation Layer (API)

#### 4.1. Machine Logs Ingestion Endpoint ❌
**Priorytet:** 🔴 Krytyczny  
**Endpoint:**
```
POST /api/ingest/logs
Header: X-API-Token: <machine_api_token>
Body: { timestamp, status, telemetry, alarms }
Response: 202 Accepted
```

**Uwagi:**
- Autentykacja przez token API, nie JWT
- Rate limiting (ochrona przed DDoS)
- Async processing (kolejka lub fire-and-forget)

---

#### 4.2. Dashboard Controller ❌
**Priorytet:** 🔴 Krytyczny  
**Endpoints:**
```
GET /api/dashboard/service     - Dashboard serwisu
GET /api/dashboard/organization - Dashboard organizacji
```

---

#### 4.3. Audit Log Controller ❌
**Priorytet:** 🔴 Krytyczny  
**Endpoints:**
```
GET /api/admin/audit-logs      - Lista logów (tylko Service Admin)
GET /api/admin/audit-logs/export - Eksport CSV/JSON
```

---

#### 4.4. Comments Controller Update ❌
**Priorytet:** 🟡 Średni  
**Opis:** Dodać endpoint do komentarzy jako oddzielne ścieżki lub wewnątrz TicketsController  
**Obecnie:** Comments są obsługiwane przez TicketsController  
**Wymagania:**
- Weryfikacja limitu edycji (15 minut dla komentarzy, 5 minut dla notatek)
- Rozróżnienie publicznych/wewnętrznych komentarzy

---

### 5. Database Migrations

#### 5.1. Brakujące Migracje ❌
**Priorytet:** 🔴 Krytyczny  
- [ ] MachineLogs table
- [ ] AuditLog table
- [ ] OrganizationContacts table
- [ ] Indeksy wydajnościowe zgodnie z db-plan.md

---

### 6. Security

#### 6.1. Machine Token Authentication ❌
**Priorytet:** 🔴 Krytyczny  
**Opis:** Mechanizm walidacji tokenu API dla endpointu /api/ingest/logs  
**Pliki do utworzenia:**
- `Flowertrack.Api/Middleware/MachineTokenAuthenticationMiddleware.cs` lub
- Custom authentication handler

---

#### 6.2. Rate Limiting ❌
**Priorytet:** 🟡 Średni  
**Opis:** Ochrona przed nadużyciami API  
**Biblioteka:** `Microsoft.AspNetCore.RateLimiting`

---

#### 6.3. Input Validation Enhancement ❌
**Priorytet:** 🟡 Średni  
**Opis:** Dodatkowa walidacja dla wszystkich endpointów z FluentValidation

---

### 7. Testing

#### 7.1. Unit Tests Coverage ❌
**Priorytet:** 🔴 Krytyczny  
**Obecny stan:** Domain tests ~232 testy  
**Wymagane:**
- [ ] Application Layer handlers tests
- [ ] Validation tests
- [ ] Integration tests z bazą testową

---

#### 7.2. Integration Tests ❌
**Priorytet:** 🔴 Krytyczny  
**Wymagane:**
- [ ] API endpoint tests
- [ ] Authentication/Authorization tests
- [ ] Database integration tests

---

### 8. Documentation

#### 8.1. Swagger/OpenAPI Enhancement ❌
**Priorytet:** 🟡 Średni  
**Wymagania:**
- Pełna dokumentacja wszystkich endpointów
- Przykłady request/response
- Opisy błędów

---

#### 8.2. API Documentation ❌
**Priorytet:** 🟢 Niski  
**Plik:** `docs/API_DOCUMENTATION.md`

---

## 📋 Checklist Ukończenia Backendu

### Warstwa Domain
- [ ] MachineLogs Entity
- [ ] AuditLog Entity
- [ ] OrganizationContact Entity (opcjonalne)
- [ ] Wszystkie encje mają testy jednostkowe

### Warstwa Infrastructure
- [ ] MachineLogs Repository + Configuration
- [ ] AuditLog Repository + Configuration
- [ ] AuditService implementation
- [ ] Email Service produkcyjna (lub stub z konfiguracją)
- [ ] Migracje EF Core dla wszystkich encji

### Warstwa Application
- [ ] IngestMachineLogCommand
- [ ] GetAuditLogsQuery
- [ ] GetServiceDashboardQuery
- [ ] GetOrganizationDashboardQuery
- [ ] BulkAssignTicketsCommand (opcjonalne)
- [ ] BulkChangeStatusCommand (opcjonalne)
- [ ] ExportTicketHistoryQuery (opcjonalne)

### Warstwa API
- [ ] POST /api/ingest/logs
- [ ] GET /api/dashboard/service
- [ ] GET /api/dashboard/organization
- [ ] GET /api/admin/audit-logs
- [ ] Machine Token Authentication

### Security
- [ ] Machine Token Authentication Middleware
- [ ] Rate Limiting (opcjonalne)
- [ ] Input Validation Enhancement

### Testing
- [ ] Application Layer unit tests (min 70% coverage)
- [ ] API Integration tests
- [ ] Authentication tests

### Documentation
- [ ] Swagger pełna dokumentacja
- [ ] README z instrukcjami deployment

---

## 🎯 Priorytety Implementacji

### Faza 1: Krytyczne (bez tego nie ma MVP) 🔴
1. MachineLogs Entity + Repository + Migration
2. Machine Logs Ingestion Endpoint (`/api/ingest/logs`)
3. Machine Token Authentication
4. AuditLog Entity + Repository + Service
5. Dashboard Queries + Controller
6. Application Layer unit tests (podstawowe)

### Faza 2: Ważne (ulepszenia MVP) 🟡
1. Audit Logs Controller
2. Comments enhancement (edycja/usunięcie z limitem czasowym)
3. Bulk Actions (US-014)
4. Email Service produkcyjna implementacja
5. Rate Limiting
6. Integration tests

### Faza 3: Opcjonalne (nice to have) 🟢
1. Export Ticket History (PDF/CSV)
2. OrganizationContact Entity
3. Zaawansowane filtrowanie Audit Logs
4. Pełna dokumentacja API

---

## 📊 Szacunkowe Nakłady Pracy

| Komponent | Szacowany czas | Priorytet |
|-----------|----------------|-----------|
| MachineLogs (full stack) | 4-6h | 🔴 |
| AuditLog (full stack) | 6-8h | 🔴 |
| Machine Token Auth | 2-3h | 🔴 |
| Dashboard Queries | 4-6h | 🔴 |
| Dashboard Controller | 2-3h | 🔴 |
| Unit Tests | 8-12h | 🔴 |
| Integration Tests | 6-8h | 🟡 |
| Email Service prod | 4-6h | 🟡 |
| Bulk Actions | 4-6h | 🟡 |
| Audit Controller | 2-3h | 🟡 |
| Export History | 4-6h | 🟢 |
| Rate Limiting | 2-3h | 🟢 |

**Łączny szacowany czas:** 48-70 godzin roboczych

---

## ✅ Kryteria Akceptacji Backendu

Backend można uznać za **ukończony**, gdy:

1. **Wszystkie endpointy z api-plan.md są zaimplementowane i działają**
2. **Maszyny mogą wysyłać logi do systemu** (US-028, US-029)
3. **Pełny audyt wszystkich akcji** (US-054)
4. **Dashboard z KPI działa** (US-006, US-007, US-037)
5. **Testy pokrywają min 70% kodu aplikacyjnego**
6. **Build przechodzi bez błędów**
7. **Swagger dokumentuje wszystkie endpointy**
8. **Authentication i Authorization działają poprawnie**
9. **RLS Policies są zaaplikowane w bazie danych**
10. **Health checks zwracają poprawny status**

---

## 🏁 Podsumowanie

Backend FLOWerTRACK jest w **zaawansowanym stadium** implementacji. Główne moduły (Tickets, Machines, Organizations, Users, Comments, Attachments) są gotowe. 

**Kluczowe brakujące elementy:**
1. **Machine Logs Ingestion** - krytyczne dla integracji z maszynami
2. **Audit Log** - wymagane przez PRD dla bezpieczeństwa
3. **Dashboard Queries** - potrzebne dla UI portali
4. **Testy** - obecne pokrycie wymaga rozszerzenia

Po zaimplementowaniu elementów z Fazy 1 (Krytyczne), backend będzie gotowy do MVP.

---

**Ostatnia aktualizacja:** 2025-12-05  
**Autor:** Analiza automatyczna na podstawie PRD, api-plan.md, db-plan.md i stanu kodu
