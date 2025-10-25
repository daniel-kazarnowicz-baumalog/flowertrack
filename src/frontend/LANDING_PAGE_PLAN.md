# Plan Landing Page'ów dla FLOWerTRACK

## 1. Przegląd

FLOWerTRACK to zaawansowany system zarządzania zgłoszeniami serwisowymi, przeznaczony dla firm zajmujących się serwisem urządzeń produkcyjnych oraz ich klientów. System składa się z dwóch oddzielnych portali:

1. **Portal Serwisu (Service Portal)** - dla zespołu serwisowego i administratorów serwisu
2. **Portal Klienta (Client Portal)** - dla operatorów i administratorów organizacji klientów

W związku z tym, potrzebujemy **DWÓCH oddzielnych landing page'ów**, każdy dostosowany do specyficznej grupy docelowej.

### Cele Landing Page'ów

- Jasne przedstawienie wartości produktu dla każdej grupy użytkowników
- Wyróżnienie kluczowych funkcjonalności dedykowanych dla danej grupy
- Zachęcenie do rozpoczęcia korzystania z systemu
- Zapewnienie łatwej nawigacji między landing page'ami

---

## 2. Wspólne Elementy Design System

### 2.1. Paleta Kolorów

- **Primary**: Gradient niebieski/fioletowy (zachowany z obecnego designu)
- **Accent**:
  - Zielony (#10B981) - success, pozytywne akcje
  - Czerwony (#EF4444) - krytyczne, alerty
  - Pomarańczowy (#F59E0B) - ostrzeżenia, wysoki priorytet
- **Neutralne**: Szarości dla tła i tekstu
- **Background**: Gradient z animowanymi elementami floating icons

### 2.2. Typografia

- **Headingi**: Bold, duże rozmiary dla maksymalnego impaktu
- **Body**: Czytelny sans-serif (system font stack lub Inter)
- **Akcenty**: Gradient text dla kluczowych słów

### 2.3. Komponenty Wspólne

Następujące komponenty będą współdzielone między oboma landing page'ami (z propsami dla różnych treści):

- `Navbar` - nawigacja z wariantem dla service/client
- `HeroSection` - sekcja hero z konfigurowalnymi treściami
- `FeatureCard` - pojedyncza karta funkcjonalności
- `BenefitItem` - element listy korzyści
- `CTASection` - sekcja call-to-action
- `Footer` - stopka

---

## 3. Landing Page #1: Service Portal

### URL: `/service` lub `/` (domyślny)

### 3.1. Grupa Docelowa

- Administratorzy Serwisu (Service Administrators)
- Serwisanci (Service Technicians)
- Menedżerowie zespołów serwisowych

### 3.2. Nawigacja (Navbar)

**Elementy:**
- Logo: FLOWerTRACK z ikoną ⚙️
- Linki:
  - Funkcje (scroll do #features)
  - Korzyści (scroll do #benefits)
  - Dla Klientów (link do `/client`)
  - Kontakt (scroll do #contact)
- Przyciski:
  - "Portal Klienta" (wersja secondary, link do `/client`)
  - "Zaloguj się" (wersja primary, link do logowania Service Portal)

### 3.3. Hero Section

**Struktura:**
- Badge: "🚀 Nowa generacja zarządzania serwisem"
- Nagłówek: "Centralne zarządzanie zgłoszeniami **serwisowymi**"
  - "serwisowymi" z gradient textem
- Podtytuł: "Kompleksowa platforma dla zespołów serwisowych. Skróć czas reakcji, zwiększ transparentność i zautomatyzuj workflow zgłoszeń."
- CTA Buttons:
  - Primary: "Rozpocznij bezpłatnie" + ikona strzałki →
  - Secondary: "Zobacz demo" + ikona play ▶

**Statystyki:**
```
100+                    -60%                    99.9%
Obsłużonych             Czasu reakcji           Dostępności
zgłoszeń                                        systemu
```

**Tło:**
- Animated floating icons: 🔧 ⚙️ 🔩 🛠️
- Grid pattern w tle

### 3.4. Features Section

**Header:**
- Badge: "Funkcjonalności"
- Tytuł: "Wszystko czego potrzebujesz w jednym miejscu"

**Features Grid (6 kart w układzie 3x2):**

1. **🎯 Dashboard KPI**
   - Opis: "Kafelki ze statusami zgłoszeń, wykresy trendów i rozkład po priorytetach. Wszystkie kluczowe metryki w jednym miejscu."
   - Lista:
     - Kafelki z liczbą zgłoszeń
     - Wykresy trendów (30/90 dni)
     - Filtrowanie po priorytetach

2. **📋 Zarządzanie Zgłoszeniami**
   - Opis: "Kompleksowy widok listy z zaawansowanym filtrowaniem, szczegóły ticketu z timeline i masowe akcje."
   - Lista:
     - Tabela z sortowaniem i paginacją
     - Timeline zmian statusu
     - Masowe przypisywanie i zmiana statusu

3. **👥 Przydzielanie Zadań**
   - Opis: "Szybkie przypisywanie zgłoszeń do serwisantów, śledzenie obciążenia zespołu i workflow statusów."
   - Lista:
     - Przypisanie do siebie lub innego
     - Workflow: Draft → Wysłany → W trakcie → Rozwiązany
     - Monitoring obciążenia zespołu

4. **🏢 Moduł Organizacji**
   - Opis: "Lista wszystkich klientów z danymi kontaktowymi, maszynami i możliwością onboardingu nowych organizacji."
   - Lista:
     - Lista organizacji z statusami
     - Zarządzanie maszynami klientów
     - Generowanie tokenów API

5. **⚙️ Administracja Zespołu**
   - Opis: "Zarządzanie serwisantami, nadawanie uprawnień i monitoring aktywności zespołu."
   - Lista:
     - Dodawanie/edycja serwisantów
     - Role i uprawnienia (Admin/Technician)
     - Resetowanie haseł

6. **📊 Integracja Logów**
   - Opis: "Automatyczne zbieranie logów z maszyn produkcyjnych, historia problemów i tworzenie zgłoszeń z logów."
   - Lista:
     - Automatyczne zbieranie danych
     - Przeglądanie raw data
     - Tworzenie ticketów z logów

### 3.5. Benefits Section

**Layout:** Two-column (text + visual)

**Tytuł:** "Dlaczego FLOWerTRACK dla Twojego Serwisu?"

**Lista korzyści:**

✅ **Pełna widoczność pracy zespołu**
Menedżerowie mają jasny wgląd w to, nad czym pracują serwisanci i jakie zadania czekają na przydzielenie.

✅ **Centralizacja informacji**
Wszystkie zgłoszenia z różnych źródeł (email, telefon, HMI, logi maszyn) w jednym miejscu. Koniec z chaosem.

✅ **Szybsza reakcja na problemy**
Automatyczne zbieranie logów z maszyn i natychmiastowe powiadamianie zespołu o nowych zgłoszeniach.

✅ **Pełny audyt zmian**
Każda akcja (zmiana statusu, komentarz, przypisanie) jest logowana z informacją kto, co i kiedy zmienił.

**Visual:**
Dashboard preview z kafelkami:
- 📊 Aktywne zgłoszenia: 24
- ⚠️ Krytyczne: 3
- ✅ Rozwiązane: 156
- [Chart preview]

### 3.6. CTA Section

**Headline:** "Gotowy na optymalizację serwisu?"
**Subheadline:** "Dołącz do firm, które już skracają czas reakcji z FLOWerTRACK"

**Buttons:**
- Primary Large: "Rozpocznij za darmo"
- Outline: "Skontaktuj się z nami"

### 3.7. Footer

**4 kolumny:**

**Kolumna 1: Logo + Opis**
- Logo FLOWerTRACK ⚙️
- Opis: "Zaawansowany system zarządzania zgłoszeniami serwisowymi dla firm zajmujących się serwisem urządzeń produkcyjnych."

**Kolumna 2: Produkt**
- Funkcje
- Korzyści
- Demo
- Portal Klienta

**Kolumna 3: Firma**
- O nas
- Blog
- Kontakt
- Kariera

**Kolumna 4: Wsparcie**
- Dokumentacja
- FAQ
- Pomoc techniczna

**Bottom:**
© 2025 FLOWerTRACK by Baumalog. Wszystkie prawa zastrzeżone.

---

## 4. Landing Page #2: Client Portal

### URL: `/client`

### 4.1. Grupa Docelowa

- Administratorzy Organizacji Klientów (Organization Administrators)
- Operatorzy Maszyn (Machine Operators)

### 4.2. Nawigacja (Navbar)

**Elementy:**
- Logo: FLOWerTRACK z ikoną ⚙️
- Linki:
  - Możliwości (scroll do #features)
  - Jak działa (scroll do #how-it-works)
  - Dla Serwisu (link do `/service` lub `/`)
  - Kontakt (scroll do #contact)
- Przyciski:
  - "Portal Serwisu" (wersja secondary, link do `/service`)
  - "Zaloguj się" (wersja primary, link do logowania Client Portal)

### 4.3. Hero Section

**Struktura:**
- Badge: "🎯 Pełna kontrola w Twoich rękach"
- Nagłówek: "Śledź swoje maszyny i zgłoszenia w **jednym miejscu**"
  - "jednym miejscu" z gradient textem
- Podtytuł: "Dedykowany portal dla operatorów i administratorów organizacji. Twórz zgłoszenia, śledź statusy maszyn i zarządzaj zespołem bez wychodzenia z aplikacji."
- CTA Buttons:
  - Primary: "Aktywuj konto" + ikona strzałki →
  - Secondary: "Dowiedz się więcej" + ikona 📖

**Statystyki:**
```
24/7                    2 min                   100%
Dostęp do               Utworzenie              Transparentność
statusów                zgłoszenia              procesu
```

**Tło:**
- Animated floating icons: 🏭 🖥️ 📱 ✉️
- Grid pattern w tle

### 4.4. Features Section

**Header:**
- Badge: "Możliwości"
- Tytuł: "Wszystko pod kontrolą"

**Features Grid (4 karty w układzie 2x2):**

1. **📊 Dashboard Organizacji**
   - Opis: "Przegląd statusów wszystkich Twoich maszyn, aktywnych zgłoszeń i ostatnich aktywności w jednym miejscu."
   - Lista:
     - Kafelki statusów maszyn (Active, Alarm, Maintenance)
     - Kafelki statusów zgłoszeń
     - Ostatnie aktywności

2. **✍️ Tworzenie Zgłoszeń**
   - Opis: "Prosty formularz do zgłaszania problemów. Dodaj opis, załącz zdjęcia i śledź postęp naprawy."
   - Lista:
     - Wybór maszyny z listy
     - Załączniki i zdjęcia
     - Status: Szkic → Wysłany

3. **👁️ Śledzenie Statusu** (Featured)
   - Opis: "Timeline z pełną historią zgłoszenia. Zobacz wszystkie zmiany statusu, komentarze serwisu i załączniki."
   - Lista:
     - Oś czasu zmian (działania organizacji + serwisu)
     - Komunikacja z serwisem
     - Możliwość wznowienia (14 dni)

4. **👥 Zarządzanie Zespołem**
   - Opis: "Dodawaj operatorów, wysyłaj zaproszenia i monitoruj aktywność zespołu. Tylko dla Administratorów."
   - Lista:
     - Wysyłanie zaproszeń emailem
     - Status aktywacji operatorów
     - Dezaktywacja dostępu

### 4.5. How It Works Section

**Tytuł:** "Jak to działa?"
**Podtytuł:** "Zacznij w 3 prostych krokach"

**3 kroki (layout: horizontal z numerami):**

**Krok 1: 📧 Aktywuj konto**
Otrzymasz zaproszenie emailem od administratora serwisu. Kliknij link, ustaw hasło i gotowe!

**Krok 2: 👥 Dodaj zespół**
Jeśli jesteś Administratorem, zaproś operatorów do współpracy. Wyślij im zaproszenia jednym kliknięciem.

**Krok 3: 🎯 Twórz zgłoszenia**
Zgłaszaj problemy, dodawaj załączniki i śledź postęp naprawy w czasie rzeczywistym.

### 4.6. Benefits Section

**Layout:** Two-column (visual + text)

**Tytuł:** "Dlaczego FLOWerTRACK dla Twojej Organizacji?"

**Lista korzyści:**

✅ **Pełna transparentność**
Zobacz status naprawy w czasie rzeczywistym. Klienci wiedzą co się dzieje, co zmniejsza liczbę zapytań do serwisu.

✅ **Łatwość obsługi**
Intuicyjny interfejs, który nie wymaga szkoleń. Każdy operator może zacząć korzystać od razu.

✅ **Scentralizowana komunikacja**
Cała korespondencja z serwisem w kontekście zgłoszenia. Brak chaotycznych wątków emailowych.

✅ **Autonomia zespołu**
Administratorzy mogą samodzielnie zarządzać dostępem swoich pracowników bez kontaktu z serwisem.

**Visual:**
Timeline preview z przykładowymi wydarzeniami:
- [Organizacja] Dodano załącznik: zdjecie_problemu.jpg
- [Serwis] Status zmieniony: W trakcie
- [Organizacja] Dodano komentarz
- [Serwis] Status zmieniony: Rozwiązany

### 4.7. CTA Section

**Headline:** "Gotowy na lepszą komunikację z serwisem?"
**Subheadline:** "Aktywuj swoje konto i zacznij korzystać już dziś"

**Buttons:**
- Primary Large: "Aktywuj konto"
- Outline: "Kontakt z serwisem"

### 4.8. Footer

Identyczny jak w Service Portal, z dodatkowym linkiem "Portal Serwisu" w kolumnie Produkt.

---

## 5. Struktura Techniczna

### 5.1. Stack Technologiczny

- **Framework**: React + TypeScript + Vite
- **Styling**: Tailwind CSS + modularny CSS dla custom animacji
- **UI Components**: DevExtreme (dla bardziej zaawansowanych komponentów w przyszłości)
- **Routing**: React Router v6
- **Animacje**: CSS animations + opcjonalnie Framer Motion

### 5.2. Struktura Folderów

```
src/
├── pages/
│   ├── ServiceLanding.tsx          # Landing dla Service Portal
│   ├── ClientLanding.tsx           # Landing dla Client Portal
│   └── NotFound.tsx                # 404 page
├── components/
│   └── landing/
│       ├── Navbar.tsx              # Wspólna nawigacja
│       ├── HeroSection.tsx         # Wspólna sekcja hero
│       ├── FeatureCard.tsx         # Karta funkcjonalności
│       ├── BenefitItem.tsx         # Element korzyści
│       ├── CTASection.tsx          # Sekcja CTA
│       ├── Footer.tsx              # Stopka
│       └── HowItWorksStep.tsx      # Krok w "Jak to działa" (tylko Client)
├── styles/
│   ├── landing.css                 # Style dla landing pages
│   └── animations.css              # Animacje floating icons
├── assets/
│   └── icons/                      # Custom ikony (jeśli potrzebne)
└── App.tsx                         # Router configuration
```

### 5.3. Routing Configuration

```typescript
// App.tsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ServiceLanding from './pages/ServiceLanding';
import ClientLanding from './pages/ClientLanding';
import NotFound from './pages/NotFound';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ServiceLanding />} />
        <Route path="/service" element={<ServiceLanding />} />
        <Route path="/client" element={<ClientLanding />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}
```

### 5.4. Responsive Breakpoints

- **Mobile**: < 640px (pojedyncza kolumna, hamburger menu)
- **Tablet**: 640px - 1024px (2 kolumny w features grid)
- **Desktop**: > 1024px (3 kolumny w Service, 2 kolumny w Client)

---

## 6. Plan Implementacji Krok po Kroku

### Faza 1: Przygotowanie Środowiska
- [ ] **1.1** Utworzyć nowy branch: `feature/landing-pages`
- [ ] **1.2** Zainstalować React Router: `npm install react-router-dom`
- [ ] **1.3** Utworzyć strukturę folderów (`pages/`, `components/landing/`)

### Faza 2: Wydzielenie Wspólnych Komponentów
- [ ] **2.1** Utworzyć `Navbar.tsx` z propsami: `variant: 'service' | 'client'`
- [ ] **2.2** Utworzyć `HeroSection.tsx` z konfigurowalnymi treściami (props)
- [ ] **2.3** Utworzyć `FeatureCard.tsx` (icon, title, description, features list)
- [ ] **2.4** Utworzyć `BenefitItem.tsx` (icon, title, description)
- [ ] **2.5** Utworzyć `CTASection.tsx` (headline, subheadline, buttons)
- [ ] **2.6** Utworzyć `Footer.tsx` (wspólny dla obu stron)
- [ ] **2.7** Utworzyć `HowItWorksStep.tsx` (dla Client Landing)

### Faza 3: Migracja Stylów
- [ ] **3.1** Przenieść style z `App.css` do `landing.css`
- [ ] **3.2** Wydzielić animacje do `animations.css`
- [ ] **3.3** Zoptymalizować dla użycia Tailwind CSS tam gdzie możliwe
- [ ] **3.4** Zapewnić responsywność (mobile-first approach)

### Faza 4: Service Landing Page
- [ ] **4.1** Utworzyć `ServiceLanding.tsx`
- [ ] **4.2** Zaimplementować Navbar z variant="service"
- [ ] **4.3** Zaimplementować Hero Section z treściami dla Service
- [ ] **4.4** Zaimplementować Features Section (6 kart)
- [ ] **4.5** Zaimplementować Benefits Section
- [ ] **4.6** Zaimplementować CTA Section
- [ ] **4.7** Dodać Footer
- [ ] **4.8** Przetestować responsywność

### Faza 5: Client Landing Page
- [ ] **5.1** Utworzyć `ClientLanding.tsx`
- [ ] **5.2** Zaimplementować Navbar z variant="client"
- [ ] **5.3** Zaimplementować Hero Section z treściami dla Client
- [ ] **5.4** Zaimplementować Features Section (4 karty)
- [ ] **5.5** Zaimplementować How It Works Section (3 kroki)
- [ ] **5.6** Zaimplementować Benefits Section
- [ ] **5.7** Zaimplementować CTA Section
- [ ] **5.8** Dodać Footer
- [ ] **5.9** Przetestować responsywność

### Faza 6: Routing i Nawigacja
- [ ] **6.1** Skonfigurować React Router w `App.tsx`
- [ ] **6.2** Dodać routes: `/` → Service, `/client` → Client
- [ ] **6.3** Zaimplementować nawigację między landing pages w Navbar
- [ ] **6.4** Dodać smooth scroll dla anchor links (#features, #benefits, etc.)
- [ ] **6.5** Utworzyć 404 page (`NotFound.tsx`)

### Faza 7: Finalizacja i Optymalizacja
- [ ] **7.1** Sprawdzić wszystkie linki i przyciski
- [ ] **7.2** Zoptymalizować animacje (performance)
- [ ] **7.3** Dodać meta tagi SEO dla obu stron
- [ ] **7.4** Przetestować na różnych rozdzielczościach (mobile, tablet, desktop)
- [ ] **7.5** Sprawdzić accessibility (semantyczny HTML, ARIA labels)
- [ ] **7.6** Code review i cleanup
- [ ] **7.7** Merge do develop branch

### Faza 8: Dokumentacja
- [ ] **8.1** Zaktualizować README z informacjami o routing
- [ ] **8.2** Dodać screenshots landing pages do dokumentacji
- [ ] **8.3** Utworzyć dokumentację komponentów (props, usage)

---

## 7. Metryki Sukcesu

### Design
- ✅ Spójny design system między oboma landing pages
- ✅ Responsywność na wszystkich urządzeniach (mobile, tablet, desktop)
- ✅ Smooth animacje bez spadków performance

### Funkcjonalność
- ✅ Sprawna nawigacja między Service i Client landing
- ✅ Wszystkie linki działają poprawnie
- ✅ Smooth scroll dla anchor links

### Treść
- ✅ Jasne przedstawienie wartości dla każdej grupy użytkowników
- ✅ Wszystkie kluczowe funkcjonalności opisane
- ✅ Przekonujące CTA

### Techniczne
- ✅ Zero błędów w konsoli
- ✅ Lighthouse score > 90 (Performance, Accessibility)
- ✅ Kod zgodny z ESLint + Prettier

---

## 8. Następne Kroki (Post-MVP)

Po zaimplementowaniu podstawowych landing pages, możliwe rozszerzenia:

1. **Animacje i Interakcje**
   - Parallax scrolling
   - Animated counters w statystykach
   - Hover effects na kartach

2. **Content Enhancement**
   - Sekcja "Klienci o nas" (testimonials)
   - Case studies
   - Video demo

3. **Funkcjonalności**
   - Formularz kontaktowy działający
   - Newsletter signup
   - Live chat widget

4. **Optymalizacja SEO**
   - Structured data (JSON-LD)
   - Open Graph tags
   - Sitemap.xml

5. **Analytics**
   - Google Analytics / Plausible
   - Heatmaps (Hotjar)
   - A/B testing

---

## 9. Action Items

### Immediate (Przed rozpoczęciem implementacji)
- [ ] Zatwierdzić ten plan z Product Ownerem
- [ ] Przygotować content (wszystkie teksty finalne)
- [ ] Zebrać/utworzyć assety graficzne (ikony, obrazy)

### Implementation (Zgodnie z Fazami 1-7)
- [ ] Utworzyć branch `feature/landing-pages`
- [ ] Wykonać implementację zgodnie z planem krok po kroku
- [ ] Code review
- [ ] Merge do develop

### Post-Implementation
- [ ] Deploy na environment staging
- [ ] User testing z reprezentantami obu grup (serwisanci + klienci)
- [ ] Zbieranie feedbacku
- [ ] Deploy na production

---

**Koniec dokumentu planu Landing Pages**
