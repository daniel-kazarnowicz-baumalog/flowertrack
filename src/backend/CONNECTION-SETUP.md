# FLOWerTRACK - Konfiguracja Połączenia Backend + Supabase

## ✅ Status Konfiguracji

Aplikacja backendowa została pomyślnie skonfigurowana i połączona z lokalną instancją **Supabase (PostgreSQL 17.6)**.

## 🗄️ Baza Danych - Supabase

### Uruchomiona Instancja Supabase
Lokalna instancja Supabase działa w Docker z następującymi usługami:

| Usługa | Port | Status | Opis |
|--------|------|--------|------|
| **PostgreSQL** | 54322 | ✅ Running | Główna baza danych |
| **Kong Gateway** | 54321 | ✅ Running | API Gateway |
| **Studio** | 54323 | ✅ Running | Supabase Studio UI |
| **Inbucket** | 54324 | ✅ Running | Email testing |
| **Analytics** | 54327 | ✅ Running | Analytics service |
| Auth, Storage, Realtime, etc. | - | ✅ Running | Pozostałe usługi |

### Dane Dostępowe do Bazy

```json
{
  "Host": "localhost",
  "Port": 54322,
  "Database": "postgres",
  "Username": "postgres",
  "Password": "postgres"
}
```

**Connection String:**
```
Host=localhost;Port=54322;Database=postgres;Username=postgres;Password=postgres
```

## 🚀 Aplikacja Backendowa - Flowertrack.Api

### Status
- ✅ Migracje zastosowane
- ✅ Dane przykładowe zainstalowane (5 organizacji)
- ✅ Aplikacja uruchomiona na `http://localhost:5102`

### Dostępne Endpointy

#### Swagger UI (Dokumentacja API)
```
http://localhost:5102/swagger
```

#### Health Checks
```
GET http://localhost:5102/health           # Status bazy danych
GET http://localhost:5102/api/health/ping  # Prosty ping
GET http://localhost:5102/api/health/version
```

#### Organizations API
```
GET http://localhost:5102/api/organizations       # Lista wszystkich organizacji
GET http://localhost:5102/api/organizations/{id}  # Szczegóły organizacji
GET http://localhost:5102/api/organizations/count # Statystyki
```

## 🔧 Konfiguracja - Pliki Zmienione

### 1. `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=54322;Database=postgres;Username=postgres;Password=postgres"
  }
}
```

### 2. `ApplicationDbContextFactory.cs`
Connection string zaktualizowany dla migracji EF Core:
```csharp
optionsBuilder.UseNpgsql("Host=localhost;Port=54322;Database=postgres;Username=postgres;Password=postgres");
```

### 3. `Migracja InitialCreate`
Poprawiona składnia SQL dla PostgreSQL:
- **Przed:** `[ApiKey] IS NOT NULL` (SQL Server)
- **Po:** `"ApiKey" IS NOT NULL` (PostgreSQL)

## 📦 Dane Przykładowe

Aplikacja automatycznie zainstalowała 5 przykładowych organizacji:

1. **Baumalog Sp. z o.o.** - Status: Active
2. **TechCorp Industries** - Status: Suspended
3. **Global Manufacturing Ltd** - Status: Active
4. **Smart Factory Solutions** - Status: Active
5. **Industrial Automation Co** - Status: Active

## 🛠️ Komendy Zarządzania

### Supabase

```powershell
# Uruchomienie Supabase
cd src/backend/supabase
& "$env:USERPROFILE\scoop\shims\supabase.exe" start

# Zatrzymanie Supabase
& "$env:USERPROFILE\scoop\shims\supabase.exe" stop

# Status Supabase
& "$env:USERPROFILE\scoop\shims\supabase.exe" status

# Sprawdzenie kontenerów Docker
docker ps --filter "name=supabase"
```

### Aplikacja Backend

```powershell
# Uruchomienie aplikacji
cd src/backend/Flowertrack.Api
dotnet run

# Build projektu
cd src/backend
dotnet build Flowertrack.sln

# Zastosowanie migracji
cd src/backend
dotnet ef database update --startup-project Flowertrack.Api --project Flowertrack.Infrastructure

# Dodanie nowej migracji
cd src/backend
dotnet ef migrations add NazwaMigracji --startup-project Flowertrack.Api --project Flowertrack.Infrastructure
```

## 🌐 Supabase Studio

Możesz zarządzać bazą danych przez przeglądarkę:

```
http://localhost:54323
```

Supabase Studio pozwala na:
- 📊 Przeglądanie tabel
- ✏️ Edycję danych
- 🔍 Wykonywanie zapytań SQL
- 👥 Zarządzanie użytkownikami
- 📈 Monitorowanie wydajności

## 📝 Logi

Logi aplikacji są zapisywane w:
```
src/backend/Flowertrack.Api/logs/flowertrack-YYYYMMDD.log
```

## ⚠️ Uwagi

### Ostrzeżenia przy kompilacji (nie krytyczne)
- `NU1902`: Pakiety JWT mają znane podatności (do aktualizacji w przyszłości)
- `NU1603`: Swashbuckle.AspNetCore 8.1.0 użyty zamiast 8.0.1

### Automatyczne procesy przy starcie aplikacji
1. ✅ Automatyczne zastosowanie migracji
2. ✅ Automatyczne seedowanie danych (jeśli baza jest pusta)
3. ✅ Weryfikacja połączenia z bazą danych

## 🎯 Następne Kroki

1. ⏳ Implementacja pozostałych encji (Ticket, Machine, User)
2. ⏳ CQRS pattern (Commands & Queries)
3. ⏳ Domain Events
4. ⏳ Authentication & Authorization z Supabase Auth
5. ⏳ GraphQL endpoint
6. ⏳ Testy integracyjne

## 🆘 Troubleshooting

### Problem: Port 54322 zajęty
```powershell
# Sprawdź co używa portu
netstat -ano | findstr :54322

# Zatrzymaj Supabase i uruchom ponownie
cd src/backend/supabase
& "$env:USERPROFILE\scoop\shims\supabase.exe" stop
& "$env:USERPROFILE\scoop\shims\supabase.exe" start
```

### Problem: Aplikacja nie łączy się z bazą
1. Sprawdź czy Supabase działa: `docker ps --filter "name=supabase_db"`
2. Sprawdź connection string w `appsettings.Development.json`
3. Sprawdź logi aplikacji w katalogu `logs/`

### Problem: Błąd migracji
1. Usuń bazę i zastosuj migracje ponownie:
```powershell
cd src/backend
dotnet ef database drop --startup-project Flowertrack.Api --project Flowertrack.Infrastructure
dotnet ef database update --startup-project Flowertrack.Api --project Flowertrack.Infrastructure
```

---

**Data konfiguracji:** 25.10.2025  
**Wersja .NET:** 10.0  
**Wersja PostgreSQL:** 17.6  
**Wersja Supabase CLI:** 2.53.6
