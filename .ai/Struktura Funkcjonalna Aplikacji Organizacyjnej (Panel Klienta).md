## Struktura Funkcjonalna Aplikacji Organizacyjnej (Panel Klienta)

Poniższy plik Markdown przedstawia strukturę funkcjonalną aplikacji przeznaczonej dla klientów (Operatorów i Administratorów Organizacji), bazując na diagramie przepływu oraz szczegółowych opisach funkcjonalności i wymagań użytkownika (Use Cases). Aplikacja jest zaprojektowana jako minimalistyczny panel webowy (mobile-first).

---

### 1. Moduł Logowania i Aktywacji

Moduł ten służy do uwierzytelniania użytkowników organizacji oraz aktywacji ich kont.

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Ekran Logowania** | Użytkownik wprowadza nazwę użytkownika i hasło, aby uzyskać dostęp do panelu. | **Jako Użytkownik** chcę móc zalogować się na swój profil, aby uzyskać dostęp do organizacyjnego panelu aplikacji. |
| **Aktywacja Konta** | Proces aktywacji konta przez link otrzymany w wiadomości e-mail, umożliwiający ustawienie hasła. | **Jako Użytkownik**, który otrzymał zaproszenie do organizacji, chcę aktywować konto przez link z e-maila, żeby ustawić swoje hasło. |

---

### 2. Ekran Główny (Dashboard)

Ekran prezentujący najważniejsze informacje i statusy kluczowych elementów organizacji.

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Kafelki Statusów Maszyn** | Liczba maszyn w podziale na statusy (Active, Alarm, Maintenance). | **Jako Operator/Administrator** chcę zobaczyć na głównym ekranie kafelki z liczbą maszyn w statusie (Active, Alarm, Maintenance), liczbą moich aktywnych ticketów i wszystkich zgłoszeń organizacji, żebym miał szybki wgląd w sytuację. |
| **Kafelki Statusów Zgłoszeń** | Liczba aktywnych ticketów użytkownika oraz wszystkich zgłoszeń organizacji. | **Jako Operator/Administrator** chcę zobaczyć na głównym ekranie kafelki z liczbą maszyn w statusie (Active, Alarm, Maintenance), liczbą moich aktywnych ticketów i wszystkich zgłoszeń organizacji, żebym miał szybki wgląd w sytuację. |
| **Kafelek Aktywności** | Kafelek z ostatnimi aktywnościami organizacji/ticketów. | **Jako Operator/Administrator** chcę mieć na dashboardzie kafelek z ostatnimi aktywnościami organizacji/ticketów, aby widzieć, co wydarzyło się ostatnio. |
| **Szybki Dostęp do Administracji** | Kafelek/przycisk prowadzący do zakładki „Zespół” (dostępny dla Administratora). | **Jako Administrator** chcę mieć kafelek/przycisk prowadzący do zakładki „Zespół”, żebym mógł szybko zarządzać operatorami. |

---

### 3. Moduł Zgłoszeń (Zgłoszenia)

Kluczowy moduł do zarządzania i obsługi zgłoszeń serwisowych.

#### 3.1. Widok Listy Zgłoszeń (Widok wszystkich zgłoszeń z filtrowaniem)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Lista Zgłoszeń** | Tabela z listą wszystkich zgłoszeń organizacji, zawierająca status, priorytet i maszynę. | **Jako Operator/Administrator** chcę widzieć listę wszystkich zgłoszeń organizacji z widocznym statusem, priorytetem i maszyną, abym mógł szybko przejrzeć problemy. |
| **Filtrowanie** | Możliwość filtrowania zgłoszeń według statusu, maszyny i właściciela (twórcy zgłoszenia). | **Jako Operator/Administrator** chcę filtrować zgłoszenia według statusu, maszyny, właściciela, żebym szybciej znalazł interesujące mnie zgłoszenie. |
| **Tworzenie Nowego Zgłoszenia** | Przycisk umożliwiający rozpoczęcie nowego zgłoszenia. | **Jako Operator/Administrator** chcę mieć przycisk „Utwórz nowe zgłoszenie”, abym mógł łatwo rozpocząć nowe zgłoszenie. |
| **Porzucenie Zgłoszenia** | Możliwość usunięcia (porzucenia) zgłoszenia w statusie „Szkic” lub „Wysłany”. | **Jako Operator/Administrator** chcę mieć możliwość „porzucenia” zgłoszenia w statusie „Szkic” lub „Wysłany”, żebym nie musiał wysyłać nieaktualnych zgłoszeń. |

#### 3.2. Widok Szczegółów Zgłoszenia (Widok pojedynczego zgłoszenia)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Oś Czasu (Timeline)** | Szczegóły zgłoszenia w formie osi czasu: działania organizacji (lewa strona), działania serwisu (prawa strona), zmiany statusów (środek). | **Jako Operator/Administrator** chcę widzieć szczegóły zgłoszenia w formie osi czasu, z przejrzystym podziałem na działania organizacji (lewa strona), działania serwisu (prawa strona), oraz zmiany statusów (środek), żebym mógł łatwo śledzić przebieg obsługi. |
| **Dodawanie Komentarzy/Załączników** | Możliwość dodawania komentarzy, załączników lub aktualizacji opisu. | **Jako Operator** chcę móc dodawać komentarz, załączniki lub aktualizować opis tylko w swoich ticketach, aby precyzyjnie komunikować się z serwisem. **Jako Administrator** chcę móc komentować, dodawać załączniki i aktualizować dowolny ticket organizacji, żeby wspierać operatorów i kontakt z serwisem. **Jako Operator/Administrator** chcę w dowolnym momencie dodać do zgłoszenia nowy załącznik, żeby wzbogacić zgłoszenie o dodatkową dokumentację. |
| **Wznowienie Ticketu** | Przycisk „Wznów ticket” widoczny na zgłoszeniach w statusie „Rozwiązany” przez mniej niż 14 dni. | **Jako Administrator** chcę widzieć przycisk „Wznów ticket” na zgłoszeniu, które jest Rozwiązane mniej niż 14 dni, aby ponownie uruchomić proces obsługi, jeśli problem nie został rozwiązany. |

---

### 4. Moduł Administracji Organizacji (Administracja / Zespół)

Moduł do zarządzania operatorami w ramach danej organizacji (dostępny dla Administratora Organizacji).

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Lista Zespołu** | Lista wszystkich użytkowników organizacji z imieniem, e-mailem, ostatnią aktywnością i liczbą utworzonych ticketów. | **Jako Administrator** chcę widzieć listę wszystkich użytkowników w mojej organizacji z imieniem, emailem, ostatnią aktywnością i liczbą utworzonych ticketów, by mieć kontrolę nad zespołem. |
| **Dodawanie Operatorów** | Dodawanie nowych operatorów przez e-mail, z widocznym statusem aktywacji (oczekuje na aktywację/aktywny). | **Jako Administrator** chcę mieć możliwość dodania nowego operatora przez e-mail oraz zobaczyć status aktywacji (oczekuje na aktywację/aktywny), żebym zwiększał skład organizacji. |
| **Dezaktywacja Operatorów** | Możliwość usunięcia (dezaktywacji) operatora, co skutkuje odebraniem dostępu do systemu. | **Jako Administrator** chcę móc usunąć (dezaktywować) operatora, by odebrać mu dostęp do systemu. |