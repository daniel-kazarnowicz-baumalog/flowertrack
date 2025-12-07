# Plan Implementacji Frontend - Panel Serwisowy

Dokument ten opisuje plan implementacji ekranów dla panelu serwisowego (Service Panel) w oparciu o `Struktura Funkcjonalna Serwisu.md`.

## 1. Dashboard (Ekran Główny)
**Plik:** `src/pages/service/ServiceDashboard.tsx`

### Funkcjonalności do zrealizowania:
- [ ] **Kafelki KPI (Key Performance Indicators):**
    - Liczba aktywnych zgłoszeń (z podziałem na priorytety: Krytyczny, Wysoki, Normalny).
    - Liczba aktywnych alarmów z maszyn.
    - Liczba zgłoszeń rozwiązanych/zamkniętych (np. w tym tygodniu).
- [ ] **Wykresy:**
    - **Trend zgłoszeń:** Wykres liniowy/słupkowy pokazujący liczbę nowych zgłoszeń w czasie.
    - **Rozkład priorytetów:** Wykres kołowy pokazujący strukturę obecnych zgłoszeń.
- [ ] **Activity Feed (Ostatnia aktywność):**
    - Lista ostatnich zdarzeń w systemie (nowe zgłoszenie, zmiana statusu, nowy komentarz).
    - Sekcja "Moje tickety" - szybki dostęp do zgłoszeń przypisanych do zalogowanego serwisanta.
    - Sekcja "Organizacje z alarmami" - lista klientów wymagających uwagi.
    - Sekcja "Nadchodzące przeglądy" - maszyny, którym kończy się resurs.
- [ ] **Szybkie akcje:**
    - Przyciski skrótów do najczęstszych akcji (jeśli dotyczy).

## 2. Zgłoszenia (Moduł Zgłoszeń)
**Plik listy:** `src/pages/service/ServiceTicketsPage.tsx`
**Plik szczegółów:** `src/pages/service/ServiceTicketDetailPage.tsx`

### Lista Zgłoszeń (`ServiceTicketsPage`):
- [ ] **Tabela Zgłoszeń:**
    - Kolumny: ID, Temat, Organizacja, Maszyna, Status, Priorytet, Przypisany, Data utworzenia.
    - Wizualne wyróżnienie priorytetów i statusów.
- [ ] **Filtrowanie i Wyszukiwanie:**
    - Pasek wyszukiwania (tekstowy: temat, ID).
    - Filtry dropdown: Status, Priorytet, Organizacja, Serwisant (Ja / Wszyscy / Nieprzypisane).
- [ ] **Sortowanie:**
    - Możliwość sortowania po dacie (od najnowszych), priorytecie.

### Szczegóły Zgłoszenia (`ServiceTicketDetailPage`):
- [ ] **Nagłówek i Kontekst:**
    - Tytuł, ID, Status (z możliwością zmiany), Priorytet.
    - Linki do Organizacji i Maszyny.
    - Dane "Raw Data" z maszyny (jeśli dostępne w zgłoszeniu).
- [ ] **Workflow (Zmiana statusu):**
    - Obsługa przejść: Nowy -> Przyjęty -> W trakcie -> Rozwiązany -> Zamknięty.
    - Możliwość wznowienia zgłoszenia.
- [ ] **Przypisywanie:**
    - Dropdown do zmiany osoby przypisanej (Assignee).
    - Przycisk "Przypisz do mnie".
- [ ] **Timeline (Oś czasu):**
    - Chronologiczna lista zdarzeń: zmiany statusów, komentarze, notatki wewnętrzne.
    - **Chat:** Komunikacja z klientem.
    - **Notatki wewnętrzne:** Komentarze widoczne tylko dla serwisu.
    - **Załączniki:** Lista plików/zdjęć.

## 3. Klienci (Moduł Organizacji)
**Plik listy:** `src/pages/service/OrganizationsListPage.tsx`
**Plik szczegółów:** `src/pages/service/OrganizationDetailPage.tsx`

### Lista Organizacji (`OrganizationsListPage`):
- [ ] **Tabela Organizacji:**
    - Nazwa, Adres, Status umowy/serwisu, Liczba maszyn, Liczba aktywnych zgłoszeń.
- [ ] **Onboarding:**
    - Przycisk "Dodaj organizację" / "Zaproś klienta".
    - Modal z formularzem (Nazwa, Email administratora) wysyłający zaproszenie.

### Szczegóły Organizacji (`OrganizationDetailPage`):
- [ ] **Informacje podstawowe:**
    - Dane teleadresowe, osoby kontaktowe.
    - Edycja danych (dostępna dla Admina).
- [ ] **Lista Maszyn Klienta:**
    - Tabela maszyn należących do tej organizacji.
    - Statusy maszyn.
- [ ] **Zarządzanie Maszynami:**
    - Dodawanie/Przypisanie maszyny do organizacji.
    - Generowanie/Odświeżanie tokenu API dla maszyny.

## 4. Maszyny
**Plik listy:** `src/pages/service/MachinesListPage.tsx`
**Plik szczegółów:** `src/pages/service/MachineDetailPage.tsx`

### Lista Maszyn (`MachinesListPage`):
- [ ] **Globalna lista maszyn:**
    - Przegląd wszystkich maszyn w systemie.
    - Filtrowanie po Organizacji, Statusie (Online/Offline/Alarm).
- [ ] **Statusy:**
    - Szybki podgląd stanu technicznego.

### Szczegóły Maszyny (`MachineDetailPage`):
- [ ] **Dane techniczne:** Model, Numer seryjny, Rok produkcji, Data instalacji.
- [ ] **Telemetria:**
    - Podgląd ostatnich danych z maszyny (jeśli zintegrowane).
- [ ] **Historia serwisowa:**
    - Lista zgłoszeń powiązanych z tą maszyną.
- [ ] **Przeglądy:**
    - Informacja o ostatnim i planowanym przeglądzie.

## 5. Zespół (Moduł Administracji)
**Plik listy:** `src/pages/service/ServiceUsersListPage.tsx`
*(Dostępne tylko dla roli Administratora)*

### Zarządzanie Zespołem:
- [ ] **Lista Serwisantów:**
    - Imię, Nazwisko, Email, Rola (Serwisant/Admin), Status (Aktywny/Zablokowany).
    - Wskaźniki aktywności (np. ostatnie logowanie).
- [ ] **Zarządzanie Kontami:**
    - Dodawanie nowego użytkownika serwisowego.
    - Edycja danych i uprawnień.
    - Resetowanie hasła.
    - Blokowanie/Odblokowanie dostępu.

---

## Kolejność Realizacji (Sugerowana)

1. **Dashboard** - aby mieć punkt wyjścia i widok ogólny.
2. **Zgłoszenia (Lista + Szczegóły)** - kluczowa funkcjonalność systemu ("Core").
3. **Klienci i Maszyny** - niezbędne do kontekstu zgłoszeń.
4. **Zespół** - funkcjonalność administracyjna.
