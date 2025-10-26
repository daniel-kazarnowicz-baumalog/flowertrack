# FLOWerTRACK Backend - Clean Architecture

## Struktura Projektu

Projekt jest zorganizowany zgodnie z zasadami **Clean Architecture**, **Domain-Driven Design (DDD)** i **CQRS**.

```
src/backend/
??? Core/             # Warstwa g³ówna (Core/Domain)
? ??? Flowertrack.Domain/        # Encje domenowe, Value Objects, Domain Events, Agregaty
?   ??? Flowertrack.Application/   # Use Cases, CQRS (Commands/Queries), Application Services, Interfaces
?
??? Infrastructure/          # Warstwa infrastruktury
? ??? Flowertrack.Infrastructure/ # Implementacje (Entity Framework, Repositories, External Services, Supabase)
?
??? Presentation/        # Warstwa prezentacji
?   ??? Flowertrack.Api/           # ASP.NET Core Web API (Controllers, Middleware, Filters)
?   ??? Flowertrack.Contracts/     # DTOs, Request/Response models, API Contracts
?
??? Tests/           # Wszystkie projekty testowe
    ??? Flowertrack.Domain.Tests/           # Testy jednostkowe warstwy domenowej
  ??? Flowertrack.Application.Tests/      # Testy jednostkowe warstwy aplikacji
 ??? Flowertrack.Infrastructure.Tests/   # Testy jednostkowe infrastruktury
    ??? Flowertrack.Api.Tests/              # Testy jednostkowe API
    ??? Flowertrack.Api.IntegrationTests/   # Testy integracyjne API
```

## Zasady Zale¿noœci (Dependency Rule)

```
???????????????????????????????????????????????
?           Presentation Layer      ?
?  (API, Contracts)       ?
?  Zale¿noœci: Application, Infrastructure ?
???????????????????????????????????????????????
       ?
???????????????????????????????????????????????
?         Infrastructure Layer                ?
?  (EF Core, Repositories, External Services) ?
?  Zale¿noœci: Application, Domain    ?
???????????????????????????????????????????????
                ?
???????????????????????????????????????????????
?  Application Layer     ?
?  (Use Cases, CQRS Handlers, Interfaces)     ?
?  Zale¿noœci: Domain         ?
???????????????????????????????????????????????
      ?
???????????????????????????????????????????????
?            Domain Layer   ?
?  (Entities, Value Objects, Domain Events)   ?
?  Zale¿noœci: BRAK (niezale¿na warstwa)      ?
???????????????????????????????????????????????
```

## Opis Warstw

### ?? Core/Domain (`Flowertrack.Domain`)
**Cel:** Reprezentacja logiki biznesowej i regu³ domenowych.

**Zawiera:**
- **Entities** - Obiekty z to¿samoœci¹ (np. `Ticket`, `User`, `Machine`)
- **Value Objects** - Obiekty bez to¿samoœci (np. `Email`, `PhoneNumber`, `Address`)
- **Domain Events** - Wydarzenia domenowe (np. `TicketCreatedEvent`)
- **Aggregates** - Agregaty DDD (granice spójnoœci transakcyjnej)
- **Domain Services** - Logika biznesowa wymagaj¹ca wielu encji
- **Exceptions** - Wyj¹tki domenowe

**Zale¿noœci:** BRAK (warstwa ca³kowicie niezale¿na)

---

### ?? Core/Application (`Flowertrack.Application`)
**Cel:** Orkiestracja logiki biznesowej, implementacja przypadków u¿ycia.

**Zawiera:**
- **Commands** - Operacje zmieniaj¹ce stan (CQRS)
- **Queries** - Operacje odczytuj¹ce dane (CQRS)
- **Handlers** - Handlery dla Commands i Queries
- **DTOs** - Obiekty transferu danych wewnêtrzne
- **Interfaces** - Abstrakcje dla repozytoriów, serwisów zewnêtrznych
- **Validators** - Walidatory (FluentValidation)
- **Mappers** - Mapowanie miêdzy encjami a DTOs

**Zale¿noœci:** `Flowertrack.Domain`

---

### ?? Infrastructure (`Flowertrack.Infrastructure`)
**Cel:** Implementacja szczegó³ów technicznych i komunikacji z zewnêtrznymi systemami.

**Zawiera:**
- **DbContext** - Entity Framework Core DbContext
- **Repositories** - Implementacje repozytoriów
- **Migrations** - Migracje bazy danych
- **External Services** - Integracje (Supabase, Email, SMS)
- **Configuration** - Konfiguracja Entity Framework
- **Health Checks** - Sprawdzanie zdrowia aplikacji

**Zale¿noœci:** `Flowertrack.Application`, `Flowertrack.Domain`

---

### ?? Presentation/Api (`Flowertrack.Api`)
**Cel:** Punkt wejœcia aplikacji, obs³uga ¿¹dañ HTTP.

**Zawiera:**
- **Controllers** - Kontrolery Web API
- **Middleware** - Niestandardowe middleware (np. error handling)
- **Filters** - Filtry akcji i autoryzacji
- **Extensions** - Metody rozszerzaj¹ce dla konfiguracji
- **Program.cs** - Konfiguracja aplikacji i Dependency Injection

**Zale¿noœci:** `Flowertrack.Application`, `Flowertrack.Infrastructure`, `Flowertrack.Contracts`

---

### ?? Presentation/Contracts (`Flowertrack.Contracts`)
**Cel:** Kontrakty API wspó³dzielone z klientami.

**Zawiera:**
- **Request Models** - Modele ¿¹dañ API
- **Response Models** - Modele odpowiedzi API
- **DTOs** - Obiekty transferu danych dla API
- **Enums** - Wyliczenia u¿ywane w API

**Zale¿noœci:** BRAK (mo¿e byæ wspó³dzielony z frontendem)

---

### ?? Tests
**Cel:** Zapewnienie jakoœci kodu poprzez testy.

**Projekty:**
- `Flowertrack.Domain.Tests` - Testy jednostkowe logiki domenowej (xUnit, FluentAssertions, Moq)
- `Flowertrack.Application.Tests` - Testy jednostkowe use cases (xUnit, Moq)
- `Flowertrack.Infrastructure.Tests` - Testy jednostkowe infrastruktury (xUnit, Moq, InMemory DB)
- `Flowertrack.Api.Tests` - Testy jednostkowe kontrolerów (xUnit)
- `Flowertrack.Api.IntegrationTests` - Testy integracyjne end-to-end (xUnit, WebApplicationFactory)

## Technologie

- **.NET 10.0** - Framework aplikacji
- **ASP.NET Core** - Web API
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** (Npgsql) - Baza danych
- **Supabase** - Backend as a Service
- **Serilog** - Logging
- **JWT Bearer** - Autentykacja
- **Swagger/OpenAPI** - Dokumentacja API
- **xUnit** - Framework testowy
- **Moq** - Mocking framework
- **FluentAssertions** - Asercje w testach

## Komendy Budowania

```powershell
# Restore dependencies
dotnet restore

# Build ca³ego solution
dotnet build

# Build konkretnego projektu
dotnet build Presentation/Flowertrack.Api/Flowertrack.Api.csproj

# Run API
dotnet run --project Presentation/Flowertrack.Api/Flowertrack.Api.csproj

# Run testów
dotnet test

# Run testów z coverage
dotnet test /p:CollectCoverage=true
```

## Dodawanie Nowych Projektów

Jeœli potrzebujesz dodaæ nowy projekt do solution:

```powershell
# Dodaj projekt do solution
dotnet sln add œcie¿ka/do/projektu.csproj

# Dodaj referencjê miêdzy projektami
dotnet add projekt1.csproj reference projekt2.csproj
```

## Zasady Kodowania

1. **Warstwa Domain** nie mo¿e zale¿eæ od ¿adnej innej warstwy
2. **Warstwa Application** zale¿y tylko od Domain
3. **Infrastructure** implementuje interfejsy z Application
4. **Presentation** ³¹czy wszystko razem i rejestruje DI
5. Stosuj **CQRS** dla oddzielenia odczytów i zapisów
6. U¿ywaj **Dependency Injection** dla wszystkich zale¿noœci
7. Wszystkie operacje I/O powinny byæ **asynchroniczne**
8. Stosuj **Repository Pattern** dla dostêpu do danych

## Konwencje Nazewnictwa

- **Commands**: `CreateTicketCommand`, `UpdateUserCommand`
- **Queries**: `GetTicketByIdQuery`, `GetAllUsersQuery`
- **Handlers**: `CreateTicketCommandHandler`, `GetTicketByIdQueryHandler`
- **Entities**: `Ticket`, `User`, `Machine`
- **Value Objects**: `Email`, `PhoneNumber`, `Address`
- **Controllers**: `TicketsController`, `UsersController`

## Migracje Entity Framework

```powershell
# Dodaj now¹ migracjê
dotnet ef migrations add NazwaMigracji --project Infrastructure/Flowertrack.Infrastructure

# Zaktualizuj bazê danych
dotnet ef database update --project Infrastructure/Flowertrack.Infrastructure

# Usuñ ostatni¹ migracjê
dotnet ef migrations remove --project Infrastructure/Flowertrack.Infrastructure
```

---

**Wersja:** 1.0  
**Ostatnia aktualizacja:** 2025-01-26
