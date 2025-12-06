# Architektura Backendu FLOWerTRACK

**Data utworzenia:** 2025-12-06  
**Wersja:** 1.0  
**Status:** Dokumentacja aktualnego stanu backendu

---

## 📋 Spis Treści

1. [Przegląd Architektury](#przegląd-architektury)
2. [Struktura Projektu](#struktura-projektu)
3. [Warstwa Domain](#warstwa-domain)
4. [Warstwa Application](#warstwa-application)
5. [Warstwa Infrastructure](#warstwa-infrastructure)
6. [Warstwa Presentation (API)](#warstwa-presentation-api)
7. [Autentykacja i Autoryzacja](#autentykacja-i-autoryzacja)
8. [Obsługa Błędów i Logowanie](#obsługa-błędów-i-logowanie)
9. [Rate Limiting](#rate-limiting)
10. [Endpointy API](#endpointy-api)

---

## 📐 Przegląd Architektury

Backend FLOWerTRACK zbudowany jest według zasad **Clean Architecture** z wyraźnym podziałem odpowiedzialności:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation (API)                        │
│  Controllers, Middleware, Contracts, Exception Handling      │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                         │
│  CQRS (Commands/Queries), Handlers, DTOs, Validators        │
├─────────────────────────────────────────────────────────────┤
│                    Domain Layer                              │
│  Entities, Value Objects, Events, Repositories (interfaces) │
├─────────────────────────────────────────────────────────────┤
│                    Infrastructure Layer                      │
│  EF Core, Repositories (impl), External Services, Supabase  │
└─────────────────────────────────────────────────────────────┘
```

### Kluczowe Technologie

| Komponent | Technologia |
|-----------|-------------|
| Framework | .NET 10.0 RC2 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core + PostgreSQL |
| Baza danych | Supabase (PostgreSQL) |
| Autentykacja | JWT Bearer + Supabase Auth |
| CQRS | MediatR |
| Walidacja | FluentValidation |
| Logowanie | Serilog |
| Dokumentacja API | Swagger/OpenAPI |

---

## 📁 Struktura Projektu

```
src/backend/
├── Core/
│   ├── Flowertrack.Domain/           # Encje, Value Objects, Events
│   └── Flowertrack.Application/      # CQRS, Handlers, Validators
├── Infrastructure/
│   └── Flowertrack.Infrastructure/   # EF Core, Repositories, Services
├── Presentation/
│   ├── Flowertrack.Api/              # Controllers, Middleware
│   └── Flowertrack.Contracts/        # Request/Response DTOs
└── Tests/
    ├── Flowertrack.Domain.Tests/
    ├── Flowertrack.Application.Tests/
    ├── Flowertrack.Infrastructure.Tests/
    └── Flowertrack.Api.IntegrationTests/
```

---

## 🏛️ Warstwa Domain

### Encje (Aggregate Roots)

#### 1. **Ticket** - Zgłoszenie serwisowe
```
Ticket
├── Id: Guid (PK)
├── TicketNumber: TicketNumber (Value Object)
├── Title: string (max 255)
├── Description: string (max 5000)
├── OrganizationId: Guid (FK)
├── MachineId: Guid (FK)
├── Status: TicketStatus (enum)
├── Priority: Priority (enum)
├── CreatedByUserId: Guid
├── AssignedToUserId: Guid?
├── ResolvedAt: DateTimeOffset?
├── ClosedAt: DateTimeOffset?
└── [AuditableEntity fields]
```

**Statusy zgłoszeń (TicketStatus):**
- `New` (0) - Nowe zgłoszenie
- `Accepted` (1) - Zaakceptowane przez serwis
- `InProgress` (2) - W trakcie realizacji
- `Resolved` (3) - Rozwiązane
- `Closed` (4) - Zamknięte (stan finalny)
- `Reopened` (5) - Ponownie otwarte (do 14 dni od rozwiązania)

**Priorytety (Priority):**
- `Low` (0) - Niski
- `Medium` (1) - Średni
- `High` (2) - Wysoki
- `Critical` (3) - Krytyczny

#### 2. **Organization** - Organizacja klienta
```
Organization
├── Id: Guid (PK)
├── Name: string
├── Email: string?
├── Phone: string?
├── Address: string?
├── City: string?
├── PostalCode: string?
├── Country: string?
├── ServiceStatus: ServiceStatus (enum)
├── ContractStartDate: DateTimeOffset?
├── ContractEndDate: DateTimeOffset?
├── ApiKey: string? (dla integracji maszyn)
├── Notes: string?
└── [AuditableEntity fields]
```

#### 3. **Machine** - Maszyna/Urządzenie
```
Machine
├── Id: Guid (PK)
├── OrganizationId: Guid (FK)
├── SerialNumber: string (unique)
├── Brand: string?
├── Model: string?
├── Location: string?
├── Status: MachineStatus (Value Object)
├── ApiToken: MachineApiKey? (Value Object)
├── LastMaintenanceDate: DateOnly?
├── NextMaintenanceDate: DateOnly?
├── MaintenanceIntervalId: int?
└── [AuditableEntity fields]
```

#### 4. **ServiceUser** - Użytkownik serwisu
```
ServiceUser
├── Id: Guid (PK)
├── SupabaseUserId: Guid? (FK do auth.users)
├── FirstName: string
├── LastName: string
├── Email: Email (Value Object)
├── PhoneNumber: string?
├── Status: UserStatus (enum)
├── Specialization: string?
├── IsAvailable: bool
├── PasswordHash: string?
├── UserRoles: List<UserRole>
└── [AuditableEntity fields]
```

#### 5. **OrganizationUser** - Użytkownik organizacji
```
OrganizationUser
├── Id: Guid (PK)
├── SupabaseUserId: Guid? (FK do auth.users)
├── OrganizationId: Guid (FK)
├── FirstName: string
├── LastName: string
├── Email: Email (Value Object)
├── PhoneNumber: string?
├── Status: UserStatus (enum)
├── Role: string (OrganizationAdministrator/Operator)
├── IsActivated: bool
├── InvitationToken: string?
├── InvitationTokenExpiresAt: DateTimeOffset?
└── [AuditableEntity fields]
```

#### 6. **AuditLog** - Log audytowy
```
AuditLog
├── Id: long (PK)
├── UserId: Guid?
├── ActionType: string
├── TargetResourceId: string?
├── OldValue: string? (JSON)
├── NewValue: string? (JSON)
├── IpAddress: string?
├── UserAgent: string?
├── CreatedAt: DateTimeOffset
```

#### 7. **MachineLog** - Log z maszyny
```
MachineLog
├── Id: Guid (PK)
├── MachineId: Guid (FK)
├── ReceivedAt: DateTimeOffset
├── LogType: string
├── LogContent: string (JSON)
├── Status: string? (ALARM/NORMAL/WARNING)
├── AlarmCode: string?
├── Severity: string?
```

### Value Objects

| Value Object | Opis |
|--------------|------|
| `TicketNumber` | Unikalny numer zgłoszenia (format: TKT-YYYYMMDD-XXXX) |
| `Email` | Walidowany adres email |
| `MachineApiKey` | Token API dla maszyny |
| `MachineStatus` | Status operacyjny maszyny |
| `MaintenanceInterval` | Interwał przeglądów |

### Domain Events

System wykorzystuje zdarzenia domenowe dla luźnego sprzężenia:

- `TicketCreatedEvent`
- `TicketStatusChangedEvent`
- `TicketAssignedEvent`
- `MachineRegisteredEvent`
- `MachineLogReceivedEvent`
- `OrganizationCreatedEvent`
- `ServiceUserInvitedEvent`
- `OrganizationUserInvitedEvent`

---

## ⚙️ Warstwa Application

### Wzorzec CQRS (Command Query Responsibility Segregation)

Warstwa aplikacji wykorzystuje MediatR do implementacji CQRS:

```csharp
// Rejestracja MediatR z pipeline behaviors
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
});
```

### Pipeline Behaviors

1. **LoggingBehavior** - Logowanie wszystkich requestów/responsów
2. **ValidationBehavior** - Walidacja FluentValidation przed wykonaniem
3. **UnitOfWorkBehavior** - Automatyczny commit transakcji

### Moduły Application

#### Tickets Module
**Commands:**
- `CreateTicketCommand` - Tworzenie nowego zgłoszenia
- `UpdateTicketCommand` - Aktualizacja zgłoszenia
- `UpdateTicketStatusCommand` - Zmiana statusu
- `AssignTicketCommand` - Przypisanie serwisanta
- `AbandonTicketCommand` - Porzucenie zgłoszenia
- `AddCommentCommand` - Dodanie komentarza
- `AddNoteCommand` - Dodanie notatki wewnętrznej
- `DeleteTicketCommand` - Usunięcie zgłoszenia
- `BulkAssignTicketsCommand` - Masowe przypisanie
- `BulkChangeStatusCommand` - Masowa zmiana statusu
- `BulkArchiveTicketsCommand` - Masowa archiwizacja
- `UploadTicketAttachmentCommand` - Upload załącznika
- `DeleteTicketAttachmentCommand` - Usunięcie załącznika
- `AddTicketCommentCommand` - Dodanie komentarza
- `UpdateTicketCommentCommand` - Edycja komentarza
- `DeleteTicketCommentCommand` - Usunięcie komentarza

**Queries:**
- `GetTicketsQuery` - Lista zgłoszeń z filtrami i paginacją
- `GetTicketQuery` - Szczegóły zgłoszenia
- `GetTicketHistoryQuery` - Historia zmian
- `GetTicketsGroupedByStatusQuery` - Zgrupowane wg statusu
- `ExportTicketHistoryQuery` - Eksport historii
- `GetTicketCommentsQuery` - Komentarze do zgłoszenia
- `GetTicketAttachmentsQuery` - Załączniki do zgłoszenia

#### Machines Module
**Commands:**
- `RegisterMachineCommand` - Rejestracja nowej maszyny
- `UpdateMachineCommand` - Aktualizacja danych maszyny
- `DeleteMachineCommand` - Usunięcie maszyny
- `ChangeMachineStatusCommand` - Zmiana statusu
- `ActivateAlarmCommand` - Aktywacja alarmu
- `ClearAlarmCommand` - Wyczyszczenie alarmu
- `RegenerateMachineTokenCommand` - Regeneracja tokenu API
- `ScheduleMaintenanceCommand` - Zaplanowanie przeglądu
- `CompleteMaintenanceCommand` - Zakończenie przeglądu
- `IngestMachineLogCommand` - Przyjęcie logu z maszyny

**Queries:**
- `GetMachinesQuery` - Lista maszyn
- `GetMachineQuery` - Szczegóły maszyny
- `GetMachineLogsQuery` - Logi maszyny

#### Organizations Module
**Commands:**
- `OnboardOrganizationCommand` - Rejestracja organizacji
- `UpdateOrganizationCommand` - Aktualizacja danych
- `RegenerateApiKeyCommand` - Regeneracja klucza API

**Queries:**
- `GetOrganizationsQuery` - Lista organizacji
- `GetOrganizationQuery` - Szczegóły organizacji

#### Users Module
**Commands (Service Users):**
- `SignupServiceUserCommand` - Rejestracja serwisanta
- `LoginServiceUserCommand` - Logowanie serwisanta
- `InviteServiceUserCommand` - Zaproszenie serwisanta
- `UpdateServiceUserCommand` - Aktualizacja profilu
- `DeactivateServiceUserCommand` - Dezaktywacja
- `ReactivateServiceUserCommand` - Reaktywacja
- `ResetServiceUserPasswordCommand` - Reset hasła

**Commands (Organization Users):**
- `InviteOrganizationUserCommand` - Zaproszenie użytkownika org.
- `ActivateOrganizationUserCommand` - Aktywacja konta
- `RemoveOrganizationUserCommand` - Usunięcie użytkownika
- `ResendOrganizationUserInviteCommand` - Ponowne zaproszenie
- `LoginOrganizationUserCommand` - Logowanie

**Commands (Auth):**
- `LoginCommand` - Ogólne logowanie
- `RefreshTokenCommand` - Odświeżenie tokenu
- `ForgotPasswordCommand` - Zapomniałem hasła
- `ResetPasswordCommand` - Reset hasła

**Queries:**
- `GetCurrentServiceUserQuery` - Aktualny użytkownik serwisu
- `GetCurrentOrganizationUserQuery` - Aktualny użytkownik org.
- `GetServiceUsersQuery` - Lista serwisantów
- `GetServiceUserStatsQuery` - Statystyki serwisanta

#### Dashboard Module
**Queries:**
- `GetServiceDashboardQuery` - Dashboard serwisu (KPI)
- `GetOrganizationDashboardQuery` - Dashboard organizacji
- `GetRecentActivitiesQuery` - Ostatnie aktywności
- `GetOrganizationActivitiesQuery` - Aktywności organizacji
- `GetUpcomingMaintenancesQuery` - Nadchodzące przeglądy
- `GetTicketTrendsQuery` - Trendy zgłoszeń (wykresy)

#### Audit Module
**Queries:**
- `GetAuditLogsQuery` - Logi audytowe z filtrami

---

## 🔧 Warstwa Infrastructure

### Entity Framework Core

**ApplicationDbContext** zawiera DbSety dla wszystkich encji:
- `Organizations`
- `Machines`
- `MachineLogs`
- `ServiceUsers`
- `OrganizationUsers`
- `Tickets`
- `TicketHistories`
- `TicketComments`
- `TicketAttachments`
- `RefreshTokens`
- `Roles`
- `UserRoles`
- `AuditLogs`

### Repozytoria

| Repozytorium | Odpowiedzialność |
|--------------|------------------|
| `OrganizationRepository` | CRUD organizacji |
| `MachineRepository` | CRUD maszyn |
| `MachineLogRepository` | Logi maszyn |
| `ServiceUserRepository` | Użytkownicy serwisu |
| `OrganizationUserRepository` | Użytkownicy organizacji |
| `TicketRepository` | Zgłoszenia serwisowe |
| `TicketHistoryRepository` | Historia zgłoszeń |
| `TicketCommentRepository` | Komentarze |
| `TicketAttachmentRepository` | Załączniki |
| `RefreshTokenRepository` | Tokeny odświeżające |
| `RoleRepository` | Role systemowe |
| `UserRoleRepository` | Przypisania ról |
| `AuditLogRepository` | Logi audytowe |

### Serwisy Infrastrukturalne

| Serwis | Interfejs | Opis |
|--------|-----------|------|
| `EmailService` | `IEmailService` | Wysyłka email (stub) |
| `SupabaseStorageService` | `IFileStorageService` | Przechowywanie plików |
| `SupabaseAuthService` | `IAuthService` | Autentykacja Supabase |
| `JwtTokenGenerator` | `IJwtTokenGenerator` | Generowanie JWT |
| `BCryptPasswordHasher` | `IPasswordHasher` | Hashowanie haseł |
| `AuditService` | `IAuditService` | Logowanie audytowe |
| `CurrentUserService` | `ICurrentUserService` | Kontekst użytkownika |
| `DateTimeProvider` | `IDateTimeProvider` | Abstrakcja czasu |

---

## 🌐 Warstwa Presentation (API)

### Kontrolery

| Kontroler | Route | Opis |
|-----------|-------|------|
| `AuthController` | `/api/auth` | Autentykacja (login, logout, reset hasła) |
| `TicketsController` | `/api/tickets` | Zarządzanie zgłoszeniami |
| `TicketCommentsController` | `/api/tickets/{id}/comments` | Komentarze do zgłoszeń |
| `TicketAttachmentsController` | `/api/tickets/{id}/attachments` | Załączniki do zgłoszeń |
| `MachinesController` | `/api/machines` | Zarządzanie maszynami |
| `OrganizationsController` | `/api/organizations` | Zarządzanie organizacjami |
| `UsersController` | `/api/users` | Zarządzanie użytkownikami |
| `DashboardController` | `/api/dashboard` | KPI i metryki |
| `AdminController` | `/api/admin` | Funkcje administracyjne |
| `IngestController` | `/api/ingest` | Przyjmowanie logów maszyn |
| `HealthController` | `/health` | Health checks |

### Middleware

1. **GlobalExceptionHandlerMiddleware** - Globalna obsługa wyjątków (RFC 7807)
2. **MachineTokenAuthenticationMiddleware** - Autentykacja tokenem maszyny

---

## 🔐 Autentykacja i Autoryzacja

### JWT Authentication

```csharp
// Konfiguracja JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

### Polityki Autoryzacji

| Polityka | Role | Opis |
|----------|------|------|
| `RequireServiceAdmin` | ServiceAdministrator | Tylko administratorzy serwisu |
| `RequireServiceUser` | ServiceAdministrator, ServiceTechnician | Użytkownicy serwisu |
| `RequireOrganizationAdmin` | OrganizationAdministrator | Administratorzy organizacji |
| `RequireOrganizationUser` | OrganizationAdministrator, Operator | Użytkownicy organizacji |
| `RequireAuthenticatedUser` | (any authenticated) | Dowolny zalogowany użytkownik |

### Role Systemowe

**Service Portal:**
- `ServiceAdministrator` - Pełny dostęp do portalu serwisu
- `ServiceTechnician` - Obsługa zgłoszeń, dostęp do maszyn

**Client Portal:**
- `OrganizationAdministrator` - Zarządzanie organizacją
- `Operator` - Zgłaszanie i śledzenie zgłoszeń

### Machine Token Authentication

Dla endpointów `/api/ingest/*` używana jest osobna autentykacja tokenem maszyny:

```csharp
// Header: X-API-Token: <machine_api_token>
app.UseMachineTokenAuthentication();
```

---

## 📝 Obsługa Błędów i Logowanie

### Serilog Configuration

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}")
    .WriteTo.File("logs/flowertrack-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();
```

### Global Exception Handler

Middleware zwraca błędy w formacie RFC 7807 Problem Details:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "instance": "/api/tickets",
  "extensions": {
    "errors": ["Title is required", "Priority must be between 0 and 3"]
  }
}
```

### Typy Wyjątków

| Exception | HTTP Status | Opis |
|-----------|-------------|------|
| `ValidationException` | 400 | Błędy walidacji |
| `NotFoundException` | 404 | Zasób nie znaleziony |
| `UnauthorizedException` | 401 | Brak autoryzacji |
| `ForbiddenException` | 403 | Brak uprawnień |
| `DomainException` | 400 | Naruszenie reguł biznesowych |
| `Exception` (inne) | 500 | Nieoczekiwany błąd |

---

## 🚦 Rate Limiting

### Konfiguracja dla /api/ingest

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("IngestRateLimit", context =>
    {
        var machineId = context.Request.Headers["X-Machine-Id"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        return RateLimitPartition.GetFixedWindowLimiter(machineId, _ =>
            new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 100,  // 100 req/min per machine
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
    });
});
```

### Odpowiedź przy przekroczeniu limitu

```json
{
  "error": "Too many requests",
  "retryAfterSeconds": 45
}
```
HTTP Status: **429 Too Many Requests**

---

## 📡 Endpointy API

### Tickets (`/api/tickets`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| GET | `/` | Lista zgłoszeń (filtry, paginacja) | Authenticated |
| GET | `/{id}` | Szczegóły zgłoszenia | Authenticated |
| POST | `/` | Utwórz zgłoszenie | Authenticated |
| PUT | `/{id}` | Aktualizuj zgłoszenie | ServiceUser |
| DELETE | `/{id}` | Usuń zgłoszenie | ServiceAdmin |
| POST | `/{id}/status` | Zmień status | ServiceUser |
| POST | `/{id}/assign` | Przypisz serwisanta | ServiceUser |
| POST | `/{id}/notes` | Dodaj notatkę wewnętrzną | ServiceUser |
| GET | `/{id}/history` | Historia zmian | Authenticated |
| GET | `/grouped` | Zgrupowane wg statusu | Authenticated |
| POST | `/bulk/assign` | Masowe przypisanie | ServiceUser |
| POST | `/bulk/status` | Masowa zmiana statusu | ServiceUser |
| POST | `/bulk/archive` | Masowa archiwizacja | ServiceUser |

### Machines (`/api/machines`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| GET | `/` | Lista maszyn | Authenticated |
| GET | `/{id}` | Szczegóły maszyny | Authenticated |
| POST | `/` | Zarejestruj maszynę | ServiceUser |
| PUT | `/{id}` | Aktualizuj maszynę | ServiceUser |
| DELETE | `/{id}` | Usuń maszynę | ServiceAdmin |
| POST | `/{id}/status` | Zmień status | ServiceUser |
| POST | `/{id}/alarm` | Aktywuj alarm | ServiceUser |
| DELETE | `/{id}/alarm` | Wyczyść alarm | ServiceUser |
| POST | `/{id}/token` | Regeneruj token API | ServiceUser |
| POST | `/{id}/maintenance/schedule` | Zaplanuj przegląd | ServiceUser |
| POST | `/{id}/maintenance/complete` | Zakończ przegląd | ServiceUser |
| GET | `/{id}/logs` | Logi maszyny | Authenticated |

### Organizations (`/api/organizations`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| GET | `/` | Lista organizacji | ServiceUser |
| GET | `/{id}` | Szczegóły organizacji | Authenticated |
| POST | `/` | Onboard organizację | ServiceAdmin |
| PUT | `/{id}` | Aktualizuj organizację | ServiceAdmin |
| POST | `/{id}/api-key` | Regeneruj klucz API | ServiceAdmin |

### Auth (`/api/auth`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| POST | `/service/login` | Logowanie serwisanta | Anonymous |
| POST | `/service/logout` | Wylogowanie | Authenticated |
| POST | `/service/signup` | Rejestracja serwisanta | ServiceAdmin |
| POST | `/service/forgot-password` | Zapomniałem hasła | Anonymous |
| POST | `/service/reset-password` | Reset hasła | Anonymous |
| POST | `/client/login` | Logowanie klienta | Anonymous |
| POST | `/client/invite` | Zaproszenie klienta | ServiceUser |
| POST | `/client/activate` | Aktywacja konta | Anonymous |
| POST | `/client/resend-invite` | Ponowne zaproszenie | ServiceUser |
| GET | `/me` | Aktualny użytkownik | Authenticated |
| POST | `/refresh` | Odśwież token | Anonymous |

### Dashboard (`/api/dashboard`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| GET | `/service` | Dashboard serwisu | ServiceUser |
| GET | `/organization` | Dashboard organizacji | OrgUser |
| GET | `/service/activities` | Ostatnie aktywności | ServiceUser |
| GET | `/organization/activities` | Aktywności org. | OrgUser |
| GET | `/service/maintenances` | Nadchodzące przeglądy | ServiceUser |

### Admin (`/api/admin`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| GET | `/audit-logs` | Logi audytowe | ServiceAdmin |
| GET | `/audit-logs/user/{userId}` | Logi użytkownika | ServiceAdmin |
| GET | `/users/service/{id}/stats` | Statystyki serwisanta | ServiceAdmin |

### Ingest (`/api/ingest`)

| Method | Endpoint | Opis | Autoryzacja |
|--------|----------|------|-------------|
| POST | `/logs` | Przyjęcie logu maszyny | MachineToken |

### Health (`/health`)

| Method | Endpoint | Opis |
|--------|----------|------|
| GET | `/health` | Status wszystkich health checks |
| GET | `/health/ready` | Readiness probe |
| GET | `/health/live` | Liveness probe |

---

## 📊 Podsumowanie

Backend FLOWerTRACK jest w pełni funkcjonalnym MVP z:

✅ **Gotowe moduły:**
- Tickets (zgłoszenia serwisowe)
- Comments & Attachments
- Machines (maszyny)
- Machine Logs Ingest
- Organizations
- Service Users & Organization Users
- Dashboard (KPI)
- Audit Logs
- Authentication & Authorization
- Rate Limiting
- Health Checks

⚠️ **Do uzupełnienia:**
- Produkcyjna implementacja EmailService (obecnie stub)
- Export PDF/CSV dla historii zgłoszeń
- Notyfikacje real-time (opcjonalnie)

---

*Dokumentacja wygenerowana automatycznie na podstawie analizy kodu źródłowego.*
