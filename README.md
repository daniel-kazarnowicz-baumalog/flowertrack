# FLOWerTRACK

FLOWerTRACK to zaawansowany system zarządzania zgłoszeniami serwisowymi zaprojektowany dla zespołów serwisowych oraz ich klientów. Umożliwia centralizację zgłoszeń, śledzenie statusów napraw, audyt zmian oraz współpracę między serwisem a klientami.

## Najważniejsze cele
- Skrócenie czasu obsługi zgłoszeń
- Pełna przejrzystość procesu serwisowego
- Automatyzacja flow zgłoszeń od utworzenia do zamknięcia
- Integracja logów z maszyn produkcyjnych
- Ułatwienie przydzielania i monitorowania zadań serwisowych

## Główne grupy użytkowników
- Administratorzy Serwisu
- Serwisanci
- Administratorzy Organizacji Klientów
- Operatorzy Maszyn

## Najważniejsze moduły (MVP)
- Portal Serwisu
  - Logowanie i uwierzytelnianie
  - Dashboard (KPI, wykresy trendów)
  - Lista zgłoszeń i szczegóły zgłoszenia (timeline, załączniki, audyt)
  - Zarządzanie Organizacjami i Maszynami
  - Panel Administracyjny (zarządzanie serwisantami i uprawnieniami)

- Portal Klienta
  - Logowanie i aktywacja konta
  - Dashboard klienta z widokiem maszyn i zgłoszeń
  - Tworzenie i edycja zgłoszeń, dodawanie załączników
  - Zarządzanie użytkownikami organizacji (zaproszenia)

## Granice MVP (co jest wyłączone)
- Real-time push/email (poza onboarding/login)
- Chat real-time (komentarze asynchroniczne zamiast live chat)
- Zaawansowane raportowanie i integracje zewnętrzne w MVP
- Automatyczne przydzielanie zgłoszeń i złożone SLA

## Tech stack (wykryty w repo)
- Frontend: React, TypeScript, Vite (znaleziono `src/frontend/flowertrack-client`)
- Backend: .NET (rozwiązanie `src/backend/Flowertrack.sln` z projektem `Flowertrack.Api`)

## Struktura repo
- `.ai/` – dokumentacja produktowa i plany (PRD, tech-stack, struktury funkcjonalne)
- `src/frontend/flowertrack-client` – kod frontendu (Vite + React + TS)
- `src/backend/Flowertrack.Api` – kod backendu (.NET)
- `.gitignore` – zaktualizowane ignorowane pliki dla frontend (.vite, node_modules) i backend (bin/, obj/, .vs/)

## Quick start (lokalnie)
Frontend (wejdź do katalogu klienta i zainstaluj zależności):

```powershell
cd src/frontend/flowertrack-client
pnpm install  # lub npm install
pnpm run dev   # lub npm run dev
```

Backend (.NET):

```powershell
cd src/backend/Flowertrack.Api
dotnet build
dotnet run
```

Uwagi:
- Domyślne pliki konfiguracyjne środowiska (np. `.env`) są ignorowane przez `.gitignore`.
- Maksymalny rozmiar załączników w MVP: 10 MB.

## Dalsze kroki (sugestie)
- Dodać README w `src/backend` i `src/frontend` z instrukcjami uruchomienia specyficznymi dla tych modułów.
- Dodać konfigurację CI/CD i skrypty do uruchamiania testów.
- W `.ai` można umieścić gotowe prompty, scenariusze testowe i modele dokumentacji.

---

Pliki referencyjne: `.ai/PRD.md`, `.ai/Tech-stack.md`, `src/frontend/flowertrack-client/package.json`, `src/backend/Flowertrack.sln`
