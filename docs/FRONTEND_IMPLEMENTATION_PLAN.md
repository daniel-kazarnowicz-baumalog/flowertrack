# FLOWerTRACK - Kompleksowy Plan Realizacji Frontendu

> **Data utworzenia:** 6 grudnia 2025  
> **Wersja:** 1.0  
> **Status:** W trakcie realizacji MVP

---

## Spis treści

1. [Przegląd projektu](#1-przegląd-projektu)
2. [Kluczowe założenia projektowe](#2-kluczowe-założenia-projektowe)
3. [Landing Page (Gateway)](#3-landing-page-gateway)
4. [Portal Serwisu (Service Portal)](#4-portal-serwisu-service-portal)
5. [Portal Klienta (Client Portal)](#5-portal-klienta-client-portal)
6. [Wspólne komponenty i architektura](#6-wspólne-komponenty-i-architektura)
7. [Design System](#7-design-system)
8. [Harmonogram realizacji](#8-harmonogram-realizacji)

---

## 1. Przegląd projektu

### 1.1. Aktualny stan

Aplikacja frontendowa jest zbudowana w oparciu o:
- **React 19.2** z TypeScript 5.9
- **Vite** (Rolldown variant 7.1.14) jako bundler
- **React Router 7** do routingu
- **React Query** (@tanstack/react-query) do zarządzania stanem serwerowym
- **i18next** do internacjonalizacji (PL/EN)
- **Recharts** do wizualizacji danych

### 1.2. Istniejące moduły

| Moduł | Stan | Uwagi |
|-------|------|-------|
| Gateway (Landing Page) | ✅ Podstawowy | Wymaga rozbudowy wizualnej |
| Logowanie Service | ✅ Podstawowy | Działa z mockowym API |
| Logowanie Client | ✅ Podstawowy | Działa z mockowym API |
| Dashboard Service | ✅ W trakcie | KPI, wykresy - podstawowa wersja |
| Dashboard Client | ✅ W trakcie | Podstawowa wersja |
| Zgłoszenia Service | ✅ W trakcie | Lista + szczegóły |
| Zgłoszenia Client | ✅ W trakcie | Lista + szczegóły |
| Organizacje | ✅ Podstawowy | Lista + szczegóły |
| Maszyny | ✅ Podstawowy | Lista + szczegóły |
| Zespół Service | ✅ Podstawowy | Lista użytkowników serwisu |
| Zespół Client | ✅ Podstawowy | Zarządzanie operatorami |
| i18n | ✅ Zaimplementowane | PL/EN |
| Ciemny motyw | ❌ Brak | Do implementacji |
| Animacje | ⚠️ Częściowe | Wymaga rozbudowy |

---

## 2. Kluczowe założenia projektowe

### 2.1. Mobile-First Design

```
Breakpointy:
├── xs: < 480px    (telefon - portrait)
├── sm: 480-768px  (telefon - landscape / mały tablet)
├── md: 768-1024px (tablet)
├── lg: 1024-1280px (laptop)
└── xl: > 1280px   (desktop)
```

**Zasady projektowe:**
- Projektowanie zaczyna się od wersji mobilnej
- Progressive enhancement - więcej funkcji na większych ekranach
- Touch-friendly UI: min. 44x44px dla elementów klikalnych
- Swipe gestures na mobilnych urządzeniach
- Bottom navigation na telefonach (zamiast sidebar)
- Collapsible sidebar na tabletach/desktopach

### 2.2. Internacjonalizacja (i18n)

**Status:** ✅ Zaimplementowane

**Struktura tłumaczeń:**
```
src/i18n/
├── index.ts                 # Konfiguracja i18next
└── locales/
    ├── pl.json              # Tłumaczenia polskie
    └── en.json              # Tłumaczenia angielskie
```

**Wymagane uzupełnienia:**
- [ ] Dodać brakujące klucze tłumaczeń dla nowych funkcji
- [ ] Tłumaczenia komunikatów walidacji
- [ ] Tłumaczenia komunikatów błędów API
- [ ] Formatowanie dat i liczb zgodne z locale
- [ ] Pluralizacja (np. "1 zgłoszenie" vs "5 zgłoszeń")

### 2.3. Dark/Light Theme

**Status:** ❌ Do implementacji

**Architektura:**
```typescript
// src/contexts/ThemeContext.tsx
interface ThemeContextValue {
  theme: 'light' | 'dark' | 'system';
  setTheme: (theme: 'light' | 'dark' | 'system') => void;
  resolvedTheme: 'light' | 'dark';
}
```

**CSS Variables (propozycja):**
```css
:root {
  /* Light theme - domyślne */
  --color-bg-primary: #ffffff;
  --color-bg-secondary: #f8fafc;
  --color-bg-tertiary: #f1f5f9;
  --color-text-primary: #1a202c;
  --color-text-secondary: #64748b;
  --color-border: #e2e8f0;
  --color-accent: #3b82f6;
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-error: #ef4444;
  --color-shadow: rgba(0, 0, 0, 0.1);
}

[data-theme="dark"] {
  --color-bg-primary: #0f172a;
  --color-bg-secondary: #1e293b;
  --color-bg-tertiary: #334155;
  --color-text-primary: #f8fafc;
  --color-text-secondary: #94a3b8;
  --color-border: #334155;
  --color-accent: #60a5fa;
  --color-success: #34d399;
  --color-warning: #fbbf24;
  --color-error: #f87171;
  --color-shadow: rgba(0, 0, 0, 0.4);
}
```

### 2.4. Animacje i UX

**Biblioteka:** CSS Animations + React Transitions

**Typy animacji:**
1. **Micro-interactions** - hover, focus, active states
2. **Page transitions** - wejście/wyjście stron
3. **Loading states** - skeleton loaders, spinners
4. **Feedback animations** - success/error toasts
5. **Data updates** - płynne aktualizacje list i wykresów

**Plik z animacjami:** `src/styles/animations.css`

```css
/* Przykładowe animacje */
@keyframes fadeIn { ... }
@keyframes slideInUp { ... }
@keyframes slideInRight { ... }
@keyframes pulse { ... }
@keyframes shake { ... }
@keyframes spin { ... }
```

---

## 3. Landing Page (Gateway)

### 3.1. Aktualny stan

**Plik:** `src/pages/Gateway.tsx`  
**CSS:** `src/styles/gateway.css`

Obecna implementacja to prosty ekran wyboru między portalem serwisowym a klienckim. Jest funkcjonalna, ale wymaga uatrakcyjnienia wizualnego.

### 3.2. Wymagania

| Wymaganie | Status | Priorytet |
|-----------|--------|-----------|
| Responsywność mobile-first | ⚠️ Częściowe | Wysoki |
| Dark/Light theme | ❌ Brak | Średni |
| Animacje wejścia | ⚠️ Podstawowe | Średni |
| Przełącznik języka | ✅ Jest | - |
| SEO meta tags | ❌ Brak | Niski |

### 3.3. Plan zmian

**Faza 1: Ulepszenia wizualne (2-3 dni)**
- [ ] Gradient background z animowanymi elementami floating
- [ ] Animacje wejścia kart (staggered animation)
- [ ] Hover effects na kartach (scale, shadow, glow)
- [ ] Lepszy kontrast i czytelność

**Faza 2: Responsywność (1-2 dni)**
- [ ] Pełna optymalizacja dla telefonów (stack layout)
- [ ] Touch-friendly buttons (min 48px)
- [ ] Safe area dla urządzeń z notchem

**Faza 3: Dostępność i polish (1 dzień)**
- [ ] Dark mode support
- [ ] Focus states dla keyboard navigation
- [ ] ARIA labels
- [ ] Meta tags dla SEO

### 3.4. Struktura komponentu

```
Gateway.tsx
├── LanguageSwitcher (prawy górny róg)
├── GatewayHeader
│   ├── Logo + Nazwa
│   └── Tagline
├── GatewayCards
│   ├── ServiceCard (portal serwisu)
│   └── ClientCard (portal klienta)
└── (opcjonalnie) Footer z linkami
```

---

## 4. Portal Serwisu (Service Portal)

### 4.1. Routing

```
/service                    → ServiceLogin
/service/forgot-password    → ServiceForgotPasswordPage
/service/reset-password     → ServiceResetPasswordPage
/service/dashboard          → ServiceDashboard (protected)
/service/tickets            → ServiceTicketsPage (protected)
/service/tickets/:id        → ServiceTicketDetailPage (protected)
/service/organizations      → OrganizationsListPage (protected)
/service/organizations/:id  → OrganizationDetailPage (protected)
/service/machines           → MachinesListPage (protected)
/service/machines/:id       → MachineDetailPage (protected)
/service/users              → ServiceUsersListPage (protected, admin only)
```

### 4.2. Layout

**Desktop:** Sidebar + Content Area
```
┌─────────────────────────────────────────────────────┐
│ ┌──────┐  ┌─────────────────────────────────────┐   │
│ │Logo  │  │ Header (date, notifications, user)  │   │
│ │──────│  ├─────────────────────────────────────┤   │
│ │ Nav  │  │                                     │   │
│ │ ───  │  │          Content Area               │   │
│ │ ───  │  │                                     │   │
│ │ ───  │  │                                     │   │
│ │──────│  │                                     │   │
│ │ User │  │                                     │   │
│ └──────┘  └─────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

**Mobile:** Bottom Navigation + Header
```
┌──────────────────────┐
│ Header (logo, menu)  │
├──────────────────────┤
│                      │
│    Content Area      │
│                      │
│                      │
├──────────────────────┤
│ 📊 │ 🎫 │ 🏢 │ 🏭 │ 👤 │  ← Bottom Nav
└──────────────────────┘
```

### 4.3. Moduły szczegółowe

#### 4.3.1. Dashboard (US-006, US-007, US-008)

**Wymagania PRD:**
- Kafelki KPI z liczbą zgłoszeń (wg priorytetów i statusów)
- Wykres trendów zgłoszeń (liniowy, 30 dni)
- Rozkład po priorytetach (kołowy/słupkowy)
- Lista ostatnich zdarzeń
- Szybkie linki: "Moje tickety", "Organizacje z alarmami"
- Akcje szybkie - kliknięcie kafelka → filtrowana lista

**Obecny stan:** ✅ Podstawowa implementacja KPI + wykresy

**TODO:**
- [ ] Real-time updates (polling co 30s)
- [ ] Animacja alertu na kafelkach krytycznych (pulsowanie)
- [ ] Responsywność kafelków na mobile (scroll horizontal lub grid 2x2)
- [ ] Skeleton loading states
- [ ] Click-through na filtrowane listy
- [ ] Dark mode styling
- [ ] Lokalizacja etykiet wykresów

#### 4.3.2. Lista Zgłoszeń (US-010, US-011, US-014)

**Wymagania PRD:**
- Tabela/kafelki ze wszystkimi zgłoszeniami
- Filtrowanie: status, priorytet, organizacja, maszyna, przypisany, data
- Wyszukiwarka po tytule i numerze
- Sortowanie kolumn
- Masowe akcje: przypisanie, zmiana statusu
- Grupowanie wg statusu
- Paginacja lub infinite scroll

**Obecny stan:** ✅ Lista z podstawowym filtrowaniem

**TODO:**
- [ ] Mobile: widok kafelkowy zamiast tabeli
- [ ] Zaawansowane filtry (drawer/modal na mobile)
- [ ] Infinite scroll dla mobile
- [ ] Masowe zaznaczanie i akcje
- [ ] Quick actions na hover/swipe
- [ ] Pull-to-refresh na mobile
- [ ] Dark mode

#### 4.3.3. Szczegóły Zgłoszenia (US-015, US-016, US-017, US-019, US-020)

**Wymagania PRD:**
- Pełne dane zgłoszenia, maszyny, organizacji
- Oś czasu (timeline) zmian statusu, notek, załączników
- Workflow statusów: Draft → Wysłany → Przyjęty → W trakcie → Rozwiązany → Wznowiony → Zamknięty
- Zmiana statusu z opcjonalnym uzasadnieniem
- Dodawanie notek wewnętrznych (niewidocznych dla klienta)
- Przypisanie do serwisanta
- Chat/komentarze z klientem
- Załączniki i galeria
- Eksport historii

**Obecny stan:** ✅ Podstawowe szczegóły + timeline

**TODO:**
- [ ] Mobile: tabs zamiast sekcji obok siebie
- [ ] Lightbox dla załączników
- [ ] Real-time comments (polling)
- [ ] Drag & drop załączników
- [ ] Status change modal z walidacją workflow
- [ ] Animowana timeline
- [ ] Dark mode

#### 4.3.4. Organizacje (US-022, US-023, US-024, US-025)

**Wymagania PRD:**
- Lista klientów z danymi kontaktowymi
- Status umowy/serwisu
- Liczba maszyn i aktywnych ticketów
- Stan alarmów
- Onboarding nowych klientów (wysyłka zaproszenia)
- Edycja danych kontaktowych (tylko admin)

**Obecny stan:** ✅ Podstawowa lista + szczegóły

**TODO:**
- [ ] Mobile card view
- [ ] Status badges (active, alarm, maintenance)
- [ ] Quick stats w listach
- [ ] Onboarding wizard/modal
- [ ] Dark mode

#### 4.3.5. Maszyny (US-026, US-027, US-028)

**Wymagania PRD:**
- Przypisywanie/edycja maszyn do organizacji
- Przegląd statusów maszyn
- Widok live danych
- Historia alarmów i problemów
- Daty ostatnich przeglądów
- Konfigurowalne interwały przeglądów

**Obecny stan:** ✅ Podstawowa lista + szczegóły

**TODO:**
- [ ] Status indicators (LED-style)
- [ ] Timeline alarmów
- [ ] Service schedule calendar view
- [ ] Mobile optimizations
- [ ] Dark mode

#### 4.3.6. Administracja - Użytkownicy (US-030, US-031, US-032, US-033)

**Wymagania PRD:**
- Lista serwisantów z rolą, statusem, ostatnią aktywnością
- Dodawanie nowych loginów
- Reset/zmiana hasła
- Nadawanie/odbieranie uprawnień administratora
- Monitorowanie obciążenia zespołu

**Obecny stan:** ✅ Podstawowa lista

**TODO:**
- [ ] Activity indicators
- [ ] Role management UI
- [ ] Workload visualization
- [ ] Invite flow
- [ ] Mobile responsive
- [ ] Dark mode

---

## 5. Portal Klienta (Client Portal)

### 5.1. Routing

```
/client                     → ClientLogin
/client/activate            → ClientActivatePage
/client/dashboard           → ClientDashboard (protected)
/client/tickets             → ClientTicketsPage (protected)
/client/tickets/:id         → ClientTicketDetailPage (protected)
/client/team                → OrganizationTeamPage (protected, admin only)
```

### 5.2. Layout

**Podobny do Service Portal, ale:**
- Prostszy sidebar (mniej opcji)
- Inny kolor akcentu (dla rozróżnienia)
- Brak sekcji Organizacje i Maszyny w nawigacji

### 5.3. Moduły szczegółowe

#### 5.3.1. Dashboard Klienta (US-034, US-035, US-036)

**Wymagania PRD:**
- Kafelki statusów maszyn: Active, Alarm, Maintenance
- Kafelki statusów zgłoszeń: moje aktywne, wszystkie organizacji
- Ostatnie aktywności
- Szybki dostęp do administracji (dla Admin)

**Obecny stan:** ✅ Podstawowa implementacja

**TODO:**
- [ ] Machine status visualization
- [ ] Activity feed
- [ ] Quick action cards
- [ ] Mobile optimizations
- [ ] Dark mode

#### 5.3.2. Lista Zgłoszeń Klienta (US-037, US-038, US-039)

**Wymagania PRD:**
- Lista zgłoszeń organizacji
- Filtrowanie: status, maszyna, właściciel
- Tworzenie nowego zgłoszenia
- Porzucanie zgłoszeń w statusie "Szkic"/"Wysłany"

**Obecny stan:** ✅ Podstawowa lista

**TODO:**
- [ ] "Utwórz nowe zgłoszenie" wizard
- [ ] Mobile-first filters
- [ ] Swipe actions (delete draft)
- [ ] Dark mode

#### 5.3.3. Szczegóły Zgłoszenia Klienta (US-044, US-045, US-046, US-047)

**Wymagania PRD:**
- Timeline: działania organizacji (lewa), działania serwisu (prawa), statusy (środek)
- Dodawanie komentarzy i załączników
- Aktualizacja opisu (Operator tylko swoje, Admin wszystkie)
- "Wznów ticket" dla rozwiązanych < 14 dni
- Zakładka z maszyną

**Obecny stan:** ✅ Podstawowe szczegóły

**TODO:**
- [ ] Dual-sided timeline
- [ ] Reopen ticket flow
- [ ] Machine details tab
- [ ] Mobile tabs
- [ ] Dark mode

#### 5.3.4. Zarządzanie Zespołem (US-048, US-049, US-050, US-051)

**Wymagania PRD:**
- Lista użytkowników organizacji
- Dla każdego: imię, email, ostatnia aktywność, liczba ticketów
- Dodawanie operatorów (zaproszenie email)
- Status aktywacji
- Dezaktywacja operatorów

**Obecny stan:** ✅ Podstawowa lista

**TODO:**
- [ ] Invite modal/form
- [ ] Activity indicators
- [ ] Deactivate confirmation
- [ ] Mobile responsive
- [ ] Dark mode

---

## 6. Wspólne komponenty i architektura

### 6.1. Struktura katalogów

```
src/
├── assets/                    # Statyczne zasoby (obrazy, ikony)
├── components/
│   ├── auth/                  # Komponenty autoryzacji
│   ├── charts/                # Komponenty wykresów
│   ├── dashboard/             # Komponenty dashboardu
│   ├── layout/                # Layouty (Service, Client)
│   ├── machines/              # Komponenty maszyn
│   ├── organizations/         # Komponenty organizacji
│   ├── team/                  # Komponenty zespołu
│   ├── tickets/               # Komponenty zgłoszeń
│   ├── ui/                    # Wspólne komponenty UI
│   │   ├── Badge/
│   │   ├── Button/
│   │   ├── Card/
│   │   ├── Checkbox/
│   │   ├── FileUploader/
│   │   ├── Input/
│   │   ├── LanguageSwitcher/
│   │   ├── Loader/
│   │   ├── Modal/
│   │   ├── ThemeSwitcher/     # NEW
│   │   └── Toast/
│   └── users/                 # Komponenty użytkowników
├── contexts/
│   ├── AuthContext.tsx
│   ├── ThemeContext.tsx       # NEW
│   └── ToastContext.tsx
├── hooks/
│   ├── useAttachments.ts
│   ├── useComments.ts
│   ├── useDashboard.ts
│   ├── useMachines.ts
│   ├── useMediaQuery.ts       # NEW - responsywność
│   ├── useOrganizations.ts
│   ├── useServiceUsers.ts
│   ├── useTeam.ts
│   ├── useTheme.ts            # NEW
│   ├── useTickets.ts
│   └── useToast.ts
├── i18n/
│   ├── index.ts
│   └── locales/
│       ├── en.json
│       └── pl.json
├── lib/
│   ├── api.ts
│   ├── apiClient.ts
│   ├── env.ts
│   └── queryClient.tsx
├── pages/
│   ├── client/
│   ├── service/
│   ├── ClientLogin.tsx
│   ├── Gateway.tsx
│   ├── NotFound.tsx
│   └── ServiceLogin.tsx
├── services/
├── styles/
│   ├── animations.css
│   ├── gateway.css
│   ├── landing.css
│   ├── login.css
│   └── variables.css          # NEW - CSS variables dla theme
├── types/
│   └── api.ts
├── App.css
├── App.tsx
├── index.css
├── main.tsx
└── vite-env.d.ts
```

### 6.2. Nowe komponenty UI do stworzenia

| Komponent | Opis | Priorytet |
|-----------|------|-----------|
| `ThemeSwitcher` | Przełącznik Light/Dark/System | Wysoki |
| `BottomNav` | Nawigacja dolna dla mobile | Wysoki |
| `Skeleton` | Loading placeholder | Wysoki |
| `EmptyState` | Stan pustej listy | Średni |
| `ConfirmDialog` | Dialog potwierdzenia | Średni |
| `Dropdown` | Rozwijane menu | Średni |
| `Tabs` | Zakładki (mobile-friendly) | Średni |
| `Timeline` | Oś czasu (dwustronna) | Średni |
| `Avatar` | Avatar użytkownika | Niski |
| `Tooltip` | Podpowiedzi | Niski |

### 6.3. Custom Hooks do stworzenia

```typescript
// useMediaQuery - wykrywanie breakpointów
const isMobile = useMediaQuery('(max-width: 768px)');

// useTheme - zarządzanie motywem
const { theme, setTheme, resolvedTheme } = useTheme();

// useOnlineStatus - wykrywanie połączenia
const isOnline = useOnlineStatus();

// usePrefersReducedMotion - preferencje animacji
const prefersReducedMotion = usePrefersReducedMotion();

// useLocalStorage - persystencja w localStorage
const [value, setValue] = useLocalStorage('key', defaultValue);

// useDebounce - debouncing wartości
const debouncedSearch = useDebounce(searchTerm, 300);
```

---

## 7. Design System

### 7.1. Paleta kolorów

```css
/* Primary Brand Colors */
--color-primary-50: #eff6ff;
--color-primary-100: #dbeafe;
--color-primary-200: #bfdbfe;
--color-primary-300: #93c5fd;
--color-primary-400: #60a5fa;
--color-primary-500: #3b82f6;  /* Main */
--color-primary-600: #2563eb;
--color-primary-700: #1d4ed8;
--color-primary-800: #1e40af;
--color-primary-900: #1e3a8a;

/* Service Portal Accent */
--color-service: #3b82f6;      /* Blue */

/* Client Portal Accent */
--color-client: #8b5cf6;       /* Purple */

/* Semantic Colors */
--color-success: #10b981;
--color-warning: #f59e0b;
--color-error: #ef4444;
--color-info: #0ea5e9;

/* Priority Colors */
--color-priority-critical: #ef4444;
--color-priority-high: #f97316;
--color-priority-medium: #eab308;
--color-priority-low: #22c55e;

/* Status Colors */
--color-status-new: #64748b;
--color-status-open: #3b82f6;
--color-status-in-progress: #f59e0b;
--color-status-resolved: #10b981;
--color-status-closed: #6b7280;
```

### 7.2. Typografia

```css
/* Font Family */
--font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
--font-mono: 'JetBrains Mono', 'Fira Code', monospace;

/* Font Sizes */
--text-xs: 0.75rem;     /* 12px */
--text-sm: 0.875rem;    /* 14px */
--text-base: 1rem;      /* 16px */
--text-lg: 1.125rem;    /* 18px */
--text-xl: 1.25rem;     /* 20px */
--text-2xl: 1.5rem;     /* 24px */
--text-3xl: 1.875rem;   /* 30px */
--text-4xl: 2.25rem;    /* 36px */

/* Font Weights */
--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;

/* Line Heights */
--leading-tight: 1.25;
--leading-normal: 1.5;
--leading-relaxed: 1.75;
```

### 7.3. Spacing & Layout

```css
/* Spacing Scale */
--space-0: 0;
--space-1: 0.25rem;   /* 4px */
--space-2: 0.5rem;    /* 8px */
--space-3: 0.75rem;   /* 12px */
--space-4: 1rem;      /* 16px */
--space-5: 1.25rem;   /* 20px */
--space-6: 1.5rem;    /* 24px */
--space-8: 2rem;      /* 32px */
--space-10: 2.5rem;   /* 40px */
--space-12: 3rem;     /* 48px */
--space-16: 4rem;     /* 64px */

/* Border Radius */
--radius-sm: 0.25rem;
--radius-md: 0.375rem;
--radius-lg: 0.5rem;
--radius-xl: 0.75rem;
--radius-2xl: 1rem;
--radius-full: 9999px;

/* Shadows */
--shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
--shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1);
--shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1);
--shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1);
```

### 7.4. Animacje

```css
/* Transitions */
--transition-fast: 150ms ease;
--transition-base: 200ms ease;
--transition-slow: 300ms ease;

/* Animation Durations */
--duration-75: 75ms;
--duration-100: 100ms;
--duration-150: 150ms;
--duration-200: 200ms;
--duration-300: 300ms;
--duration-500: 500ms;

/* Easing */
--ease-in: cubic-bezier(0.4, 0, 1, 1);
--ease-out: cubic-bezier(0, 0, 0.2, 1);
--ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
--ease-bounce: cubic-bezier(0.68, -0.55, 0.265, 1.55);
```

---

## 8. Harmonogram realizacji

### Faza 1: Fundamenty (1-2 tygodnie)

**Tydzień 1:**
- [ ] Setup CSS Variables dla theming
- [ ] Implementacja ThemeContext i ThemeSwitcher
- [ ] Hook useMediaQuery
- [ ] Responsywne breakpointy w CSS
- [ ] Komponent Skeleton loader

**Tydzień 2:**
- [ ] BottomNav dla mobile
- [ ] Aktualizacja ServiceLayout dla mobile
- [ ] Aktualizacja ClientLayout dla mobile
- [ ] Testy responsywności na różnych urządzeniach

### Faza 2: Dark Mode (1 tydzień)

- [ ] CSS Variables dla dark theme
- [ ] Migracja wszystkich komponentów UI
- [ ] Migracja stron Service Portal
- [ ] Migracja stron Client Portal
- [ ] Testy wizualne

### Faza 3: Mobile Optimization (2-3 tygodnie)

**Service Portal:**
- [ ] Dashboard mobile layout
- [ ] Tickets list - card view
- [ ] Ticket detail - tabbed layout
- [ ] Organizations mobile
- [ ] Machines mobile
- [ ] Users mobile

**Client Portal:**
- [ ] Dashboard mobile layout
- [ ] Tickets list mobile
- [ ] Ticket detail mobile
- [ ] Team management mobile

### Faza 4: Animacje i Polish (1-2 tygodnie)

- [ ] Page transitions
- [ ] List item animations
- [ ] Micro-interactions (buttons, inputs)
- [ ] Loading states
- [ ] Toast animations
- [ ] Modal animations
- [ ] Chart animations

### Faza 5: Uzupełnienia i18n (1 tydzień)

- [ ] Audit wszystkich tekstów
- [ ] Brakujące tłumaczenia PL
- [ ] Brakujące tłumaczenia EN
- [ ] Formatowanie dat/liczb
- [ ] Pluralizacja

### Faza 6: Testy i QA (1-2 tygodnie)

- [ ] Unit tests dla hooks
- [ ] Component tests
- [ ] E2E tests (happy paths)
- [ ] Cross-browser testing
- [ ] Accessibility audit
- [ ] Performance audit

---

## Podsumowanie priorytetów

| Priorytet | Element | Estimated |
|-----------|---------|-----------|
| 🔴 Wysoki | Mobile-first layouts | 2-3 tyg |
| 🔴 Wysoki | Dark mode | 1 tyg |
| 🟡 Średni | Animacje | 1-2 tyg |
| 🟡 Średni | i18n uzupełnienia | 1 tyg |
| 🟢 Niski | Landing page ulepszenia | 3-4 dni |
| 🟢 Niski | SEO & meta tags | 1 dzień |

**Całkowity szacowany czas:** 8-12 tygodni dla pełnej implementacji MVP

---

## Załączniki

### A. Referencje PRD

Dokument ten opiera się na wymaganiach zdefiniowanych w:
- `.ai/PRD.md` - Dokument Wymagań Produktu
- `.ai/Struktura Funkcjonalna Aplikacji Organizacyjnej (Panel Klienta).md`
- `.ai/Struktura Funkcjonalna Serwisu.md`
- `.ai/Tech-stack.md`

### B. Istniejące zasoby

- `src/i18n/locales/pl.json` - Tłumaczenia polskie
- `src/i18n/locales/en.json` - Tłumaczenia angielskie
- `src/styles/animations.css` - Definicje animacji

### C. Kolejne kroki

1. Review planu przez zespół
2. Priorytyzacja konkretnych zadań
3. Utworzenie issues w systemie zarządzania projektem
4. Rozpoczęcie implementacji od Fazy 1

---

*Dokument opracowany na podstawie analizy istniejącego kodu i dokumentacji projektowej.*
