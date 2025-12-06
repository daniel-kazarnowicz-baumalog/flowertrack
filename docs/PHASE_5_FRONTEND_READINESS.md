# FAZA 5: Gotowość do integracji z Frontendem

**Data:** 2025-12-06  
**Status:** W trakcie implementacji  
**Zaktualizowano:** 2025-12-06 - Zaimplementowano kluczowe brakujące funkcje  
**Cel:** Przygotowanie backendu do pełnej integracji z frontendem React  

---

## 📊 Analiza Stanu Obecnego

### ✅ Zaimplementowane Moduły (Gotowe)

| Moduł | Encje | Application Layer | API Endpoints | Status |
|-------|-------|-------------------|---------------|--------|
| **Tickets** | ✅ Ticket, TicketHistory | ✅ CQRS Commands/Queries | ✅ 12+ endpointów | ✅ Gotowe |
| **Comments** | ✅ TicketComment | ✅ Add/Update/Delete/Get | ✅ TicketCommentsController | ✅ Gotowe |
| **Attachments** | ✅ TicketAttachment | ✅ Upload/Delete/Get | ✅ TicketAttachmentsController | ✅ Gotowe |
| **Machines** | ✅ Machine, MachineLog | ✅ Commands/Queries | ✅ MachinesController | ✅ Gotowe |
| **Machine Logs Ingest** | ✅ MachineLog | ✅ IngestMachineLogCommand | ✅ POST /api/ingest/logs | ✅ Gotowe |
| **Organizations** | ✅ Organization | ✅ Commands/Queries | ✅ OrganizationsController | ✅ Gotowe |
| **Users (Service)** | ✅ ServiceUser | ✅ Invite/Update/Deactivate | ✅ UsersController | ✅ Gotowe |
| **Users (Org)** | ✅ OrganizationUser | ✅ Invite/Activate/Remove | ✅ AuthController | ✅ Gotowe |
| **Dashboard** | ❌ Brak encji | ✅ Service/Org Dashboard Queries | ✅ DashboardController | ✅ Gotowe |
| **Audit Logs** | ✅ AuditLog | ✅ GetAuditLogsQuery | ✅ AdminController | ✅ Gotowe |
| **Auth** | ✅ RefreshToken | ✅ Login/Logout/Refresh | ✅ AuthController | ✅ Gotowe |

### 🔄 Częściowo Zaimplementowane

| Element | Stan | Co brakuje |
|---------|------|------------|
| **Bulk Actions** | ✅ Commands istnieją | Wymaga weryfikacji działania |
| **Export History** | ✅ Query istnieje | Wymaga formatowania PDF/CSV |
| **Ticket Reopen** | ✅ Naprawione | US-047: 14-dniowe okno walidowane |
| **Rate Limiting** | ✅ Zaimplementowane | 100 req/min dla /api/ingest |
| **Email Service** | ⚠️ Stub | Produkcyjna implementacja |

---

## 🆕 Zaimplementowane w tej fazie (2025-12-06)

### 1. GetServiceUserStats Query (US-031)
- **Plik:** `Application/Users/Queries/GetServiceUserStats/`
- **Endpoint:** `GET /api/admin/users/service/{id}/stats`
- **Funkcja:** Statystyki serwisanta - przypisane/rozwiązane/zamknięte zgłoszenia, średni czas rozwiązania

### 2. ResendOrganizationUserInvite Command (US-051)
- **Plik:** `Application/Users/Commands/ResendOrganizationUserInvite/`
- **Endpoint:** `POST /api/auth/client/resend-invite`
- **Funkcja:** Ponowne wysłanie zaproszenia email dla użytkowników organizacji

### 3. Dashboard DTO Extensions (US-007)
- **Plik:** `Application/Dashboard/Queries/GetServiceDashboard/ServiceDashboardDto.cs`
- **Dodano:** `TicketTrendDataPointDto` i `PriorityDistributionDto` dla wykresów
- **Handler zaktualizowany:** Kalkulacja trendów 30-dniowych i dystrybucji priorytetów

### 4. Rate Limiting (Bezpieczeństwo)
- **Plik:** `Program.cs` - dodano AddRateLimiter
- **Plik:** `IngestController.cs` - dodano [EnableRateLimiting("IngestRateLimit")]
- **Konfiguracja:** 100 requestów/minutę per maszyna (FixedWindowLimiter)

### 5. Ticket Reopen 14-day Validation (US-047)
- **Plik:** `Domain/Entities/Ticket.cs`
- **Zmiana:** Metoda Reopen() teraz waliduje 14-dniowe okno dla zgłoszeń Resolved

---

## 🚨 Wymagania dla Frontendu (US z PRD)

### Portal Serwisu - Wymagane Endpointy

#### 1. Dashboard (US-006, US-007, US-008)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-006 | Kafelki KPI | GET /api/dashboard/service | ✅ Gotowe |
| US-007 | Wykresy trendów | GET /api/dashboard/service | ✅ Zaimplementowane (TicketTrends, PriorityDistribution) |
| US-008 | Ostatnie zdarzenia | GET /api/dashboard/service/activities | ✅ Gotowe |
| US-008 | Organizacje z alarmami | GET /api/dashboard/service | ✅ W odpowiedzi |
| US-008 | Nadchodzące przeglądy | GET /api/dashboard/service/maintenances | ✅ Gotowe |

#### 2. Lista Zgłoszeń (US-010 do US-014)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-010 | Lista zgłoszeń | GET /api/tickets | ✅ Gotowe |
| US-011 | Filtrowanie | GET /api/tickets?... | ✅ Gotowe |
| US-012 | Wyszukiwanie | GET /api/tickets?searchText= | ✅ Gotowe |
| US-013 | Grupowanie wg statusu | GET /api/tickets/grouped | ✅ Gotowe |
| US-014 | Bulk Assign | POST /api/tickets/bulk/assign | ✅ Gotowe |
| US-014 | Bulk Status | POST /api/tickets/bulk/status | ✅ Gotowe |
| US-014 | Bulk Archive | POST /api/tickets/bulk/archive | ✅ Gotowe |

#### 3. Szczegóły Zgłoszenia (US-015 do US-023)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-015 | Szczegóły ticketu | GET /api/tickets/{id} | ✅ Gotowe |
| US-016 | Historia/Timeline | GET /api/tickets/{id}/history | ✅ Gotowe |
| US-017 | Zmiana statusu | POST /api/tickets/{id}/status | ✅ Gotowe |
| US-018 | Notatki wewnętrzne | POST /api/tickets/{id}/notes | ✅ Gotowe |
| US-019 | Przypisanie | POST /api/tickets/{id}/assign | ✅ Gotowe |
| US-020 | Komentarze | POST /api/tickets/{id}/comments | ✅ Gotowe |
| US-021 | Edycja komentarza | PUT /api/ticket-comments/{id} | ✅ Gotowe |
| US-022 | Eksport historii | GET /api/tickets/{id}/history/export | ⚠️ Wymaga formatowania |
| US-023 | Lista załączników | GET /api/ticket-attachments?ticketId= | ✅ Gotowe |
| US-023 | Upload załącznika | POST /api/ticket-attachments | ✅ Gotowe |
| US-023 | Download załącznika | GET /api/ticket-attachments/{id}/download | ✅ Gotowe |

#### 4. Organizacje (US-024 do US-029)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-024 | Lista organizacji | GET /api/organizations | ✅ Gotowe |
| US-025 | Onboarding | POST /api/organizations/onboard | ✅ Gotowe |
| US-026 | Szczegóły org | GET /api/organizations/{id} | ✅ Gotowe |
| US-027 | Maszyny org | GET /api/organizations/{id}/machines | ✅ Gotowe |
| US-028 | Token API maszyny | POST /api/machines/{id}/regenerate-token | ✅ Gotowe |
| US-029 | Status maszyn | GET /api/machines | ✅ Gotowe |

#### 5. Administracja (US-030 do US-034)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-030 | Lista serwisantów | GET /api/admin/users/service | ✅ Gotowe |
| US-031 | Statystyki serwisanta | GET /api/admin/users/service/{id}/stats | ✅ Zaimplementowane |
| US-032 | Zaproszenie serwisanta | POST /api/admin/users/service/invite | ✅ Gotowe |
| US-033 | Reset hasła | POST /api/admin/users/service/{id}/reset-password | ✅ Gotowe |
| US-034 | Dezaktywacja | POST /api/admin/users/service/{id}/deactivate | ✅ Gotowe |
| US-054 | Audit Logs | GET /api/admin/audit-logs | ✅ Gotowe |

### Portal Klienta - Wymagane Endpointy

#### 1. Autentykacja (US-004, US-005)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-004 | Login | POST /api/auth/organization/login | ✅ Gotowe |
| US-004 | Logout | POST /api/auth/organization/logout | ✅ Gotowe |
| US-005 | Aktywacja konta | POST /api/auth/organization/activate | ✅ Gotowe |
| - | Refresh Token | POST /api/auth/refresh | ✅ Gotowe |

#### 2. Dashboard Klienta (US-037 do US-039)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-037 | Dashboard org | GET /api/dashboard/organization | ✅ Gotowe |
| US-038 | Statusy maszyn | GET /api/machines?organizationId= | ✅ Gotowe |
| US-039 | Ostatnie aktywności | GET /api/dashboard/organization/activities | ✅ Gotowe |

#### 3. Zgłoszenia Klienta (US-042 do US-047)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-042 | Lista ticketów | GET /api/tickets?organizationId= | ✅ Gotowe |
| US-043 | Tworzenie ticketu | POST /api/tickets | ✅ Gotowe |
| US-044 | Szczegóły ticketu | GET /api/tickets/{id} | ✅ Gotowe |
| US-045 | Dodaj komentarz | POST /api/tickets/{id}/comments | ✅ Gotowe |
| US-046 | Porzuć ticket | POST /api/tickets/{id}/abandon | ✅ Gotowe |
| US-047 | Wznów ticket | POST /api/tickets/{id}/reopen | ⚠️ Do weryfikacji |

#### 4. Administracja Zespołu (US-049 do US-052)
| US | Funkcja | Endpoint | Status |
|----|---------|----------|--------|
| US-049 | Lista operatorów | GET /api/organizations/{id}/users | ✅ Gotowe |
| US-050 | Zaproś operatora | POST /api/auth/organization/invite | ✅ Gotowe |
| US-051 | Wyślij ponownie | POST /api/auth/organization/resend-invite | ❌ Brakuje |
| US-052 | Usuń operatora | DELETE /api/organizations/{id}/users/{userId} | ✅ Gotowe |

---

## 🎯 Plan Implementacji - FAZA 5

### Faza 5.1: Brakujące Endpointy (4-6h)

#### 5.1.1 Statystyki Serwisanta (US-031)
**Ścieżka:** `GET /api/admin/users/service/{id}/stats`

**Pliki do utworzenia:**
```
Application/Users/Queries/GetServiceUserStats/
├── GetServiceUserStatsQuery.cs
├── GetServiceUserStatsQueryHandler.cs
└── ServiceUserStatsDto.cs
```

**Dane zwracane:**
```csharp
public record ServiceUserStatsDto
{
    public Guid UserId { get; init; }
    public int TotalTicketsAssigned { get; init; }
    public int TicketsResolvedLast30Days { get; init; }
    public int TicketsInProgress { get; init; }
    public double AverageResolutionTimeHours { get; init; }
    public DateTimeOffset? LastActivityAt { get; init; }
}
```

#### 5.1.2 Ponowne Wysłanie Zaproszenia (US-051)
**Ścieżka:** `POST /api/auth/organization/resend-invite`

**Pliki do utworzenia:**
```
Application/Users/Commands/ResendOrganizationUserInvite/
├── ResendOrganizationUserInviteCommand.cs
├── ResendOrganizationUserInviteCommandHandler.cs
└── ResendOrganizationUserInviteCommandValidator.cs
```

#### 5.1.3 Ticket Reopen z 14-dniowym limitem (US-047)
**Weryfikacja istniejącego endpointu:** `POST /api/tickets/{id}/reopen`

**Wymagania:**
- Sprawdzić czy implementacja sprawdza 14-dniowy limit od `ResolvedAt`
- Ticket musi być w statusie `Resolved`
- Tylko OrganizationAdministrator może wznowić

### Faza 5.2: Ulepszenia Dashboard (4-6h)

#### 5.2.1 Rozszerzenie ServiceDashboardDto
**Dodać do odpowiedzi:**
- `TicketTrends` - dane do wykresów (ostatnie 30 dni)
- `PriorityDistribution` - rozkład priorytetów

**Plik:** `Application/Dashboard/Queries/GetServiceDashboard/ServiceDashboardDto.cs`

```csharp
public record ServiceDashboardDto
{
    // Istniejące pola...
    
    // Nowe pola dla wykresów (US-007)
    public List<TicketTrendDataPoint> TicketTrends { get; init; } = [];
    public List<PriorityDistributionItem> PriorityDistribution { get; init; } = [];
}

public record TicketTrendDataPoint
{
    public DateOnly Date { get; init; }
    public int Created { get; init; }
    public int Resolved { get; init; }
    public int Closed { get; init; }
}

public record PriorityDistributionItem
{
    public string Priority { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal Percentage { get; init; }
}
```

### Faza 5.3: Export Ticket History (4-6h)

#### 5.3.1 Formatowanie PDF/CSV
**Endpoint:** `GET /api/tickets/{id}/history/export?format=pdf|csv|json`

**Pliki do utworzenia/aktualizacji:**
```
Application/Tickets/Queries/ExportTicketHistory/
├── ExportTicketHistoryQuery.cs (istniejący)
├── ExportTicketHistoryQueryHandler.cs (aktualizacja)
├── ITicketHistoryExporter.cs (nowy)
├── PdfTicketHistoryExporter.cs (nowy)
├── CsvTicketHistoryExporter.cs (nowy)
```

**Biblioteki wymagane:**
- `QuestPDF` dla PDF
- Wbudowany CSV handler

### Faza 5.4: Contracts/DTOs dla Frontendu (3-4h)

#### 5.4.1 Weryfikacja i uzupełnienie kontraktów
**Sprawdzić spójność między:**
- `Flowertrack.Contracts` (Response DTOs)
- `Flowertrack.Application` (Internal DTOs)

**Pliki do przeglądu:**
```
Contracts/
├── Tickets/Responses/
│   ├── TicketDetailsResponse.cs - czy zawiera wszystkie pola dla US-015?
│   ├── TicketHistoryResponse.cs - czy zawiera wszystkie typy zdarzeń?
│   └── GetTicketsResponse.cs - paginacja OK?
├── Dashboard/
│   ├── ServiceDashboardResponse.cs - dodać nowe pola
│   └── OrganizationDashboardResponse.cs - weryfikacja
├── Users/
│   └── ServiceUserStatsResponse.cs - NOWY
```

### Faza 5.5: Walidacja i Bezpieczeństwo (4-6h)

#### 5.5.1 FluentValidation dla wszystkich Commands
**Sprawdzić pokrycie:**
- [ ] CreateTicketCommand
- [ ] UpdateTicketCommand
- [ ] AddCommentCommand
- [ ] InviteOrganizationUserCommand
- [ ] OnboardOrganizationCommand

#### 5.5.2 Rate Limiting dla /api/ingest
```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("MachineIngest", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });
});
```

### Faza 5.6: Dokumentacja API (2-3h)

#### 5.6.1 Swagger Enhancement
**Dodać do wszystkich endpointów:**
- Opisy parametrów
- Przykłady request/response
- Kody błędów

**Plik:** `Program.cs` - rozszerzenie SwaggerGen

### Faza 5.7: Testy (8-10h)

#### 5.7.1 Application Layer Unit Tests
**Priorytet wysokich:**
- `CreateTicketCommandHandlerTests.cs`
- `UpdateTicketStatusCommandHandlerTests.cs`
- `AddCommentCommandHandlerTests.cs`
- `GetTicketsQueryHandlerTests.cs`
- `GetServiceDashboardQueryHandlerTests.cs`

#### 5.7.2 Integration Tests
**Pliki do utworzenia:**
```
Tests/Flowertrack.Api.IntegrationTests/
├── TicketsControllerTests.cs
├── AuthControllerTests.cs
├── DashboardControllerTests.cs
└── Fixtures/
    ├── TestWebApplicationFactory.cs
    └── TestAuthHandler.cs
```

---

## 📋 Checklist Gotowości do Frontendu

### Autentykacja
- [x] Login Service User
- [x] Login Organization User
- [x] Logout
- [x] Refresh Token
- [x] Activate Account
- [ ] Forgot Password (endpoint istnieje, email stub)
- [ ] Reset Password (endpoint istnieje, email stub)

### Dashboard
- [x] Service Dashboard KPIs
- [x] Organization Dashboard KPIs
- [ ] Trend Charts Data (rozszerzenie DTO)
- [x] Recent Activities
- [x] Upcoming Maintenances

### Tickets
- [x] List with filters/pagination
- [x] Create
- [x] Get Details
- [x] Update
- [x] Change Status
- [x] Assign
- [x] Add Comment
- [x] Add Internal Note
- [x] Get History/Timeline
- [x] Upload Attachment
- [x] Download Attachment
- [x] Abandon (Client)
- [ ] Reopen (weryfikacja 14 dni)
- [x] Bulk Assign
- [x] Bulk Change Status
- [x] Bulk Archive
- [x] Grouped by Status
- [ ] Export History (formatowanie)

### Organizations
- [x] List
- [x] Details
- [x] Onboard
- [x] Update
- [x] Get Machines
- [x] Get Users
- [x] Get Tickets

### Users
- [x] List Service Users
- [x] Invite Service User
- [x] Update Service User
- [x] Deactivate Service User
- [x] Reactivate Service User
- [x] Reset Password
- [ ] Get Stats (nowy endpoint)
- [x] Invite Organization User
- [ ] Resend Invite (nowy endpoint)
- [x] Remove Organization User

### Machines
- [x] List
- [x] Register
- [x] Update
- [x] Delete
- [x] Regenerate Token
- [x] Get Logs
- [x] Change Status
- [x] Schedule Maintenance
- [x] Complete Maintenance
- [x] Activate Alarm
- [x] Clear Alarm

### Admin
- [x] Audit Logs List
- [x] Audit Logs by User
- [x] Audit Logs by Resource

### Machine Ingest
- [x] POST /api/ingest/logs
- [x] Token Authentication
- [ ] Rate Limiting (do dodania)

---

## ⏱️ Szacowany Czas Implementacji

| Faza | Opis | Czas |
|------|------|------|
| 5.1 | Brakujące Endpointy | 4-6h |
| 5.2 | Ulepszenia Dashboard | 4-6h |
| 5.3 | Export Ticket History | 4-6h |
| 5.4 | Contracts/DTOs | 3-4h |
| 5.5 | Walidacja i Bezpieczeństwo | 4-6h |
| 5.6 | Dokumentacja API | 2-3h |
| 5.7 | Testy | 8-10h |
| **Razem** | | **29-41h** |

---

## 🏃 Kolejność Implementacji (Rekomendacja)

### Sprint 1 (Must Have dla MVP) - 16-22h
1. **Faza 5.1** - Brakujące endpointy (statystyki, resend invite)
2. **Faza 5.4** - Weryfikacja Contracts dla frontendu
3. **Faza 5.5** - Rate Limiting dla ingest

### Sprint 2 (Ważne dla UX) - 8-12h
1. **Faza 5.2** - Dashboard z danymi do wykresów
2. **Faza 5.6** - Dokumentacja Swagger

### Sprint 3 (Nice to Have) - 12-16h
1. **Faza 5.3** - Export PDF/CSV
2. **Faza 5.7** - Testy

---

## 📝 Uwagi dla Frontendu

### Formaty Danych
- Wszystkie daty: `DateTimeOffset` w formacie ISO 8601
- ID: `Guid` (UUID)
- Enumy: zwracane jako `int` (np. Status: 0-5, Priority: 0-3)
- Paginacja: `PageNumber` (1-based), `PageSize`, `TotalCount`, `TotalPages`

### Autentykacja
- JWT Bearer Token w nagłówku `Authorization: Bearer <token>`
- Refresh Token w ciele żądania POST /api/auth/refresh
- Token expiry: ~1h (konfigurowalny)

### Role i Uprawnienia
| Rola | Dostęp |
|------|--------|
| ServiceAdministrator | Pełny dostęp do wszystkiego |
| ServiceTechnician | Zarządzanie ticketami, maszynami |
| OrganizationAdministrator | Pełny dostęp do swojej organizacji |
| Operator | Własne tickety + widok maszyn organizacji |

### Kody Błędów
- `400` - Błąd walidacji (szczegóły w body)
- `401` - Nieautoryzowany (token wygasł/brak)
- `403` - Brak uprawnień
- `404` - Zasób nie znaleziony
- `409` - Konflikt (np. duplikat)
- `500` - Błąd serwera

---

**Autor:** Copilot AI Analysis  
**Ostatnia aktualizacja:** 2025-12-06
