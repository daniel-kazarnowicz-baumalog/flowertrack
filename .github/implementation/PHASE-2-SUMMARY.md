# Faza 2: Application Layer - Plan Implementacji

## 📋 Podsumowanie

Przygotowany został **kompleksowy, szczegółowy plan Fazy 2** projektu FLOWerTRACK, obejmujący implementację warstwy aplikacji (Application Layer) zgodnie z najnowszymi podejściami architektonicznymi.

**Status:** ✅ Plan gotowy do realizacji  
**Data utworzenia:** 26 października 2025  
**Czas realizacji:** Sprint 2-3 (Tydzień 3-5)  
**Liczba zadań:** 120  
**GitHub Issues:** 10 szczegółowych issues

---

## 📚 Utworzone Dokumenty

### 1. PHASE-2-APPLICATION.md
**Główny dokument planistyczny** - 120 zadań

**Zawartość:**
- Szczegółowy opis każdej sekcji Fazy 2
- Checklisty z konkretnymi plikami do utworzenia
- Przykłady kodu dla każdego wzorca
- Struktura folderów
- Acceptance criteria
- Decyzje architektoniczne
- Best practices

**Sekcje:**
- 2.0 Repository Interfaces (dokończenie Fazy 1)
- 2.1 Application Infrastructure (MediatR, FluentValidation, AutoMapper)
- 2.2 Tickets Feature (Queries + Commands) - ⚡ Najbardziej krytyczne
- 2.3 Organizations Feature
- 2.4 Machines Feature
- 2.5 Users Feature
- 2.6 Domain Event Handlers
- 2.7 Application Services
- 2.8 Dependency Injection Configuration
- 2.9 Unit Tests

### 2. PHASE-2-GITHUB-ISSUES.md
**10 szczegółowych GitHub Issues**

Każdy issue zawiera:
- ✅ Jasny tytuł i opis kontekstu
- ✅ Technical Requirements (konkretne wymagania techniczne)
- ✅ Acceptance Criteria (kryteria akceptacji)
- ✅ Files to Create (dokładna lista plików)
- ✅ Example Code (przykłady implementacji)
- ✅ Dependencies (zależności od innych issues)
- ✅ Testing Requirements (wymagania testowe)
- ✅ Labels i estymacje czasowe

**Lista Issues:**

**🔥 Critical Priority (11-13 dni):**
1. **Issue #10** - Repository Interfaces (2 dni) - Dokończenie Fazy 1
2. **Issue #11** - Application Infrastructure Setup (3 dni) - MediatR, FluentValidation, AutoMapper
3. **Issue #12** - Ticket Queries (3 dni) - GetTickets, GetTicketById, GetTicketHistory
4. **Issue #13** - Ticket Commands (4 dni) - Create, Update, Assign, ChangeStatus, etc.

**🟡 High Priority (10 dni):**
5. **Issue #14** - Ticket Event Handlers (2 dni)
6. **Issue #15** - Organizations Feature (3 dni)
7. **Issue #16** - Machines Feature (3 dni)
8. **Issue #17** - Users Feature (2 dni)

**🟢 Medium Priority (6-7 dni):**
9. **Issue #18** - Application Services (2 dni)
10. **Issue #19** - Unit Tests (4-5 dni, równolegle)

### 3. PHASE-2-QUICK-REFERENCE.md
**Szybki przewodnik implementacyjny**

Zawiera:
- Przegląd architektury Vertical Slice
- Przykłady wzorców CQRS
- Pipeline Behaviors
- Strategie testowania
- Common Interfaces
- Dependency Injection
- Best Practices (DO's and DON'Ts)
- Konwencje nazewnictwa
- Checklist przed rozpoczęciem
- Success Criteria
- Przydatne komendy

---

## 🏗️ Architektura

### Przyjęty Wzorzec: **Vertical Slice Architecture**

```
Application/Features/
├── Tickets/
│   ├── Commands/
│   │   ├── CreateTicket/
│   │   │   ├── CreateTicketCommand.cs
│   │   │   ├── CreateTicketCommandHandler.cs
│   │   │   ├── CreateTicketCommandValidator.cs
│   │   │   └── TicketCreatedDto.cs
│   │   └── AssignTicket/...
│   ├── Queries/
│   │   ├── GetTickets/
│   │   │   ├── GetTicketsQuery.cs
│   │   │   ├── GetTicketsQueryHandler.cs
│   │   │   └── TicketDto.cs
│   │   └── GetTicketById/...
│   └── EventHandlers/
│       └── TicketCreatedEventHandler.cs
├── Organizations/...
├── Machines/...
└── Users/...
```

**Zalety tego podejścia:**
- ✅ Każda funkcja jest samodzielna
- ✅ Minimalna zależność między funkcjami
- ✅ Łatwe do rozwijania przez zespół
- ✅ Jasna struktura i odpowiedzialności

### Kluczowe Technologie

| Technologia | Wersja | Cel |
|-------------|--------|-----|
| MediatR | 12.4.0+ | Implementacja CQRS |
| FluentValidation | 11.9.2+ | Walidacja requestów |
| AutoMapper | 13.0.1+ | Mapowanie obiektów |
| Ardalis.Result | 9.0.0+ | Result pattern (zamiast wyjątków) |
| Ardalis.Specification | 8.0.0+ | Specyfikacje zapytań |

---

## 🎯 Zgodność z Najnowszymi Podejściami

### Z Context7 (jasontaylordev/cleanarchitecture):

✅ **Feature-based organization** - Struktura według funkcji biznesowych, nie warstw technicznych  
✅ **CQRS with MediatR** - Separacja Command/Query  
✅ **FluentValidation pipeline** - Walidacja w pipeline  
✅ **Vertical Slices** - Każdy use case jest samodzielny  
✅ **Result Pattern** - Lepsze zarządzanie błędami niż wyjątki  
✅ **Specification Pattern** - Hermetyzacja logiki zapytań  
✅ **Pipeline Behaviors** - Cross-cutting concerns  
✅ **Domain Events** - Event handlers w Application layer

### Zgodność z PRD i api-plan.md:

✅ Wszystkie User Stories z PRD pokryte  
✅ Wszystkie endpointy z api-plan.md zamapowane na Commands/Queries  
✅ Business rules zgodne z workflow z PRD  
✅ Filtrowanie, sortowanie, paginacja zgodnie z api-plan.md  
✅ Authorization zgodnie z rolami z PRD  
✅ Ticket workflow (Draft → Closed) zgodnie z PRD

---

## 📊 Statystyki Planu

### Według Priorytetów:
- 🔥 **Critical:** 4 issues, 13 dni, 60 zadań
- 🟡 **High:** 4 issues, 10 dni, 40 zadań
- 🟢 **Medium:** 2 issues, 7 dni, 20 zadań

### Według Funkcji:
- **Tickets:** 40 zadań (najbardziej krytyczne MVP)
- **Organizations:** 15 zadań
- **Machines:** 15 zadań
- **Users:** 10 zadań
- **Infrastructure:** 20 zadań
- **Event Handlers:** 10 zadań
- **Tests:** 10 zadań

### Coverage:
- **Queries:** 12 queries (list + detail dla każdej encji)
- **Commands:** 25 commands (CRUD + business operations)
- **Event Handlers:** 15 handlers
- **Validators:** 37 validators (każdy command/query ma validator)
- **DTOs:** 30+ DTOs

---

## 🚀 Droga Krytyczna

### Tydzień 3 (Foundation):
1. **Dzień 1-2:** Issue #10 - Repository Interfaces
2. **Dzień 3-5:** Issue #11 - Application Infrastructure

### Tydzień 4 (Core Features):
3. **Dzień 1-3:** Issue #12 - Ticket Queries
4. **Dzień 4-7:** Issue #13 - Ticket Commands
5. **Dzień 8-9:** Issue #14 - Event Handlers

### Tydzień 5 (Additional Features):
6. **Dzień 1-3:** Issue #15 - Organizations (równolegle z #16)
7. **Dzień 1-3:** Issue #16 - Machines (równolegle z #15)
8. **Dzień 4-5:** Issue #17 - Users
9. **Dzień 6-7:** Issue #18 - Application Services

### Równolegle przez cały czas:
10. **Issue #19** - Unit Tests (4-5 dni, równolegle z rozwojem)

---

## 💡 Kluczowe Wzorce Implementacyjne

### Command Pattern
```csharp
public record CreateTicketCommand(
    Guid MachineId,
    string Title,
    string Description,
    Priority Priority
) : IRequest<Result<Guid>>;

public class CreateTicketCommandHandler 
    : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Walidacja biznesowa
        // 2. Delegacja do domeny
        // 3. Zapis zmian
        // 4. Return Result
    }
}
```

### Query Pattern
```csharp
public record GetTicketsQuery(
    int Page = 1,
    int PageSize = 20,
    TicketStatus? Status = null
) : IRequest<Result<PaginatedList<TicketDto>>>;

public class GetTicketsQueryHandler 
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketDto>>>
{
    public async Task<Result<PaginatedList<TicketDto>>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Specyfikacja
        // 2. Query repository
        // 3. Mapowanie do DTO
        // 4. Return Result
    }
}
```

### Pipeline Behaviors
```
Request → UnhandledException → Validation → Performance → Logging → Handler → Response
```

---

## ✅ Kryteria Sukcesu Fazy 2

- [ ] Wszystkie Repository Interfaces zdefiniowane
- [ ] Wszystkie Commands/Queries dla core features (Tickets, Organizations, Machines)
- [ ] FluentValidation rules dla wszystkich commands/queries
- [ ] AutoMapper profiles dla wszystkich DTOs
- [ ] Pipeline behaviors działają poprawnie
- [ ] Wszystkie domain events mają handlery
- [ ] Unit test coverage > 80%
- [ ] Wszystkie commands/queries zgodne ze wzorcem
- [ ] Proper error handling z Result pattern
- [ ] Zero błędów kompilacji
- [ ] DependencyInjection.cs skonfigurowane
- [ ] XML documentation dla wszystkich public types

---

## 📖 Dokumentacja dla Zespołu

### Struktura Dokumentów:
```
.github/implementation/
├── PHASE-2-APPLICATION.md          ← Szczegółowy plan (120 zadań)
├── PHASE-2-GITHUB-ISSUES.md        ← 10 issues gotowych do utworzenia
├── PHASE-2-QUICK-REFERENCE.md      ← Szybki przewodnik implementacji
├── IMPLEMENTATION-TRACKER.md        ← Główny tracker (zaktualizowany)
├── CURRENT-PROGRESS.md             ← Postęp bieżący
└── PHASE-1-DOMAIN.md               ← Faza 1 (do dokończenia)
```

### Jak Korzystać:

1. **Przed rozpoczęciem:** Przeczytaj `PHASE-2-QUICK-REFERENCE.md`
2. **Podczas implementacji:** Używaj `PHASE-2-APPLICATION.md` jako checklisty
3. **Tworzenie GitHub Issues:** Kopiuj z `PHASE-2-GITHUB-ISSUES.md`
4. **Śledzenie postępu:** Aktualizuj `CURRENT-PROGRESS.md`
5. **Przykłady kodu:** Wszystkie dokumenty zawierają konkretne przykłady

---

## 🔄 Następne Kroki

### 1. Dokończenie Fazy 1 (jeśli potrzebne)
- [ ] Repository Interfaces (Issue #9 → teraz #10)
- [ ] Testy jednostkowe dla pozostałych encji
- [ ] Address Value Object (opcjonalnie)

### 2. Rozpoczęcie Fazy 2
**Najpierw (Critical Path):**
- [ ] Utworzyć Issue #10 na GitHubie (Repository Interfaces)
- [ ] Utworzyć Issue #11 na GitHubie (Application Infrastructure)
- [ ] Zaimplementować Issue #10 (2 dni)
- [ ] Zaimplementować Issue #11 (3 dni)

**Potem (Core Features):**
- [ ] Issue #12: Ticket Queries (3 dni)
- [ ] Issue #13: Ticket Commands (4 dni)
- [ ] Issue #14: Event Handlers (2 dni)

**Równolegle:**
- [ ] Issue #19: Pisanie testów jednostkowych

### 3. Przygotowanie do Fazy 3
Po ukończeniu Fazy 2, przygotować plan dla:
- Infrastructure Layer (EF Core, Supabase)
- Repository implementations
- Authentication/Authorization
- Email service
- File storage

---

## 📞 Pytania i Wyjaśnienia

### Decyzje Architektoniczne

**Q: Dlaczego Vertical Slice zamiast klasycznej Clean Architecture z warstwami?**  
A: Vertical Slice daje lepszą organizację i mniejsze coupling między features. Każdy use case jest samodzielny, łatwiejszy do rozwijania i testowania.

**Q: Dlaczego Result pattern zamiast exceptions?**  
A: Result pattern daje lepszą kontrolę nad błędami biznesowymi. Exceptions używamy tylko dla wyjątkowych sytuacji (np. błędy infrastruktury).

**Q: Czy używamy Generic Repository czy Specialized Repositories?**  
A: Oba. Mamy `IRepository<T>` jako bazę, a potem specialized repositories dla domain-specific queries.

### Implementacja

**Q: W jakiej kolejności implementować features?**  
A: Najpierw Tickets (najbardziej krytyczne MVP), potem Organizations, Machines, Users.

**Q: Czy event handlers mogą modyfikować dane?**  
A: Tak, ale ostrożnie. Event handlers mogą tworzyć nowe encje (np. auto-create ticket z alarmu), ale nie powinny modyfikować źródłowej encji.

**Q: Jak obsługiwać transakcje?**  
A: Używamy `IUnitOfWork` z metodami `BeginTransaction`, `CommitTransaction`, `RollbackTransaction`.

---

## 📊 Szacunki Czasowe

### Optymistyczny scenariusz: 20 dni
- Jeśli team ma doświadczenie z CQRS/MediatR
- Równoległa praca nad features
- Minimalne blockers

### Realistyczny scenariusz: 25-30 dni (plan bazowy)
- Team uczy się wzorców w trakcie
- Sekwencyjna praca nad critical path
- Normalne blockers i code reviews

### Pesymistyczny scenariusz: 35-40 dni
- Team nowy w CQRS
- Dużo refactoringu
- Problemy z zależnościami

**Rekomendacja:** Zaplanuj 30 dni (6 tygodni) z buforem 10 dni.

---

## 🎉 Podsumowanie

✅ **Plan Fazy 2 jest kompleksowy i gotowy do realizacji**

### Co zostało dostarczone:
1. ✅ Szczegółowy plan 120 zadań w `PHASE-2-APPLICATION.md`
2. ✅ 10 GitHub Issues gotowych do utworzenia w `PHASE-2-GITHUB-ISSUES.md`
3. ✅ Szybki przewodnik w `PHASE-2-QUICK-REFERENCE.md`
4. ✅ Przykłady kodu dla każdego wzorca
5. ✅ Best practices i decyzje architektoniczne
6. ✅ Strategie testowania
7. ✅ Zgodność z PRD i api-plan.md
8. ✅ Najnowsze podejścia architektoniczne (Vertical Slice, CQRS, Result Pattern)

### Kluczowe cechy planu:
- 📋 Bardzo szczegółowy - każde zadanie ma konkretne pliki do utworzenia
- 🎯 Zgodny z najnowszymi praktykami (jasontaylordev/cleanarchitecture)
- 🏗️ Vertical Slice Architecture - nowoczesne podejście
- 📚 Bogata dokumentacja i przykłady
- ✅ Konkretne acceptance criteria
- 🧪 Strategie testowania
- 🔄 Jasna droga krytyczna

### Gotowość do implementacji:
- ⚡ **Critical Path jasno określona:** Repository → Infrastructure → Tickets
- 📊 **Priorytety ustalone:** 4 Critical, 4 High, 2 Medium
- 🎫 **GitHub Issues gotowe:** Wystarczy skopiować na GitHub
- 📖 **Dokumentacja dla zespołu:** Quick Reference Guide
- ✅ **Success Criteria zdefiniowane**

---

## 🚀 Możesz Zaczynać!

**Pierwszy krok:**
1. Przeczytaj `PHASE-2-QUICK-REFERENCE.md`
2. Dokończ Repository Interfaces (Issue #10)
3. Setup Application Infrastructure (Issue #11)
4. Zacznij od Ticket Queries i Commands

**Powodzenia w implementacji!** 🎉

---

**Dokument utworzony:** 26 października 2025  
**Autor:** GitHub Copilot w oparciu o analizę PRD, api-plan.md, obecnej struktury projektu oraz najnowszych podejść architektonicznych z Context7  
**Wersja:** 1.0  
**Status:** ✅ Gotowy do realizacji
