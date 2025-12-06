# Faza 2 - Plan Implementacji Ważnych Komponentów

**Data utworzenia:** 2025-12-06  
**Status:** Plan implementacji  
**Cel:** Zdefiniowanie wszystkich brakujących komponentów wymaganych do pełnego MVP zgodnie z PRD

---

## 📊 Analiza Pokrycia User Stories

### ✅ Zaimplementowane User Stories (Backend)

| US | Tytuł | Moduł | Status |
|----|-------|-------|--------|
| US-001 | Zalogowanie serwisanta | Auth | ✅ AuthController |
| US-004 | Zalogowanie operatora | Auth | ✅ AuthController |
| US-006 | Kafelki KPI Dashboard | Dashboard | ✅ GetServiceDashboardQuery |
| US-010 | Lista zgłoszeń | Tickets | ✅ TicketsController |
| US-011 | Filtrowanie zgłoszeń | Tickets | ✅ GetTicketsQuery |
| US-012 | Wyszukiwanie zgłoszeń | Tickets | ✅ Search w GetTicketsQuery |
| US-013 | Grupowanie wg statusu | Tickets | ✅ GetTicketsGroupedByStatusQuery |
| US-015 | Szczegóły zgłoszenia | Tickets | ✅ GetTicketByIdQuery |
| US-016 | Oś czasu (timeline) | Tickets | ✅ GetTicketHistoryQuery |
| US-017 | Zmiana statusu | Tickets | ✅ UpdateTicketStatusCommand |
| US-018 | Notatki wewnętrzne | Tickets | ✅ AddTicketNoteCommand |
| US-019 | Przypisanie ticketa | Tickets | ✅ AssignTicketCommand |
| US-020 | Komentarze | Tickets | ✅ AddCommentCommand |
| US-023 | Załączniki | Tickets | ✅ TicketAttachmentsController |
| US-024 | Lista organizacji | Organizations | ✅ OrganizationsController |
| US-025 | Onboarding organizacji | Organizations | ✅ OnboardOrganizationCommand |
| US-026 | Edycja organizacji | Organizations | ✅ UpdateOrganizationCommand |
| US-027 | Zarządzanie maszynami | Machines | ✅ MachinesController |
| US-028 | Token API maszyn | Machines | ✅ RegenerateMachineApiTokenCommand |
| US-029 | Statusy maszyn i alerty | Machines | ✅ MachineLog + IngestController |
| US-031 | Lista serwisantów | Users | ✅ UsersController |
| US-032 | Dodawanie serwisanta | Users | ✅ InviteServiceUserCommand |
| US-037 | Dashboard organizacji | Dashboard | ✅ GetOrganizationDashboardQuery |
| US-040 | Lista zgłoszeń klienta | Tickets | ✅ GetTicketsQuery (RLS) |
| US-042 | Tworzenie zgłoszenia | Tickets | ✅ CreateTicketCommand |
| US-049 | Lista operatorów | Users | ✅ GetOrganizationUsersQuery |
| US-050 | Dodawanie operatora | Users | ✅ InviteOrganizationUserCommand |
| US-053 | RBAC | Auth | ✅ Authorization Policies |
| US-054 | Audyt bezpieczeństwa | Audit | ✅ AuditLog + AdminController |

---

## 🚨 Brakujące/Niekompletne User Stories

### Priorytet 1: Krytyczne dla MVP 🔴

#### US-002: Reset hasła
**Status:** ⚠️ Częściowo (endpoint istnieje, brak wysyłki email)  
**Brakuje:**
- Produkcyjna implementacja EmailService
- Szablony email dla resetowania hasła
- Endpoint do ustawiania nowego hasła po kliknięciu linku

**Pliki do modyfikacji/utworzenia:**
```
Application/Auth/Commands/ForgotPassword/
Application/Auth/Commands/ResetPassword/
Infrastructure/Services/EmailService.cs (produkcyjna implementacja)
Infrastructure/Templates/PasswordResetEmail.html
```

---

#### US-005: Aktywacja konta przez link
**Status:** ⚠️ Częściowo (zaproszenie działa, brak pełnego flow aktywacji)  
**Brakuje:**
- Endpoint do weryfikacji tokenu aktywacyjnego
- Endpoint do ustawienia hasła przy aktywacji
- Szablon email dla aktywacji

**Pliki do utworzenia:**
```
Application/Auth/Commands/ActivateAccount/ActivateAccountCommand.cs
Application/Auth/Commands/ActivateAccount/ActivateAccountCommandHandler.cs
Application/Auth/Queries/ValidateActivationToken/ValidateActivationTokenQuery.cs
API/Controllers/AuthController.cs (rozszerzenie)
```

---

#### US-007: Wykres trendów zgłoszeń
**Status:** ❌ Brak  
**Opis:** Wykres liniowy trendów z ostatnich 30 dni, rozkład po priorytetach

**Pliki do utworzenia:**
```
Application/Dashboard/Queries/GetTicketTrends/GetTicketTrendsQuery.cs
Application/Dashboard/Queries/GetTicketTrends/GetTicketTrendsQueryHandler.cs
Application/Dashboard/Queries/GetTicketTrends/TicketTrendsDto.cs
API/Controllers/DashboardController.cs (rozszerzenie)
```

**Endpoint:**
```
GET /api/dashboard/trends?days=30
Response: { dates: [], opened: [], resolved: [], closed: [], byPriority: {} }
```

---

#### US-008: Lista ostatnich zdarzeń
**Status:** ❌ Brak  
**Opis:** Sekcja "Ostatnie zdarzenia" z listą 10 ostatnich zmian

**Pliki do utworzenia:**
```
Application/Dashboard/Queries/GetRecentActivities/GetRecentActivitiesQuery.cs
Application/Dashboard/Queries/GetRecentActivities/GetRecentActivitiesQueryHandler.cs
Application/Dashboard/Queries/GetRecentActivities/RecentActivityDto.cs
```

**Endpoint:**
```
GET /api/dashboard/recent-activities?limit=10
```

---

#### US-014: Masowe akcje na zgłoszeniach
**Status:** ❌ Brak  
**Opis:** Bulk assign, bulk status change, bulk archive

**Pliki do utworzenia:**
```
Application/Tickets/Commands/BulkAssignTickets/BulkAssignTicketsCommand.cs
Application/Tickets/Commands/BulkAssignTickets/BulkAssignTicketsCommandHandler.cs
Application/Tickets/Commands/BulkChangeStatus/BulkChangeStatusCommand.cs
Application/Tickets/Commands/BulkChangeStatus/BulkChangeStatusCommandHandler.cs
Application/Tickets/Commands/BulkArchiveTickets/BulkArchiveTicketsCommand.cs
```

**Endpoints:**
```
POST /api/tickets/bulk/assign
POST /api/tickets/bulk/status
POST /api/tickets/bulk/archive
```

---

#### US-021: Uzasadnienie do zmian statusu
**Status:** ⚠️ Częściowo (jest pole reason, brak walidacji obowiązkowości)  
**Brakuje:**
- Walidacja: uzasadnienie obowiązkowe dla Resolved/Closed
- Minimum 10 znaków

**Pliki do modyfikacji:**
```
Application/Tickets/Commands/UpdateTicketStatus/UpdateTicketStatusCommandValidator.cs
```

---

#### US-030: Konfiguracja przeglądów technicznych
**Status:** ❌ Brak  
**Opis:** Planowanie i śledzenie przeglądów serwisowych maszyn

**Pliki do utworzenia:**
```
Domain/Entities/MaintenanceSchedule.cs
Domain/Repositories/IMaintenanceScheduleRepository.cs
Application/Machines/Commands/ScheduleMaintenance/
Application/Machines/Queries/GetUpcomingMaintenances/
Infrastructure/Persistence/Configurations/MaintenanceScheduleConfiguration.cs
```

**Endpoints:**
```
POST /api/machines/{id}/maintenance
GET /api/machines/{id}/maintenance-history
GET /api/dashboard/upcoming-maintenances
```

---

#### US-033: Edycja konta serwisanta
**Status:** ⚠️ Częściowo  
**Brakuje:**
- Pełna walidacja zmiany emaila
- Historia zmian w audycie

---

#### US-034: Resetowanie hasła serwisanta
**Status:** ⚠️ Częściowo (endpoint istnieje, brak wysyłki email)

---

#### US-035: Zarządzanie uprawnieniami
**Status:** ⚠️ Częściowo  
**Brakuje:**
- Walidacja - nie można usunąć ostatniego admina
- Email z powiadomieniem o zmianie roli

---

#### US-036: Dezaktywacja serwisanta
**Status:** ✅ Zaimplementowane  

---

#### US-038: Ostatnie aktywności (Portal Klienta)
**Status:** ❌ Brak  

**Pliki do utworzenia:**
```
Application/Dashboard/Queries/GetOrganizationActivities/GetOrganizationActivitiesQuery.cs
```

---

#### US-043: Porzucanie zgłoszeń
**Status:** ❌ Brak  
**Opis:** Możliwość "porzucenia" zgłoszenia w statusie Draft/New

**Pliki do utworzenia:**
```
Application/Tickets/Commands/AbandonTicket/AbandonTicketCommand.cs
Application/Tickets/Commands/AbandonTicket/AbandonTicketCommandHandler.cs
```

**Endpoint:**
```
POST /api/tickets/{id}/abandon
```

---

#### US-047: Wznowienie rozwiązanego zgłoszenia
**Status:** ⚠️ Częściowo (status Reopened istnieje, brak walidacji 14 dni)  
**Brakuje:**
- Walidacja: można wznowić tylko w ciągu 14 dni od rozwiązania
- Obowiązkowe pole "przyczyna wznowienia"

**Pliki do modyfikacji:**
```
Application/Tickets/Commands/ReopenTicket/ReopenTicketCommand.cs
Application/Tickets/Commands/ReopenTicket/ReopenTicketCommandValidator.cs
Domain/Entities/Ticket.cs (walidacja czasowa)
```

---

#### US-051: Dezaktywacja operatora
**Status:** ✅ Zaimplementowane (DeleteOrganizationUserCommand)

---

#### US-052: Bezpieczna obsługa sesji
**Status:** ⚠️ Częściowo  
**Brakuje:**
- Konfigurowalny timeout sesji (30 min serwis, 60 min klient)
- Refresh token mechanism

---

### Priorytet 2: Ważne (Ulepszenia) 🟡

#### US-022: Eksport historii ticketu
**Status:** ❌ Brak  
**Opis:** PDF, CSV, JSON export

**Pliki do utworzenia:**
```
Application/Tickets/Queries/ExportTicketHistory/ExportTicketHistoryQuery.cs
Application/Tickets/Queries/ExportTicketHistory/ExportTicketHistoryQueryHandler.cs
Infrastructure/Services/PdfExportService.cs
Infrastructure/Services/CsvExportService.cs
```

**Endpoint:**
```
GET /api/tickets/{id}/export?format=pdf|csv|json
```

---

#### US-039: Kafelek administracji dla administratora
**Status:** ❌ Brak (frontend feature, ale wymaga endpoint)

---

#### US-044: Oś czasu z podziałem na strony
**Status:** ⚠️ Częściowo (timeline jest, brak podziału lewa/prawa)  
**To jest głównie frontend feature**

---

#### US-045: Komentarze - edycja z limitem czasowym
**Status:** ⚠️ Częściowo  
**Brakuje:**
- Endpoint do edycji komentarza
- Walidacja: można edytować tylko do 15 minut po dodaniu

**Pliki do utworzenia:**
```
Application/Comments/Commands/UpdateComment/UpdateCommentCommand.cs
Application/Comments/Commands/UpdateComment/UpdateCommentCommandHandler.cs
Application/Comments/Commands/UpdateComment/UpdateCommentCommandValidator.cs
```

**Endpoint:**
```
PATCH /api/tickets/{ticketId}/comments/{commentId}
```

---

#### US-046: Dodawanie załączników (limit 3 na raz)
**Status:** ⚠️ Częściowo  
**Brakuje:**
- Walidacja: max 3 pliki jednocześnie
- Walidacja typów plików

---

#### US-048: Aktualizacja opisu zgłoszenia
**Status:** ✅ Zaimplementowane (UpdateTicketCommand)

---

### Priorytet 3: Opcjonalne (Nice to have) 🟢

| US | Tytuł | Status | Uwagi |
|----|-------|--------|-------|
| US-003 | Wybór języka | ❌ | Frontend + i18n |
| US-009 | Nawigacja i menu | ❌ | Frontend |

---

## 📋 Plan Implementacji - Faza 2

### Etap 2.1: Email Service & Account Activation (8-12h)

**Cel:** Pełny flow aktywacji konta i resetowania hasła

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| Produkcyjna implementacja EmailService (SendGrid/SMTP) | 4h | 🔴 |
| Szablony email (HTML) | 2h | 🔴 |
| ActivateAccountCommand + Validator | 2h | 🔴 |
| ValidateActivationTokenQuery | 1h | 🔴 |
| ResetPasswordCommand (pełny flow) | 2h | 🔴 |
| Testy jednostkowe | 2h | 🔴 |

**Pliki:**
```
Infrastructure/Services/EmailService.cs (przepisanie)
Infrastructure/Services/IEmailService.cs
Infrastructure/Templates/
  - ActivationEmail.html
  - PasswordResetEmail.html
  - ServiceUserInviteEmail.html
  - OrganizationInviteEmail.html
Application/Auth/Commands/ActivateAccount/
Application/Auth/Commands/ResetPassword/
Application/Auth/Queries/ValidateActivationToken/
```

---

### Etap 2.2: Dashboard Enhancements (6-8h)

**Cel:** Kompletne dane dla dashboardów zgodnie z PRD

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| GetTicketTrendsQuery (wykres 30 dni) | 3h | 🔴 |
| GetRecentActivitiesQuery | 2h | 🔴 |
| GetUpcomingMaintenancesQuery | 2h | 🟡 |
| GetOrganizationActivitiesQuery | 2h | 🔴 |
| Rozszerzenie DashboardController | 1h | 🔴 |

**Endpoints:**
```
GET /api/dashboard/trends
GET /api/dashboard/recent-activities
GET /api/dashboard/upcoming-maintenances
GET /api/dashboard/organization/activities
```

---

### Etap 2.3: Bulk Actions (4-6h)

**Cel:** Masowe operacje na ticketach

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| BulkAssignTicketsCommand | 2h | 🟡 |
| BulkChangeStatusCommand | 2h | 🟡 |
| BulkArchiveTicketsCommand | 1h | 🟡 |
| Walidacja i testy | 2h | 🟡 |

**Endpoints:**
```
POST /api/tickets/bulk/assign
POST /api/tickets/bulk/status  
POST /api/tickets/bulk/archive
```

---

### Etap 2.4: Maintenance Scheduling (6-8h)

**Cel:** Planowanie przeglądów technicznych maszyn (US-030)

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| MaintenanceSchedule Entity | 1h | 🟡 |
| Repository + EF Configuration | 2h | 🟡 |
| ScheduleMaintenanceCommand | 2h | 🟡 |
| GetUpcomingMaintenancesQuery | 1h | 🟡 |
| GetMachineMaintenanceHistoryQuery | 1h | 🟡 |
| Migracja EF | 0.5h | 🟡 |

---

### Etap 2.5: Comments & Notes Enhancement (4h)

**Cel:** Edycja komentarzy z limitem czasowym

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| UpdateCommentCommand z walidacją 15 min | 2h | 🟡 |
| DeleteCommentCommand z walidacją 15 min | 1h | 🟡 |
| UpdateNoteCommand z walidacją 5 min | 1h | 🟡 |

---

### Etap 2.6: Ticket Workflow Enhancements (4h)

**Cel:** Pełna zgodność z workflow PRD

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| AbandonTicketCommand (US-043) | 1h | 🔴 |
| Walidacja 14 dni dla Reopen (US-047) | 1h | 🔴 |
| Obowiązkowe uzasadnienie dla Resolved/Closed | 1h | 🔴 |
| Testy workflow | 1h | 🔴 |

---

### Etap 2.7: Export Feature (4-6h)

**Cel:** Eksport historii ticketu (US-022)

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| ExportTicketHistoryQuery | 1h | 🟢 |
| PdfExportService | 3h | 🟢 |
| CsvExportService | 1h | 🟢 |
| JsonExportService | 0.5h | 🟢 |

---

### Etap 2.8: Testing & Fixes (8-12h)

**Cel:** Naprawienie istniejących testów i dodanie nowych

| Zadanie | Czas | Priorytet |
|---------|------|-----------|
| Naprawa 9 testów TicketTests | 2h | 🔴 |
| Naprawa 5 testów integracyjnych | 2h | 🔴 |
| Testy dla nowych komponentów | 4h | 🔴 |
| Testy integracyjne API | 4h | 🟡 |

---

## 📊 Podsumowanie Szacunkowego Nakładu Pracy

| Etap | Czas | Priorytet |
|------|------|-----------|
| 2.1 Email & Activation | 8-12h | 🔴 |
| 2.2 Dashboard Enhancements | 6-8h | 🔴 |
| 2.3 Bulk Actions | 4-6h | 🟡 |
| 2.4 Maintenance Scheduling | 6-8h | 🟡 |
| 2.5 Comments Enhancement | 4h | 🟡 |
| 2.6 Ticket Workflow | 4h | 🔴 |
| 2.7 Export Feature | 4-6h | 🟢 |
| 2.8 Testing & Fixes | 8-12h | 🔴 |

**Łączny szacowany czas:** 44-68 godzin roboczych

---

## ✅ Kryteria Akceptacji Fazy 2

Faza 2 zostanie uznana za ukończoną, gdy:

1. **Pełny flow aktywacji konta działa** (US-002, US-005)
2. **Dashboard zawiera wszystkie KPI** (US-006, US-007, US-008)
3. **Masowe akcje na ticketach działają** (US-014)
4. **Workflow ticketów jest zgodny z PRD** (US-021, US-043, US-047)
5. **Testy przechodzą** (min 90% success rate)
6. **Wszystkie endpointy z api-plan.md są zaimplementowane**

---

## 🎯 Rekomendowana Kolejność Implementacji

### Sprint 1 (Krytyczne)
1. ✅ Etap 2.6: Ticket Workflow Enhancements
2. ✅ Etap 2.8 (część): Naprawa testów
3. ✅ Etap 2.1: Email Service & Account Activation

### Sprint 2 (Ważne)
1. ✅ Etap 2.2: Dashboard Enhancements
2. ✅ Etap 2.3: Bulk Actions
3. ✅ Etap 2.5: Comments Enhancement

### Sprint 3 (Opcjonalne)
1. ✅ Etap 2.4: Maintenance Scheduling
2. ✅ Etap 2.7: Export Feature
3. ✅ Etap 2.8 (reszta): Testy integracyjne

---

## 📝 Uwagi Końcowe

### Zależności zewnętrzne
- **SendGrid/SMTP** - konfiguracja dla EmailService
- **PDF Library** - wybór biblioteki (QuestPDF, iTextSharp, etc.)

### Potencjalne ryzyki
1. **Integracja email** - wymaga konfiguracji zewnętrznej usługi
2. **PDF generation** - może wymagać dodatkowych dependencji
3. **Testy integracyjne** - wymagają testowej bazy danych

### Frontend Dependencies
Niektóre US wymagają również pracy frontendowej:
- US-003 (Wybór języka) - wymaga i18n w React
- US-007 (Wykresy) - wymaga Chart.js lub podobnej biblioteki
- US-044 (Timeline z podziałem) - wymaga custom UI

---

**Ostatnia aktualizacja:** 2025-12-06  
**Autor:** Analiza na podstawie PRD, api-plan.md i stanu implementacji
