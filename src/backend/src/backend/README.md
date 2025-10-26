# FLOWerTRACK Backend - Clean Architecture

## Struktura Projektu

Projekt jest zorganizowany zgodnie z zasadami **Clean Architecture**, **Domain-Driven Design (DDD)** i **CQRS**.

```
src/backend/
??? Core/             # Warstwa g��wna (Core/Domain)
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

## Zasady Zale�no�ci (Dependency Rule)

```
???????????????????????????????????????????????
?           Presentation Layer      ?
?  (API, Contracts)       ?
?  Zale�no�ci: Application, Infrastructure ?
???????????????????????????????????????????????
       ?
???????????????????????????????????????????????
?         Infrastructure Layer                ?
?  (EF Core, Repositories, External Services) ?
?  Zale�no�ci: Application, Domain    ?
???????????????????????????????????????????????
                ?
???????????????????????????????????????????????
?  Application Layer     ?
?  (Use Cases, CQRS Handlers, Interfaces)     ?
?  Zale�no�ci: Domain         ?
???????????????????????????????????????????????
      ?
???????????????????????????????????????????????
?            Domain Layer   ?
?  (Entities, Value Objects, Domain Events)   ?
?  Zale�no�ci: BRAK (niezale�na warstwa)      ?
???????????????????????????????????????????????
```

## Opis Warstw

### ?? Core/Domain (`Flowertrack.Domain`)
**Cel:** Reprezentacja logiki biznesowej i regu� domenowych.

**Zawiera:**
- **Entities** - Obiekty z to�samo�ci� (np. `Ticket`, `User`, `Machine`)
- **Value Objects** - Obiekty bez to�samo�ci (np. `Email`, `PhoneNumber`, `Address`)
- **Domain Events** - Wydarzenia domenowe (np. `TicketCreatedEvent`)
- **Aggregates** - Agregaty DDD (granice sp�jno�ci transakcyjnej)
- **Domain Services** - Logika biznesowa wymagaj�ca wielu encji
- **Exceptions** - Wyj�tki domenowe

**Zale�no�ci:** BRAK (warstwa ca�kowicie niezale�na)

---

### ?? Core/Application (`Flowertrack.Application`)
**Cel:** Orkiestracja logiki biznesowej, implementacja przypadk�w u�ycia.

**Zawiera:**
- **Commands** - Operacje zmieniaj�ce stan (CQRS)
- **Queries** - Operacje odczytuj�ce dane (CQRS)
- **Handlers** - Handlery dla Commands i Queries
- **DTOs** - Obiekty transferu danych wewn�trzne
- **Interfaces** - Abstrakcje dla repozytori�w, serwis�w zewn�trznych
- **Validators** - Walidatory (FluentValidation)
- **Mappers** - Mapowanie mi�dzy encjami a DTOs

**Zale�no�ci:** `Flowertrack.Domain`

---

### ?? Infrastructure (`Flowertrack.Infrastructure`)
**Cel:** Implementacja szczeg��w technicznych i komunikacji z zewn�trznymi systemami.

**Zawiera:**
- **DbContext** - Entity Framework Core DbContext
- **Repositories** - Implementacje repozytori�w
- **Migrations** - Migracje bazy danych
- **External Services** - Integracje (Supabase, Email, SMS)
- **Configuration** - Konfiguracja Entity Framework
- **Health Checks** - Sprawdzanie zdrowia aplikacji

**Zale�no�ci:** `Flowertrack.Application`, `Flowertrack.Domain`

---

### ?? Presentation/Api (`Flowertrack.Api`)
**Cel:** Punkt wej�cia aplikacji, obs�uga ��da� HTTP.

**Zawiera:**
- **Controllers** - Kontrolery Web API
- **Middleware** - Niestandardowe middleware (np. error handling)
- **Filters** - Filtry akcji i autoryzacji
- **Extensions** - Metody rozszerzaj�ce dla konfiguracji
- **Program.cs** - Konfiguracja aplikacji i Dependency Injection

**Zale�no�ci:** `Flowertrack.Application`, `Flowertrack.Infrastructure`, `Flowertrack.Contracts`

---

### ?? Presentation/Contracts (`Flowertrack.Contracts`)
**Cel:** Kontrakty API wsp�dzielone z klientami.

**Zawiera:**
- **Request Models** - Modele ��da� API
- **Response Models** - Modele odpowiedzi API
- **DTOs** - Obiekty transferu danych dla API
- **Enums** - Wyliczenia u�ywane w API

**Zale�no�ci:** BRAK (mo�e by� wsp�dzielony z frontendem)

---

### ?? Tests
**Cel:** Zapewnienie jako�ci kodu poprzez testy.

**Projekty:**
- `Flowertrack.Domain.Tests` - Testy jednostkowe logiki domenowej (xUnit, FluentAssertions, Moq)
- `Flowertrack.Application.Tests` - Testy jednostkowe use cases (xUnit, Moq)
- `Flowertrack.Infrastructure.Tests` - Testy jednostkowe infrastruktury (xUnit, Moq, InMemory DB)
- `Flowertrack.Api.Tests` - Testy jednostkowe kontroler�w (xUnit)
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

# Build ca�ego solution
dotnet build

# Build konkretnego projektu
dotnet build Presentation/Flowertrack.Api/Flowertrack.Api.csproj

# Run API
dotnet run --project Presentation/Flowertrack.Api/Flowertrack.Api.csproj

# Run test�w
dotnet test

# Run test�w z coverage
dotnet test /p:CollectCoverage=true
```

## Dodawanie Nowych Projekt�w

Je�li potrzebujesz doda� nowy projekt do solution:

```powershell
# Dodaj projekt do solution
dotnet sln add �cie�ka/do/projektu.csproj

# Dodaj referencj� mi�dzy projektami
dotnet add projekt1.csproj reference projekt2.csproj
```

## Zasady Kodowania

1. **Warstwa Domain** nie mo�e zale�e� od �adnej innej warstwy
2. **Warstwa Application** zale�y tylko od Domain
3. **Infrastructure** implementuje interfejsy z Application
4. **Presentation** ��czy wszystko razem i rejestruje DI
5. Stosuj **CQRS** dla oddzielenia odczyt�w i zapis�w
6. U�ywaj **Dependency Injection** dla wszystkich zale�no�ci
7. Wszystkie operacje I/O powinny by� **asynchroniczne**
8. Stosuj **Repository Pattern** dla dost�pu do danych

## Konwencje Nazewnictwa

- **Commands**: `CreateTicketCommand`, `UpdateUserCommand`
- **Queries**: `GetTicketByIdQuery`, `GetAllUsersQuery`
- **Handlers**: `CreateTicketCommandHandler`, `GetTicketByIdQueryHandler`
- **Entities**: `Ticket`, `User`, `Machine`
- **Value Objects**: `Email`, `PhoneNumber`, `Address`
- **Controllers**: `TicketsController`, `UsersController`

## Migracje Entity Framework

```powershell
# Dodaj now� migracj�
dotnet ef migrations add NazwaMigracji --project Infrastructure/Flowertrack.Infrastructure

# Zaktualizuj baz� danych
dotnet ef database update --project Infrastructure/Flowertrack.Infrastructure

# Usu� ostatni� migracj�
dotnet ef migrations remove --project Infrastructure/Flowertrack.Infrastructure
```

---

**Wersja:** 1.0  
**Ostatnia aktualizacja:** 2025-01-26
