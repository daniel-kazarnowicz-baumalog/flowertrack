# FAZA 0: Setup & Infrastructure ⚙️

**Status:** 🟡 In Progress  
**Started:** 2025-10-25  
**Target Completion:** Sprint 1 (Week 1)

---

## 0.1 Solution Structure ✅

### Projekty do utworzenia:

- [ ] **Flowertrack.Domain** (Class Library)
  - TargetFramework: net10.0
  - Nullable: enable
  - Dependencies: Brak (pure domain)

- [ ] **Flowertrack.Application** (Class Library)
  - TargetFramework: net10.0
  - Nullable: enable
  - Dependencies: Flowertrack.Domain, MediatR, FluentValidation, AutoMapper

- [ ] **Flowertrack.Infrastructure** (Class Library)
  - TargetFramework: net10.0
  - Nullable: enable
  - Dependencies: Flowertrack.Application, EF Core, Supabase SDK, Npgsql

- [ ] **Flowertrack.Contracts** (Class Library)
  - TargetFramework: net10.0
  - Nullable: enable
  - Dependencies: Brak (tylko DTOs)

- [ ] **Flowertrack.Api** (Web API - już istnieje, wymaga refactoru)
  - TargetFramework: net10.0
  - Nullable: enable
  - Dependencies: Flowertrack.Infrastructure, HotChocolate

- [ ] **Flowertrack.Domain.Tests** (xUnit Test Project)
  - TargetFramework: net10.0
  - Dependencies: xUnit, FluentAssertions, Moq

- [ ] **Flowertrack.Application.Tests** (xUnit Test Project)
  - TargetFramework: net10.0
  - Dependencies: xUnit, FluentAssertions, Moq, MediatR

- [ ] **Flowertrack.Infrastructure.Tests** (xUnit Test Project)
  - TargetFramework: net10.0
  - Dependencies: xUnit, FluentAssertions, Testcontainers.PostgreSql

- [ ] **Flowertrack.Api.IntegrationTests** (xUnit Test Project)
  - TargetFramework: net10.0
  - Dependencies: xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing

### Solution File:
- [ ] Zaktualizować `Flowertrack.sln` z nowymi projektami
- [ ] Zorganizować w foldery: `src/`, `tests/`

---

## 0.2 Supabase Configuration 🔐

### Connection & Authentication:
- [ ] Dodać Supabase connection string do `appsettings.json`
- [ ] Dodać Supabase API key, URL, Anon key do User Secrets
- [ ] Utworzyć `SupabaseOptions` configuration class
- [ ] Skonfigurować JWT Bearer authentication z Supabase
- [ ] Dodać `ISupabaseClient` interface w Infrastructure
- [ ] Implementacja `SupabaseClientService`

### Packages:
- [ ] `supabase-csharp` - Official Supabase SDK
- [ ] `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider dla EF Core
- [ ] `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication

### Storage Configuration:
- [ ] Skonfigurować Supabase Storage client
- [ ] Utworzyć bucket `attachments` w Supabase
- [ ] Skonfigurować RLS policies dla storage

### Migration Strategy:
- [ ] Określić strategię: EF Core migrations + manual Supabase SQL scripts dla RLS
- [ ] Utworzyć folder `Infrastructure/Persistence/Scripts/` dla custom SQL
- [ ] Dokumentacja procesu migracji

---

## 0.3 Base Configuration 🛠️

### Logging (Serilog):
- [ ] Dodać `Serilog.AspNetCore`
- [ ] Dodać `Serilog.Sinks.Console`
- [ ] Dodać `Serilog.Sinks.File`
- [ ] Dodać `Serilog.Enrichers.Environment`
- [ ] Dodać `Serilog.Enrichers.Thread`
- [ ] Skonfigurować Serilog w `Program.cs`
- [ ] Utworzyć logs folder structure: `logs/flowertrack-.log`
- [ ] Rolling file configuration (daily, size limit: 100MB)

### Exception Handling:
- [ ] Utworzyć `GlobalExceptionHandler` middleware
- [ ] Obsługa `ValidationException`, `NotFoundException`, `UnauthorizedException`
- [ ] Strukturyzowany error response model
- [ ] Logging exceptions z Serilog

### CORS:
- [ ] Dodać CORS policy w `Program.cs`
- [ ] Konfiguracja dla Frontend URL (z appsettings)
- [ ] Allow credentials dla JWT

### Health Checks:
- [ ] Dodać `Microsoft.Extensions.Diagnostics.HealthChecks`
- [ ] Dodać `AspNetCore.HealthChecks.Npgsql` - PostgreSQL health check
- [ ] Endpoint `/health` z podstawowymi checks
- [ ] Database connectivity check
- [ ] Supabase API availability check

### Configuration Files:
- [ ] `appsettings.json` - base config
- [ ] `appsettings.Development.json` - dev overrides
- [ ] `appsettings.Production.json` - production config
- [ ] `.gitignore` - exclude secrets, logs, bin, obj
- [ ] User Secrets configuration dla Development

---

## Postęp Fazy 0

**Ukończone:** 0/30 (0%)  
**W trakcie:** 0  
**Do zrobienia:** 30

---

## Notatki

### Decyzje Architektoniczne:
- **Code-First Approach**: Używamy EF Core migrations jako source of truth
- **Supabase Auth Integration**: JWT tokens z Supabase, walidacja w middleware
- **Clean Architecture**: Ścisłe rozdzielenie warstw, dependency flow: API → Infrastructure → Application → Domain

### Pytania/Uwagi:
- ❓ Czy Supabase Storage będzie wymagał custom buckets dla różnych typów plików?
- ❓ Jaki maksymalny rozmiar pliku w storage? (PRD mówi 10MB)
- ❓ Czy potrzebujemy rate limiting w MVP?

---

**Next Steps:** Po ukończeniu Fazy 0 przejdziemy do Fazy 1 (Domain Layer)
