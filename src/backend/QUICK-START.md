# FLOWerTRACK Backend - Quick Start Guide

## Wymagania

- .NET 10 SDK (RC2 lub nowszy)
- PostgreSQL 12+ (lokalnie zainstalowany lub Docker)
- IDE: Visual Studio 2022, VS Code lub Rider

## Konfiguracja PostgreSQL

### Opcja 1: Używanie lokalnego PostgreSQL

Jeśli masz zainstalowany PostgreSQL lokalnie:

1. Otwórz pgAdmin lub psql
2. Uruchom skrypt inicjalizacyjny:
   ```bash
   psql -U postgres -f scripts/init-database.sql
   ```

Alternatywnie, utwórz bazę ręcznie:
```sql
CREATE DATABASE flowertrack_dev;
```

### Opcja 2: Docker PostgreSQL

```bash
docker run --name flowertrack-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=flowertrack_dev -p 5432:5432 -d postgres:16
```

## Konfiguracja połączenia

Zaktualizuj connection string w `appsettings.Development.json` według swoich potrzeb:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=flowertrack_dev;Username=postgres;Password=twoje_haslo"
  }
}
```

**WAŻNE:** To samo musisz zrobić w pliku `Flowertrack.Infrastructure/Persistence/ApplicationDbContextFactory.cs` dla migracji.

## Uruchomienie aplikacji

### Sposób 1: Automatyczne migracje i seed (ZALECANY)

Po prostu uruchom aplikację - migracje i seedowanie danych odbędą się automatycznie:

```bash
cd Flowertrack.Api
dotnet run
```

Aplikacja:
1. Automatycznie zastosuje migracje do bazy danych
2. Zainstaluje przykładowe dane (5 organizacji)
3. Uruchomi się na `http://localhost:5102`

### Sposób 2: Ręczne migracje

Jeśli chcesz ręcznie zarządzać migracjami:

```bash
# Z katalogu src/backend
dotnet ef database update --startup-project Flowertrack.Api --project Flowertrack.Infrastructure
```

## Sprawdzenie działania

Po uruchomieniu aplikacji otwórz:

- **Swagger UI**: http://localhost:5102/swagger
- **Health Check**: http://localhost:5102/health
- **Health Ping**: http://localhost:5102/api/health/ping
- **Organizations**: http://localhost:5102/api/organizations

## Przykładowe dane

Aplikacja automatycznie zainstaluje 5 przykładowych organizacji:

1. **Baumalog Sp. z o.o.** - Status: Active (z notatką)
2. **TechCorp Industries** - Status: Suspended
3. **Global Manufacturing Ltd** - Status: Active (z notatką)
4. **Smart Factory Solutions** - Status: Active
5. **Industrial Automation Co** - Status: Active

Każda organizacja ma:
- Wygenerowany API Key
- Pełne dane adresowe
- Dane kontaktowe
- Status serwisu

## Dostępne endpointy

### Health
- `GET /health` - Status bazy danych
- `GET /api/health/ping` - Prosty ping
- `GET /api/health/version` - Informacje o wersji

### Organizations
- `GET /api/organizations` - Lista wszystkich organizacji
- `GET /api/organizations/{id}` - Szczegóły organizacji
- `GET /api/organizations/count` - Statystyki organizacji

## Logowanie

Logi są zapisywane w:
- Konsola (real-time)
- `logs/flowertrack-YYYYMMDD.log` (pliki dzienne)

## Struktura projektu

```
backend/
├── Flowertrack.Api/              # API Entry Point
│   ├── Controllers/              # REST Controllers
│   └── Program.cs               # Application startup
├── Flowertrack.Domain/          # Domain Layer (Clean Architecture)
│   ├── Common/                  # Base classes (Entity, ValueObject)
│   ├── Entities/                # Domain entities (Organization)
│   └── ValueObjects/            # Enums and value objects
├── Flowertrack.Infrastructure/  # Infrastructure Layer
│   ├── Data/                    # Database seeder
│   ├── Persistence/             # EF Core context & configurations
│   └── Migrations/              # EF Core migrations
└── Flowertrack.Application/     # Application Layer (CQRS będzie tutaj)
```

## Następne kroki

1. ✅ Podstawowa infrastruktura
2. ✅ Encja Organization
3. ✅ Migracje i seed
4. ⏳ Pozostałe encje (Ticket, Machine, User)
5. ⏳ CQRS (Commands & Queries)
6. ⏳ Domain Events
7. ⏳ Authentication & Authorization
8. ⏳ GraphQL endpoint

## Troubleshooting

### Błąd połączenia z bazą
- Sprawdź czy PostgreSQL działa: `Test-NetConnection -ComputerName localhost -Port 5432`
- Sprawdź hasło w connection string
- Sprawdź czy baza `flowertrack_dev` istnieje

### Błąd podczas migracji
- Upewnij się że connection string jest identyczny w:
  - `appsettings.Development.json`
  - `ApplicationDbContextFactory.cs`

### Brak przykładowych danych
- Usuń bazę i pozwól aplikacji utworzyć ją ponownie
- Sprawdź logi w konsoli lub w pliku `logs/`

## Kontakt

W razie problemów sprawdź:
- Logi aplikacji w `logs/`
- Swagger UI pod `/swagger`
- Dokumentację w katalogu `.ai/`
