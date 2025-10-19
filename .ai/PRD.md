# Dokument Wymagań Produktu (PRD) - FLOWerTRACK

## 1. Przegląd Produktu

FLOWerTRACK to zaawansowany system zarządzania zgłoszeniami serwisowymi, przeznaczony dla firm zajmujących się serwisem urządzeń produkcyjnych (takich jak Baumalog) oraz ich klientów. System stanowi kompleksową platformę komunikacyjną umożliwiającą centralizację procesów zgłaszania, przydzielania, śledzenia i rozwiązywania problemów technicznych.

Architektura produktu obejmuje dwa oddzielne portale, dostosowane do potrzeb dwóch głównych grup użytkowników:

1. Portal Serwisu (Service Portal) - dla zespołu serwisowego i administratorów serwisu
2. Portal Klienta (Client Portal) - dla operatorów i administratorów organizacji klientów

Maszyny produkcyjne klientów automatycznie wysyłają logi problemów do systemu, umożliwiając zespołowi serwisu i operatorom klientów przeglądanie tych informacji w jednym miejscu. Całość komunikacji jest scentralizowana, śledzalna i w pełni zaudytowana.

### Cele Produktu

- Skrócenie czasu obsługi zgłoszeń do minimum
- Zapewnienie pełnej przejrzystości procesu serwisowego dla wszystkich stron
- Automatyzacja flow zgłoszeń od utworzenia do zamknięcia
- Minimalizacja błędów i braków komunikacji
- Ułatwienie integracji logów z urządzeń produkcyjnych
- Umożliwienie szybkiego przydzielania zadań zespołowi serwisowemu

### Grupy Docelowe

- Administratorzy Serwisu (Service Administrators)
- Serwisanci (Service Technicians)
- Administratorzy Organizacji Klientów (Organization Administrators)
- Operatorzy Maszyn (Machine Operators)

---

## 2. Problem Użytkownika

### Problem dla Zespołu Serwisu

1. Brak centralizacji informacji o zgłoszeniach: Zgłoszenia pochodzą z różnych źródeł (email, telefon, HMI, logi maszyn), trudno je śledzić w jednym miejscu.

2. Niedostateczna widoczność statusu pracy: Menedżerowie nie mają jasnego wglądu w to, nad czym pracują serwisanci i jakie zadania czekają na przydzielenie.

3. Brak transparentności dla klientów: Klienci nie wiedzą w jakim stadium znajduje się ich naprawa, co prowadzi do wielu zapytań.

4. Utrudniony dostęp do kontekstu technicznego: Serwisanci muszą szukać informacji o maszynach, historiach problemów i raw data w różnych systemach.

5. Brak audytu zmian: Nie ma pełnego śledzenia kto, co i kiedy zmienił w zgłoszeniu.

### Problem dla Klientów

1. Brak dostępu do informacji o maszynie: Operatorzy nie mają szybkiego dostępu do statusu swoich maszyn i historii problemów.

2. Trudności w komunikacji z serwisem: Brak skonsolidowanego miejsca do komunikacji i śledzenia postępu naprawy.

3. Niedostateczna dokumentacja problemu: Trudno dodawać dodatkowe informacje, załączniki lub aktualizacje do zgłoszeń.

4. Brak możliwości samodzielnego zarządzania operatorami: Administrator organizacji nie ma narzędzi do zarządzania dostępem swoich pracowników.

5. Słaby wgląd w logi maszyn: Operatorzy nie potrafią szybko znaleźć i zrozumieć logów swoich maszyn.

---

## 3. Wymagania Funkcjonalne

### 3.1. Portal Serwisu

#### Moduł Logowania i Uwierzytelniania
- Ekran logowania z polem login/hasło
- Weryfikacja poświadczeń użytkownika
- Automatyczne przekierowanie po zalogowaniu do dashboardu
- Przycisk "Zapomniałem hasła"
- Możliwość wyboru języka przed zalogowaniem
- Sesje użytkownika z timeoutem bezpieczeństwa

#### Moduł Główny (Dashboard)
- Wyświetlanie kafelków KPI z liczbą aktywnych zgłoszeń (podzielone na priorytety i statusy)
- Wykres trendów zgłoszeń i rozkład po priorytetach
- Lista ostatnich zdarzeń i powiadomień
- Szybkie linki do "Moje tickety", "Organizacje z alarmami", "Nadchodzące przeglądy"
- Menu nawigacyjne z zakładkami: Dashboard, Zgłoszenia, Organizacje, Administracja (tylko dla adminów)
- Opcja zmiany języka
- Przycisk wylogowania
- Akcje szybkie - klikniecie na kafelek prowadzi do przefiltrowanej listy

#### Moduł Zgłoszeń - Widok Listy
- Tabela/kafelki ze wszystkimi zgłoszeniami organizacji
- Filtrowanie po: statusie, priorytecie, organizacji, maszynie, przypisanym serwisancie, dacie utworzenia
- Wyszukiwarka po tytule i numerze zgłoszenia
- Możliwość sortowania kolumn
- Masowe akcje: przypisanie, zmiana statusu, archiwizacja
- Grupowanie zgłoszeń wg statusu (Aktywne, Rozwiązane, Zamknięte)
- Paginacja lub infinite scroll
- Historia i trendy rozwiązania ticketów

#### Moduł Zgłoszeń - Widok Szczegółów
- Prezentacja pełnych szczegółów zgłoszenia (numer, tytuł, opis)
- Dane o maszynie (marka, model, numer seryjny, raw data)
- Dane o organizacji (nazwa, kontakt, umowa)
- Priorytet i status zgłoszenia
- Przypisany serwisant
- Data utworzenia i ostatniej aktualizacji
- Oś czasu (timeline) zmian statusu, przypisań, notek, załączników, logów
- Pełny audyt wszystkich akcji z informacją kto, co i kiedy zmienił
- Workflow statusów: Draft → Wysłany → Przyjęty → W trakcie → Rozwiązany → Wznowiony → Zamknięty
- Możliwość zmiany statusu na: Przyjęty, W trakcie, Rozwiązany, Zamknięty
- Dodawanie wewnętrznych notek (niewidocznych dla klienta)
- Przypisanie do siebie lub innego serwisanta
- Chat/komunikacja z operatorem/klientem w tickecie
- Dodawanie uzasadnienia do istotnych zmian
- Eksport historii ticketu
- Załączniki i galeria

#### Moduł Organizacji - Lista i Zarządzanie
- Lista wszystkich klientów z danymi kontaktowymi
- Status umowy/serwisu dla każdej organizacji
- Liczba maszyn i aktywnych ticketów
- Stan alarmów dla każdej organizacji
- Przycisk do inicjowania onboardingu nowych klientów (wysłanie zaproszenia)
- Edycja danych kontaktowych organizacji (tylko dla admina serwisu)

#### Moduł Organizacji - Szczegóły
- Zarządzanie maszynami: przypisywanie, edycja
- Generowanie tokenu API dla organizacji
- Przeglądanie statusów maszyn
- Widok live danych z maszyn
- Historia alarmów i problemów
- Przechowywanie dat ostatnich przeglądów
- Konfigurowalne interwały przeglądów

#### Moduł Administracji (dostępny tylko dla Administratorów Serwisu)
- Lista wszystkich serwisantów z rolą, statusem konta
- Indykatory ostatniej aktywności serwisantów
- Dodawanie nowych loginów serwisantów
- Resetowanie/zmiana hasła serwisanta
- Edycja danych konta serwisanta
- Nadawanie/odbieranie uprawnień administratora
- Monitorowanie obciążenia zespołu
- Opcjonalnie: statystyki efektywności serwisantów

### 3.2. Portal Klienta

#### Moduł Logowania i Aktywacji
- Ekran logowania z polami nazwa użytkownika i hasło
- Proces aktywacji konta przez link z emaila
- Ustawienie hasła podczas aktywacji
- Link aktywacyjny ważny przez określony okres
- Obsługa błędów: niepoprawny link, wygaśnięty link, konto już aktywne

#### Ekran Główny (Dashboard)
- Kafelki statusów maszyn: liczba maszyn w statusie Active, Alarm, Maintenance
- Kafelki statusów zgłoszeń: liczba moich aktywnych ticketów oraz wszystkich zgłoszeń organizacji
- Kafelek z ostatnimi aktywnościami organizacji/ticketów
- Szybki dostęp do administracji (kafelek dla Administratora prowadzący do zakładki "Zespół")
- Menu nawigacyjne z zakładkami: Dashboard, Zgłoszenia, Zespół (dla Administratora)
- Opcja wylogowania

#### Moduł Zgłoszeń - Widok Listy
- Lista wszystkich zgłoszeń organizacji z statusem, priorytetem i maszyną
- Filtrowanie po statusie, maszynie, właścicielu (twórcy zgłoszenia)
- Przycisk "Utwórz nowe zgłoszenie"
- Możliwość "porzucenia" zgłoszenia w statusie "Szkic" lub "Wysłany"
- Sortowanie i paginacja
- Widoczna liczba nowych/nieodczytanych zgłoszeń

#### Moduł Zgłoszeń - Widok Szczegółów
- Oś czasu (timeline) szczegółów zgłoszenia:
  - Działania organizacji (lewa strona)
  - Działania serwisu (prawa strona)
  - Zmiany statusów (środek)
- Możliwość dodawania komentarzy
- Możliwość dodawania załączników
- Możliwość aktualizacji opisu zgłoszenia (dla Operatorów tylko w swoich ticketach, dla Administratora wszędzie)
- Przycisk "Wznów ticket" na zgłoszeniach w statusie "Rozwiązany" przez mniej niż 14 dni (dla Administratora)
- Zakładka z maszyna i jej szczegółami
- Historia zmian i notek serwisu
- Możliwość pobierania attachmentów

#### Moduł Administracji Organizacji (Zespół) - dostępny dla Administratora
- Lista wszystkich użytkowników organizacji
- Dla każdego użytkownika: imię, email, ostatnia aktywność, liczba utworzonych ticketów
- Przycisk do dodania nowego operatora (wysłanie zaproszenia przez email)
- Status aktywacji dla każdego operatora (oczekuje na aktywację/aktywny)
- Możliwość usunięcia (dezaktywacji) operatora - odebranie dostępu

---

## 4. Granice Produktu

### Funkcjonalności Wykluczone z MVP

1. Powiadomienia real-time push/email (Email tylko do logowania i onboardingu)
2. Chat real-time - komunikacja ograniczona do komentarzy asynchronicznych
3. Automatyczne przydzielanie zgłoszeń na podstawie algorytmów
4. Integracje z zewnętrznymi systemami ticketingowymi
5. Zaawansowane raportowanie i analytics (poza podstawowymi KPI)
6. Automatyczne przypomnienia o przeterminowanych ticketach
7. Funkcja video-call czy wsparcia zdalnego
8. Wielopoziomowa obsługa (routing do specjalistów)
9. Szablony zgłoszeń
10. Zarządzanie umowami serwisowymi i SLA
11. Automatyczne zamykanie ticketów
12. Integracja z systemami CRM

### Ograniczenia Techniczne

1. Integracja logów maszyn ograniczona do predefiniowanych formatów
2. Brak historii logów starszych niż 90 dni (w podstawowej wersji)
3. Maksymalny rozmiar załącznika: 10 MB na plik
4. Maksymalna liczba współbieżnych użytkowników w MVP: 100
5. Brak synchronizacji offline - aplikacja wymaga połączenia internetowego
6. Ograniczona liczba niestandardowych pól dla organizacji

### Ograniczenia Organizacyjne

1. MVP wspiera maksymalnie jeden język - Polish (z możliwością zmiany na początkową konfigurację)
2. System nie obsługuje rozliczeń i fakturowania
3. Brak możliwości podziału na regiony lub podmioty
4. Brak wielotekstowych katalogów produktów/maszyn
5. Ogólne logowanie dla całej bazy - brak SAML/OAuth

---

## 5. Historyjki Użytkowników

### 5.1. Moduł Logowania i Uwierzytelniania - Portal Serwisu

#### US-001: Zalogowanie się serwisanta do portalu serwisu

Tytuł: Zalogowanie się jako Serwisant

Opis: Jako Serwisant chcę zalogować się na dedykowany portal serwisowy, aby uzyskać dostęp do panelu głównego i moich zadań.

Kryteria akceptacji:
- Aplikacja wyświetla ekran logowania z polami: login i hasło
- Możliwość wprowadzenia poprawnych poświadczeń
- Po kliknięciu "Zaloguj" system weryfikuje dane przeciwko bazie
- Po poprawnym logowaniu użytkownik jest przekierowywany na Dashboard
- Po niepoprawnym logowaniu wyświetla się komunikat błędu: "Niepoprawny login lub hasło"
- Sesja użytkownika jest utrzymywana na 30 minut inaktywności
- Po timeout'ie system automatycznie wylogowuje i wyświetla komunikat
- Hasło w polu wejściowym jest zasłonięte (bullets/dots)

---

#### US-002: Resetowanie zapomnianego hasła - Serwisant

Tytuł: Resetowanie hasła przy użyciu opcji "Zapomniałem hasła"

Opis: Jako Serwisant, który zapomniał hasło, chcę móc zresetować je poprzez link wysłany na mój email serwisowy, aby odzyskać dostęp do systemu.

Kryteria akceptacji:
- Na ekranie logowania dostępny jest link "Zapomniałem hasła"
- Kliknięcie linku otwiera formularz z polem email
- Po wpisaniu email i kliknięciu "Wyślij" system wysyła link resetujący
- Komunikat potwierdzający wysłanie maila: "Email z instrukcjami został wysłany"
- Link w mailu zawiera token resetujący ważny 24 godziny
- Kliknięcie linku otwiera formularz do wpisania nowego hasła
- Po wygaśnięciu linku wyświetla się komunikat: "Link wygasł, spróbuj ponownie"
- Nowe hasło musi spełniać wymagania bezpieczeństwa (min 8 znaków, duża litera, liczba, znak specjalny)
- Po zmianie hasła użytkownik może zalogować się nowymi poświadczeniami

---

#### US-003: Wybór języka przed zalogowaniem

Tytuł: Zmiana języka przed zalogowaniem

Opis: Jako Serwisant chcę móc wybrać język interfejsu (Polski/English) przed zalogowaniem, aby pracować w preferowanym języku.

Kryteria akceptacji:
- Na ekranie logowania dostępny jest selektor języka (flaga lub lista rozwijana)
- Opcje dostępne: Polski (domyślnie) i English
- Zmiana języka stosuje się natychmiast do całego ekranu logowania
- Wybrana język jest zapamiętywany w localstorage (przeglądarka)
- Po zalogowaniu wybrany język pozostaje aktywny na całym portalu
- Administratorzy mogą zmienić domyślny język w konfiguracji serwisu

---

### 5.2. Moduł Logowania i Aktywacji - Portal Klienta

#### US-004: Zalogowanie się operatora do portalu klienta

Tytuł: Zalogowanie się jako Operator

Opis: Jako Operator chcę móc zalogować się na portal klienta, aby uzyskać dostęp do informacji o moich maszynach i zgłoszeniach serwisowych.

Kryteria akceptacji:
- Aplikacja wyświetla ekran logowania z polami: nazwa użytkownika i hasło
- Po wprowadzeniu poprawnych poświadczeń i kliknięciu "Zaloguj" system weryfikuje dane
- Po poprawnym logowaniu użytkownik jest przekierowywany na Dashboard organizacji
- Po niepoprawnym logowaniu wyświetla się komunikat: "Niepoprawna nazwa użytkownika lub hasło"
- Sesja użytkownika jest utrzymywana przez 60 minut inaktywności
- Po timeout'ie system automatycznie wylogowuje i wyświetla komunikat
- Przycisk "Wyloguj" jest dostępny na każdej stronie portalu

---

#### US-005: Aktywacja konta operatora przez link z emaila

Tytuł: Aktywacja konta poprzez link z zaproszenia

Opis: Jako nowy Operator, który otrzymał zaproszenie do organizacji, chcę aktywować konto poprzez link w emailu i ustawić swoje hasło, aby uzyskać dostęp do portalu klienta.

Kryteria akceptacji:
- Administrator organizacji wysyła zaproszenie z linkiem aktywacyjnym
- Email zawiera: link aktywacyjny, nazwę organizacji, instrukcje
- Link aktywacyjny jest ważny przez 7 dni
- Kliknięcie linku otwiera stronę aktywacji z polami: email (wstępnie wypełnione), hasło, potwierdzenie hasła
- Hasło musi spełniać wymagania: min 8 znaków, duża litera, liczba, znak specjalny
- Po wpisaniu prawidłowego hasła i kliknięciu "Aktywuj" konto jest aktywne
- Komunikat potwierdzający: "Konto aktywne! Możesz teraz się zalogować"
- Wygaśnięty link wyświetla komunikat: "Link wygasł. Poproś administratora o ponowne wysłanie zaproszenia"
- Już aktywne konto wyświetla komunikat: "Konto już aktywne. Przejdź do logowania"
- Po aktywacji użytkownik może zalogować się na portal

---

### 5.3. Dashboard - Portal Serwisu

#### US-006: Wyświetlenie kafelków KPI na dashboardzie

Tytuł: Widok kafelków KPI z liczbą aktywnych zgłoszeń

Opis: Jako Serwisant chcę widzieć liczbę aktywnych zgłoszeń i alarmów krytycznych na kafelkach na dashboardzie, aby natychmiast rozpoznać najpilniejsze zadania.

Kryteria akceptacji:
- Dashboard wyświetla kafelki KPI po zalogowaniu
- Kafelek "Aktywne zgłoszenia" pokazuje łączną liczbę otwartych ticketów
- Kafelek "Krytyczne" pokazuje liczbę zgłoszeń z priorytetem "Krytyczny" (w kolorze czerwonym)
- Kafelek "Wysoki priorytet" pokazuje liczbę zgłoszeń z priorytetem "Wysoki" (w kolorze pomarańczowym)
- Kafelek "Średni priorytet" pokazuje liczbę zgłoszeń z priorytetem "Średni"
- Kafelek "Rozwiązane" pokazuje liczbę ticketów w statusie "Rozwiązany" (ostatnie 24 h)
- Kafelek "Zamknięte" pokazuje liczbę ticketów w statusie "Zamknięty" (ostatnie 24 h)
- Liczby aktualizują się na żywo (co 30 sekund)
- Kliknięcie kafelka otwiera filtrowaną listę zgłoszeń odpowiadającą kategor
- Każdy kafelek zawiera ikonę odpowiadającą kategorii
- Na kafelkach z wysokim priorytetem wyświetla się animacja alertu (pulsowanie)

---

#### US-007: Wyświetlenie wykresu trendów zgłoszeń

Tytuł: Wizualizacja trendów zgłoszeń i rozkładu po priorytetach

Opis: Jako Serwisant chcę mieć wykres trendów zgłoszeń i rozkład po priorytetach, aby monitorować obciążenie serwisu i strukturę problemów.

Kryteria akceptacji:
- Dashboard wyświetla wykres liniowy trendów zgłoszeń z ostatnich 30 dni
- Oś X: daty (od 30 dni temu do dziś)
- Oś Y: liczba zgłoszeń
- Linia wykresu pokazuje trend otwartych, rozwiązanych i zamkniętych ticketów
- Poniżej znajduje się chart kołowy/słupkowy z rozkładem po priorytetach
- Kategorie: Krytyczny, Wysoki, Średni, Niski
- Każda kategoria ma przypisany kolor
- Hover na segmencie wykresu pokazuje dokładne liczby i procenty
- Wykresy aktualizują się co godzinę
- Dostępna opcja przefiltrowania po dacie (ostatnie 7 dni, 30 dni, 90 dni, custom)

---

#### US-008: Wyświetlenie listy ostatnich zdarzeń i powiadomień

Tytuł: Widok ostatnich zdarzeń i szybkie linki do ticketów

Opis: Jako Serwisant chcę widzieć listę ostatnich zdarzeń oraz moich ticketów wymagających akcji, aby szybko przejść do działań operacyjnych.

Kryteria akceptacji:
- Dashboard wyświetla sekcję "Ostatnie zdarzenia" z listą 10 ostatnich zmian
- Każde zdarzenie zawiera: typ (nowy ticket, zmiana statusu, nowy komentarz), opis, datę
- Sekcja "Moje tickety" wyświetla tickets przypisane do zalogowanego serwisanta
- Filtry szybkie: "Oczekujące akcje", "Nowe", "W trakcie"
- Linkuje do listy zgłoszeń z odpowiadającym filtrem
- Sekcja "Organizacje z alarmami" wyświetla 5 organizacji z najnowszymi alarmami
- Sekcja "Nadchodzące przeglądy" wyświetla maszyny z planowanymi przegląd
- Wszystkie sekcje zawierają linki "Pokaż wszystkie"
- Zdarzenia aktualizują się co 2 minuty
- Możliwość odśwież strony poprzez przycisk refresh

---

### 5.4. Menu Nawigacyjne - Portal Serwisu

#### US-009: Nawigacja między modułami i zmiana języka

Tytuł: Menu nawigacyjne z przełączaniem sekcji i języka

Opis: Jako Serwisant chcę mieć menu z zakładkami nawigacyjnymi, możliwość zmiany języka i przycisk Wyloguj, aby wygodnie przełączać się między sekcjami.

Kryteria akceptacji:
- Menu znajduje się na górze lub z lewej strony (responsywne na urządzeniach mobilnych)
- Menu zawiera zakładki: Dashboard, Zgłoszenia, Organizacje, Administracja (tylko dla adminów)
- Bieżąca zakładka jest wyróżniona (bold, inny kolor tła)
- Kliknięcie zakładki nawiguje do odpowiedniej sekcji
- Selektor języka wyświetla flagę lub tekst z aktualnym językiem
- Kliknięcie selektora otwiera menu z opcjami: Polski, English
- Zmiana języka przeładowuje stronę i aplikuje nowy język do wszystkich tekstów
- Przycisk "Wyloguj" jest widoczny w menu
- Kliknięcie "Wyloguj" kończy sesję i przekierowuje na ekran logowania
- Na urządzeniach mobilnych menu można zwinąć/rozwinąć (hamburger menu)
- Logo/nazwa aplikacji w menu odsyła na Dashboard

---

### 5.5. Widok Listy Zgłoszeń - Portal Serwisu

#### US-010: Wyświetlenie listy wszystkich zgłoszeń z podstawowymi informacjami

Tytuł: Przeglądanie listy zgłoszeń z statusem, priorytetem i maszyną

Opis: Jako Serwisant chcę przeglądać listę wszystkich zgłoszeń w serwisie z widocznym statusem, priorytetem i organizacją, aby szybko znaleźć zgłoszenia wymagające działania.

Kryteria akceptacji:
- Aplikacja wyświetla tabelę/listę ze wszystkimi zgłoszeniami (domyślnie sortowana po dacie DESC)
- Każda pozycja zawiera: numer zgłoszenia, tytuł, status, priorytet, organizacja, maszyna, przypisany serwisant, data utworzenia
- Liczby są czytelne i sformatowane z odpowiednimi kolorami (Krytyczny = czerwony, Wysoki = pomarańczowy, itp.)
- Możliwość sortowania po kolumnach: numer, data, priorytet, status
- Tabela zawiera paginację (20 elementów na stronę lub infinite scroll)
- Paginacja wyświetla informację: "Wyświetlanie 1-20 z 150 zgłoszeń"
- Kliknięcie na wiersz otwiera szczegóły zgłoszenia
- Tabela jest responsywna - na mobilnym wyświetla karty zamiast tabeli

---

#### US-011: Filtrowanie zgłoszeń po statusie, priorytecie, organizacji i serwisancie

Tytuł: Zaawansowane filtrowanie listy zgłoszeń

Opis: Jako Serwisant chcę filtrować tickety po organizacji, maszynie, statusie, priorytecie i sobie, aby sprawnie zarządzać zgłoszeniami.

Kryteria akceptacji:
- Panel filtrowania jest dostępny nad tabelą zgłoszeń
- Filtry: Status, Priorytet, Organizacja, Maszyna, Przypisany do, Data od/do
- Każdy filtr ma opcję "Wszystkie" (domyślnie)
- Status: opcje - Nowy, Przyjęty, W trakcie, Rozwiązany, Wznowiony, Zamknięty, Archiwizowany
- Priorytet: opcje - Krytyczny, Wysoki, Średni, Niski
- Organizacja: dynamiczna lista dostępnych organizacji
- Maszyna: dynamiczna lista maszyn wybranej organizacji
- Przypisany do: opcja "Moje", lista serwisantów, "Nieprzypisane"
- Możliwość łączenia wielu filtrów jednocześnie (AND logic)
- Przycisk "Szukaj" / automatyczne aktualizowanie wyników w czasie pisania
- Przycisk "Resetuj filtry" - czyści wszystkie filtry
- Liczba aktywnych filtrów wyświetla się na przycisku "Filtry" (jeśli zwinięte)
- Wybrane filtry są zapamiętywane w URL (możliwość udostępnienia linka)
- Wyniki filtrowania pokazują komunikat: "Znaleziono 45 zgłoszeń spełniających kryteria"

---

#### US-012: Wyszukiwanie zgłoszeń po tytule i numerze

Tytuł: Szybkie wyszukiwanie ticketów

Opis: Jako Serwisant chcę szybciej odnajdować zgłoszenia dzięki wyszukiwarce po tytule i numerze, aby nie tracić czasu na przeglądanie długiej listy.

Kryteria akceptacji:
- Wyszukiwarka znajduje się na górze listy zgłoszeń (search box)
- Wyszukiwarka obsługuje wpisanie: numeru zgłoszenia (np. "TICK-001234"), słów z tytułu
- Wyszukiwanie działa w real-time (bez konieczności kliknięcia "Szukaj")
- Wyniki pojawiają się z opóźnieniem max 500ms
- Podświetlane są znalezione fragmenty w wynikach
- Jeśli nie ma wyników: "Brak zgłoszeń pasujących do wyszukiwania"
- Historia wyszukiwań jest dostępna (ostatnie 5 wyszukiwań)
- Możliwość wyczyszczenia historii
- Wyszukiwanie jest case-insensitive
- Wyszukiwanie ignoruje znaki diakrytyczne

---

#### US-013: Grupowanie i widok zgłoszeń wg statusu

Tytuł: Organizacja zgłoszeń w grupy wg statusu

Opis: Jako Serwisant chcę grupować tickety według statusu (Aktywne, Rozwiązane, Zamknięte), aby mieć logiczny podział spraw i workflow.

Kryteria akceptacji:
- Dostępny przycisk "Grupuj wg statusu" nad tabelą
- Po aktywowaniu, lista wyświetla zgłoszenia pogrupowane w sekcjach:
  - Sekcja "Nowe" (licznik)
  - Sekcja "Przyjęte" (licznik)
  - Sekcja "W trakcie" (licznik)
  - Sekcja "Rozwiązane" (licznik)
  - Sekcja "Zamknięte" (licznik)
  - Sekcja "Archiwizowane" (licznik)
- Każda sekcja jest zwijalna/rozwijalna (accordion)
- Każda sekcja pokazuje liczbę zgłoszeń w nawiasie
- Domyślnie "Nowe" i "W trakcie" są rozwinięte, pozostałe zwinięte
- Rozwijanie sekcji pobiera dane (lazy loading)
- Po wycofaniu grupowania wraca widok tabelaryczny

---

#### US-014: Masowe akcje na zgłoszeniach

Tytuł: Wykonywanie operacji hurtowych na wybranych ticketach

Opis: Jako Serwisant-Admin chcę wykonywać masowe akcje na wybranych zgłoszeniach (przypisanie, zmiana statusu, archiwizacja), aby efektywnie zarządzać dużymi ilościami ticketów.

Kryteria akceptacji:
- Każdy wiersz w tabeli zawiera checkbox do zaznaczania
- Checkbox w nagłówku "Zaznacz wszystkie" zaznacza/odznacza wszystkie na stronie
- Po zaznaczeniu min. 1 wiersza pojawia się pasek akcji pod tabelą
- Pasek akcji zawiera:
  - Informacja: "Zaznaczono 5 zgłoszeń"
  - Przycisk "Przypisz do serwisanta" - otwiera dialog z listą serwisantów
  - Przycisk "Zmień status na" - otwiera menu z dostępnymi statusami
  - Przycisk "Archiwizuj" - archiwizuje zaznaczone
  - Przycisk "Anuluj zaznaczenie"
- Po wybraniu akcji pojawia się dialog potwierdzenia
- Po potwierdzeniu: "Akcja zastosowana do 5 zgłoszeń"
- Historia masowych akcji jest zapisywana w audycie
- Jeśli akcja nie może być zastosowana do wszystkich (np. status niedostępny), wyświetla się ostrzeżenie z powodami

---

### 5.6. Szczegóły Zgłoszenia - Portal Serwisu

#### US-015: Przeglądanie pełnych szczegółów zgłoszenia

Tytuł: Widok szczegółów pojedynczego zgłoszenia z kontekstem technicznym

Opis: Jako Serwisant chcę zobaczyć pełne szczegóły ticketa – dane zgłoszenia, maszynę, załączniki, priorytet, organizację, status, aby mieć natychmiast potrzebny kontekst.

Kryteria akceptacji:
- Po kliknięciu zgłoszenia z listy otwiera się strona szczegółów
- URL zmienia się na /tickets/{id}
- Strona zawiera karty informacyjne:
  - Karta "Podstawowe": numer, tytuł, opis, priorytet, status, data utworzenia
  - Karta "Maszyna": nazwa, model, numer seryjny, lokalizacja, ostatni stan
  - Karta "Raw Data": surowe dane z maszyny (JSON/XML w formacie read-only)
  - Karta "Organizacja": nazwa, kontakt, osoba do kontaktu
  - Karta "Przypisanie": obecny serwisant, możliwość zmiany
- Każdy element posiada etykietę i wartość
- Dane są sformatowane i czytelne
- Brak elementów edytowalnych w sekcjach informacyjnych (poza przypisaniem)
- Przycisk "Edytuj" w karcie podstawowe umożliwia edycję tytułu i opisu (tylko dla admina/właściciela)
- Przycisk "Powrót" wraca do listy zgłoszeń z zachowaniem filtrów

---

#### US-016: Przeglądanie osi czasu (timeline) zdarzeń zgłoszenia

Tytuł: Historia zmian statusu, przypisań, komentarzy i załączników w postaci osi czasu

Opis: Jako Serwisant chcę widzieć całą oś czasu zgłoszenia w formie łatwego do prześledzenia timeline (zdarzenia, statusy, wiadomości, załączniki), aby lepiej analizować przebieg obsługi.

Kryteria akceptacji:
- Sekcja "Oś Czasu" wyświetla się poniżej szczegółów zgłoszenia
- Timeline zawiera chronologicznie uporządkowane zdarzenia
- Każde zdarzenie zawiera:
  - Avatar użytkownika (inicjały lub zdjęcie)
  - Imię i nazwisko użytkownika / "System"
  - Typ zdarzenia (ikona): zmiana statusu, przypisanie, komentarz, załącznik, notatka, zmiana pola
  - Opis zdarzenia: "zmienił status na 'W trakcie'", "dodał komentarz", "dodał załącznik document.pdf"
  - Czas zdarzenia (względny: "2 minuty temu" i bezwzględny: "19.10.2025 14:30")
  - Treść komentarza (jeśli dotyczy)
  - Miniatura załącznika (jeśli dotyczy)
- Zdarzenia są posortowane od najnowszych
- Możliwość filtrowania: "Wszystkie", "Tylko statusy", "Tylko komentarze", "Tylko załączniki"
- Możliwość rozwijania zdarzeń, aby zobaczyć więcej detali
- Pełny audyt: każda zmiana zawiera informację kto, co, kiedy zmienił
- Load more / paginacja dla historii powyżej 50 zdarzeń
- Brak limitów czasowych - dostępna pełna historia

---

#### US-017: Zmiana statusu zgłoszenia

Tytuł: Przepływ zmian statusu ticketa

Opis: Jako Serwisant chcę zmieniać status ticketa na: Przyjęty, W trakcie, Rozwiązany, Zamknięty, aby odzwierciedlić cykl serwisowy.

Kryteria akceptacji:
- Pole "Status" na stronie szczegółów jest interaktywne (dropdown/przycisk)
- Możliwe statusy w menu: Przyjęty, W trakcie, Rozwiązany, Wznowiony, Zamknięty
- Nie wszystkie przejścia statusów są dozwolone:
  - Z "Nowy" → "Przyjęty" LUB "Zamknięty" (odrzucenie)
  - Z "Przyjęty" → "W trakcie" LUB "Zamknięty"
  - Z "W trakcie" → "Rozwiązany" LUB "W trakcie"
  - Z "Rozwiązany" → "Wznowiony" LUB "Zamknięty"
  - Z "Wznowiony" → "W trakcie" LUB "Zamknięty"
  - Z "Zamknięty" → (brak możliwości zmiany, tylko admin może wznowić)
- Niedozwolone przejścia są wyszarzone/wyłączone
- Po kliknięciu nowego statusu pojawia się dialog potwierdzenia
- Dialog zawiera pole "Uzasadnienie" (obowiązkowe dla zmian na "Rozwiązany" lub "Zamknięty")
- Przykład tekstu: "Dlaczego zamykasz to zgłoszenie?"
- Po potwierdzeniu status zmienia się natychmiast
- Zmiana jest widoczna na osi czasu z nazwą zmieniającego i uzasadnieniem
- Wiadomość email nie jest wysyłana (brak powiadomień w MVP)

---

#### US-018: Dodawanie wewnętrznych notek do zgłoszenia

Tytuł: Dodawanie notek widocznych tylko dla zespołu serwisu

Opis: Jako Serwisant chcę przeglądać i dodawać historię zmian statusów, przypisań oraz wewnętrzne notatki, aby transparentnie dokumentować postęp i decyzje.

Kryteria akceptacji:
- W sekcji "Komentarze" dostępna jest opcja "Notatka wewnętrzna"
- Pole tekstowe do wpisania notatki
- Notatka zawiera toggle: "Widoczna tylko dla zespołu" (domyślnie zaznaczone)
- Przycisk "Dodaj notatkę"
- Po dodaniu notatka pojawia się na timeline z oznaką "Notatka wewnętrzna" i ikoną zamka
- Wewnętrzne notatki NIE są widoczne dla klienta w portalu klienta
- Wewnętrzne notatki zawierają informację: nazwa autora, czas, treść
- Możliwość edycji własnych notek (tylko w ciągu 5 minut po dodaniu)
- Możliwość usunięcia własnych notek (tylko w ciągu 5 minut po dodaniu)
- Historia edycji notek jest dostępna
- Wewnętrzne notatki mogą zawierać formatowanie: bold, italic, list

---

#### US-019: Przypisanie zgłoszenia do serwisanta

Tytuł: Przydzielenie ticketa konkretnemu pracownikowi

Opis: Jako Serwisant chcę przypisać ticket do siebie lub innego serwisanta, aby jasno określić odpowiedzialność.

Kryteria akceptacji:
- Pole "Przypisane do" wyświetla bieżącego przypisanego serwisanta (jeśli brak: "Nieprzypisane")
- Kliknięcie pola otwiera dropdown z listą serwisantów
- Lista zawiera:
  - "Mnie" (skrót do zalogowanego użytkownika)
  - "Nieprzypisane" (opcja usunięcia przypisania)
  - Alfabetyczna lista serwisantów (imię, nazwisko)
- Po wyborze serwisanta przypisanie zmienia się natychmiast
- Zmiana jest zapisywana w bazie danych
- Zdarzenie "przypisania" pojawia się na timeline
- Jeśli ticket już jest przypisany, pojawia się pytanie: "Czy na pewno przenieść ticket od [poprzedni serwisant]?"
- Opcja "Przypisz do mnie" (keyboard shortcut ALT+M) przypisuje ticket do zalogowanego użytkownika
- Serwisant może przypisać ticket do siebie z listy lub ze szczegółów
- Możliwość grupowego przypisania z masowych akcji

---

#### US-020: Komunikacja w tickecie - dodawanie komentarzy dla klienta

Tytuł: Wymiana wiadomości z operatorem/klientem w tickecie

Opis: Jako Serwisant chcę komunikować się w tickecie przez komentarze z operatorem/klientem, aby wyjaśniać szczegóły i prosić o dodatkowe informacje.

Kryteria akceptacji:
- Sekcja "Komentarze" wyświetla wszystkie komentarze (wewnętrzne i publiczne)
- Pole do wpisania nowego komentarza na dole
- Domyślnie komentarz jest "Publiczny" (widoczny dla klienta)
- Checkbox "Tylko dla zespołu" zmienia komentarz na wewnętrzny (niewidoczny dla klienta)
- Przycisk "Wyślij komentarz"
- Po wysłaniu komentarz pojawia się na timeline
- Komentarz zawiera: avatar, imię i nazwisko, treść, czas, informacja czy jest wewnętrzny
- Możliwość edycji własnego komentarza (do 1 godziny po wysłaniu)
- Możliwość usunięcia własnego komentarza (do 1 godziny po wysłaniu)
- Komentarz może zawierać formatowanie: bold, italic, listy, linki
- Możliwość cytowania poprzedniego komentarza
- Wzmianki (@mention) mogą oznaczyć innego serwisanta (notifications nie są wysyłane, ale @ jest widoczny)
- Max długość komentarza: 5000 znaków
- Jeśli ticket jest "Zamknięty", komentarze są read-only (chyba że admin pozwoli)

---

#### US-021: Dodawanie uzasadnienia do istotnych zmian

Tytuł: Dokumentacja przyczyn dla zmian statusu (Rozwiązany, Zamknięty, Odrzucony)

Opis: Jako Serwisant chcę dodawać uzasadnienie do istotnych zmian (np. zamknięcia, odrzucenia wznowienia), aby każda operacja była udokumentowana.

Kryteria akceptacji:
- Przy zmianie statusu na "Rozwiązany" pojawia się dialog z polem obowiązkowym: "Uzasadnienie rozwiązania"
- Przy zmianie statusu na "Zamknięty" pojawia się dialog z polem obowiązkowym: "Uzasadnienie zamknięcia"
- Przy zmianie statusu na "Odrzucony" pojawia się dialog z polem obowiązkowym: "Przyczyna odrzucenia"
- Pola zawierają placeholder tekstu: "Opisz powód zmiany statusu..."
- Min 10 znaków w uzasadnieniu
- Po potwierdzeniu uzasadnienie jest zapisywane w bazie
- Uzasadnienie pojawia się na timeline jako część zdarzenia zmiany statusu
- Uzasadnienie jest widoczne dla klienta w portalu klienta
- Historia uzasadnień jest dostępna w audycie

---

#### US-022: Eksport historii ticketu

Tytuł: Pobieranie pełnej historii zgłoszenia w formacie PDF

Opis: Jako Serwisant chcę móc wyeksportować pełną historię ticketu, aby dokumentować proces serwisowy.

Kryteria akceptacji:
- Na stronie szczegółów ticketu dostępny jest przycisk "Eksportuj"
- Po kliknięciu pojawia się menu opcji:
  - "Eksportuj jako PDF"
  - "Eksportuj jako CSV"
  - "Eksportuj jako JSON"
- Eksport PDF zawiera:
  - Nagłówek z numerem i tytułem ticketu
  - Sekcja: Szczegóły (status, priorytet, organizacja, maszyna, przypisany)
  - Sekcja: Pełna oś czasu z komentarzami, zmianami statusu, załącznikami
  - Sekcja: Załączniki (lista z linkami)
  - Data wygenerowania raportu
- PDF jest czytelnie sformatowany z logo serwisu
- Plik pobiera się z nazwą: "TICK-{number}-{date}.pdf"
- Eksport CSV zawiera kolumny: Data, Typ, Użytkownik, Opis, Detale
- Eksport JSON zawiera pełną strukturę danych ticketu

---

#### US-023: Przeglądanie i pobieranie załączników

Tytuł: Obsługa plików załączonych do zgłoszenia

Opis: Jako Serwisant chcę przeglądać i pobierać załączniki dołączone do zgłoszenia, aby analizować dokumenty i zrzuty ekranu dotyczące problemu.

Kryteria akceptacji:
- Sekcja "Załączniki" wyświetla listę wszystkich plików
- Każdy plik zawiera: ikonę typu, nazwę, rozmiar, datę dodania, autora
- Możliwość pobrania pliku - kliknięcie na plik pobiera go
- Możliwość podglądu zdjęć (JPG, PNG, GIF) w lightbox
- Możliwość podglądu dokumentów (PDF, TXT) w przeglądarce
- Pliki wideo (MP4, MOV) mają player HTML5
- Max rozmiar pliku: 10 MB
- Obsługiwane typy: JPG, PNG, PDF, TXT, DOC, DOCX, XLS, XLSX, ZIP, MP4, MOV
- Jeśli plik nie może być podejrzany, wyświetla się komunikat z opcją pobrania
- Brak zabezpieczeń poprzez hasło (dostęp na bazie uprawnień do ticketu)
- Historia pobrań jest rejestrowana w audycie

---

### 5.7. Moduł Organizacji - Portal Serwisu

#### US-024: Wyświetlenie listy organizacji z kluczowymi informacjami

Tytuł: Przeglądanie listy klientów (organizacji)

Opis: Jako Serwisant(-Admin) chcę widzieć listę organizacji, liczbę aktywnych maszyn/ticketów, statusy umów/przeglądów, aby identyfikować "gorące" punkty wymagające reakcji.

Kryteria akceptacji:
- Aplikacja wyświetla tabelę/listę ze wszystkimi organizacjami
- Każda pozycja zawiera:
  - Nazwa organizacji
  - Status serwisu (Aktywny, Wstrzymany, Wygasły)
  - Liczba aktywnych maszyn
  - Liczba aktywnych ticketów
  - Liczba alarmów
  - Ostatni alarm (data i opis)
  - Data ostatniego przeglądu
  - Osoba kontaktowa
  - Przycisk "Szczegóły"
- Organizacje z alarmami są wyróżnione (różny kolor)
- Organizacje z przeterminowanymi przegląd są wyróżnione (ostrzeżenie)
- Możliwość sortowania po: nazwa, liczba ticketów, liczba alarmów, status
- Tabela zawiera paginację
- Tabela jest responsywna

---

#### US-025: Inicjowanie onboardingu nowej organizacji

Tytuł: Zaproszenie nowego klienta do systemu

Opis: Jako Serwisant-Admin chcę tworzyć i zapraszać nowe organizacje (email do admina klienta), aby onboardingować nowych klientów.

Kryteria akceptacji:
- Na stronie listy organizacji dostępny jest przycisk "+ Nowa organizacja"
- Kliknięcie otwiera formularz onboardingu z polami:
  - Nazwa organizacji (obowiązkowe)
  - Email administratora klienta (obowiązkowe)
  - Osoba kontaktowa (imię, nazwisko)
  - Adres, telefon, strona internetowa
  - Kraj (lista rozwijana)
  - Typ umowy (lista: Roczna, Quarterly, Pay-as-you-go)
  - Data rozpoczęcia umowy
  - Liczba maszyn (przybliżenie)
  - Notatki wewnętrzne
- Walidacja: email musi być poprawny, nazwa min 3 znaki
- Przycisk "Wyślij zaproszenie"
- System generuje unikatowy token aktywacyjny
- Email do administratora klienta zawiera:
  - Link do aktywacji konta
  - Login czasowy (email lub username)
  - Instrukcje
  - Dane kontaktowe administratora serwisu
- Zaproszenie jest ważne 7 dni
- Status organizacji zmienia się na "Oczekuje aktywacji"
- Po odpowiedniku przez administratora klienta, status zmienia się na "Aktywna"
- Informacja o nowej organizacji pojawia się w notifikowaniu admina serwisu (w systemie, nie email)
- Historia onboardingu jest rejestrowana w audycie

---

#### US-026: Edycja danych organizacji

Tytuł: Aktualizacja informacji kontaktowych organizacji

Opis: Jako Serwisant-Admin chcę edytować dane kontaktowe organizacji (nazwa, adres, telefon, osoba), aby móc aktualizować niezbędne informacje.

Kryteria akceptacji:
- Na stronie szczegółów organizacji dostępny jest przycisk "Edytuj"
- Kliknięcie otwiera formularz edycji z polami:
  - Nazwa organizacji
  - Osoba kontaktowa (imię, nazwisko)
  - Email kontaktowy
  - Telefon
  - Adres
  - Kraj
  - Strona internetowa
  - Notatki
  - Status umowy (lista: Aktywna, Wstrzymana, Wygasła)
- Pola są preuzupełnione obecnymi wartościami
- Walidacja jak przy tworzeniu
- Przycisk "Zapisz"
- Po zapisaniu dane się aktualizują natychmiast
- Historia zmian jest zapisywana w audycie z informacją: kto, co, kiedy zmienił
- Po edycji wyświetla się komunikat: "Dane organizacji zaktualizowane"
- Edycja danych nie wymaga potwierdzenia przez administratora klienta

---

#### US-027: Zarządzanie maszynami w organizacji

Tytuł: Przypisywanie i edycja maszyn dla organizacji

Opis: Jako Serwisant(-Admin) chcę przypisywać i edytować maszyny dla organizacji, aby odwzorowywać stan faktyczny parku maszynowego klienta.

Kryteria akceptacji:
- Na stronie szczegółów organizacji jest sekcja "Maszyny"
- Wyświetla się lista maszyn należących do organizacji z:
  - Numerem seryjnym
  - Marką i modelem
  - Statusem (Aktywna, Nieaktywna, Konserwacja)
  - Data ostatniego kontaktu
  - Ostatni alarm (data)
  - Przycisk "Edytuj" i "Usuń"
- Przycisk "+ Nowa maszyna" otwiera formularz:
  - Numer seryjny (obowiązkowe, unikat)
  - Marka (lista lub custom)
  - Model (lista lub custom)
  - Typ (lista: CNC, 3D Printer, Lathe, Custom)
  - Lokalizacja (obowiązkowe)
  - Adres IP (opcjonalne)
  - Opis (opcjonalne)
- Kliknięcie "Edytuj" na maszynie otwiera formularz edycji
- Pole "Numer seryjny" jest read-only (nie da się zmienić)
- Możliwość edycji: marki, modelu, lokalizacji, adresu IP, opisu, statusu
- Przycisk "Usuń" usuwa maszynę z potwierdzeniem
- Jeśli maszyna ma aktywne tickety, wyświetla się ostrzeżenie
- Po dodaniu/edycji maszyny pojawia się komunikat potwierdzenia
- Maszyna na liście pojawia się z animacją fade-in

---

#### US-028: Generowanie tokenu API dla maszyn

Tytuł: Tworzenie klucza dostępu API dla integracji maszyn

Opis: Jako Serwisant-Admin chcę generować tokeny API dla organizacji, aby umożliwić maszynom wysyłanie logów do systemu.

Kryteria akceptacji:
- Na stronie szczegółów organizacji jest sekcja "Integracja API"
- Wyświetla się obecny token API (zamazany, np. ***-***-****-****)
- Przycisk "Pokaż token" pokazuje pełny token (dla Admina)
- Przycisk "Skopiuj token" kopiuje do schowka
- Przycisk "Regeneruj token"
- Po kliknięciu "Regeneruj" pojawia się potwierdzenie: "Czy na pewno regenerować token? Wszystkie bieżące połączenia maszyn zostaną rozłączone."
- Po potwierdzeniu generowany jest nowy token
- Stary token przestaje być ważny natychmiast
- Komunikat: "Token zmieniony. Maszyny mogą wymagać aktualizacji"
- Historia regeneracji tokenów jest w audycie
- Token API zawiera format: {organizationId}:{randomKey}:{timestamp}

---

#### US-029: Przeglądanie statusów maszyn i alertów

Tytuł: Widok live danych i historycznych alarmów maszyn

Opis: Jako Serwisant(-Admin) chcę przeglądać statusy maszyn, widok live danych i historyczne alarmy, aby monitorować stan parku maszynowego.

Kryteria akceptacji:
- Na stronie szczegółów organizacji sekcja "Maszyny" zawiera szczegółowy widok każdej maszyny
- Dla każdej maszyny wyświetla się:
  - Status (Aktywna, Nieaktywna, Alarm, Konserwacja) z kolorem
  - Data ostatniego kontaktu
  - Uptime (% czasu aktywności z ostatnich 7 dni)
  - Liczba alarmów (ostatnie 24h)
  - Live dane: temperatura, ciśnienie, prędkość (jeśli dostępne)
  - Przycisk "Szczegóły" / "Historia alarmów"
- Kliknięcie na maszynę otwiera panel szczegółowy z:
  - Pełne dane techniczne
  - Graf trendów (temperatura, ciśnienie, itp.)
  - Historia 30 ostatnich alarmów z datą, opisem, statusem
  - Możliwość filtrowania alarmów po typie
- Alarmy zawierają:
  - Identyfikator alarmu
  - Typ (temperatura_wysoka, przeciążenie, błąd_czujnika, itp.)
  - Opis
  - Data i czas
  - Status (aktywny, rozwiązany, zignorowany)
- Możliwość kliknięcia alarmu i przejścia do powiązanego ticketu
- Dane aktualizują się co 30 sekund
- Jeśli maszyna nie wysyła danych ponad 24h, pojawia się ostrzeżenie

---

#### US-030: Konfiguracja przeglądów technicznych maszyn

Tytuł: Planowanie i śledzenie przeglądów serwisowych

Opis: Jako Serwisant-Admin chcę przechowywać daty ostatnich przeglądów i konfigurować interwały przeglądów dla maszyn.

Kryteria akceptacji:
- Dla każdej maszyny w organizacji dostępne pola:
  - "Data ostatniego przeglądu" (datepicker)
  - "Interwał przeglądu" (lista: Miesiąc, Kwartał, Pół roku, Rok)
  - "Data następnego przeglądu" (obliczana automatycznie)
- Po zmian daty lub interwału pola obliczają się automatycznie
- Przycisk "Zaplanuj przegląd" otwiera formularz:
  - Data przeglądu (domyślnie obliczona)
  - Typ przeglądu (lista: Przegląd, Serwis, Konserwacja)
  - Notatki
- Po zaplanowaniu przegląd pojawia się w kalendarzu
- Na dashboardzie serwisu wyświetla się sekcja "Nadchodzące przeglądy" z maszynami wymagającymi przeglądów w ciągu 7 dni
- Maszyny z przeterminowanym przeglądem są wyróżnione na czerwono
- Historia przeglądów jest rejestrowana (data, typ, notatki, kto zaplanował)
- Możliwość ustawienia automatycz alertu (7 dni przed przeglądem)

---

### 5.8. Moduł Administracji - Portal Serwisu

#### US-031: Wyświetlenie listy serwisantów z aktivością

Tytuł: Przeglądanie zespołu serwisu

Opis: Jako Serwisant-Admin chcę mieć listę wszystkich serwisantów wraz z indikatorami aktywności, aby w razie potrzeby móc przekierowywać sprawy i monitorować obciążenie zespołu.

Kryteria akceptacji:
- Na stronie Administracja wyświetla się tabela z listą wszystkich serwisantów
- Każda pozycja zawiera:
  - Imię i nazwisko (inicjały + nazwa)
  - Email
  - Rola (Serwisant, Serwisant-Admin)
  - Status konta (Aktywny, Nieaktywny, Wstrzymany)
  - Ostatnia aktywność (data i czas, lub "nigdy")
  - Liczba przypisanych ticketów
  - Liczba rozwiązanych ticketów (ostatnie 30 dni)
  - Średni czas rozwiązania (godziny)
  - Indikator zielony/żółty/czerwony (dostępność)
  - Przycisk "Edytuj"
- Serwisanci nieaktywni ponad 7 dni są wyróżnieni (szara czcionka)
- Możliwość sortowania po kolumnach
- Paginacja
- Przycisk "Odśwież" odświeża dane

---

#### US-032: Dodawanie nowego serwisanta

Tytuł: Tworzenie konta dla nowego członka zespołu

Opis: Jako Serwisant-Admin chcę zarządzać loginami serwisantów – dodawać, edytować, resetować hasła, aby zapewnić bezpieczeństwo i płynność pracy zespołu.

Kryteria akceptacji:
- Na stronie Administracja dostępny jest przycisk "+ Nowy serwisant"
- Kliknięcie otwiera formularz:
  - Imię (obowiązkowe)
  - Nazwisko (obowiązkowe)
  - Email (obowiązkowe, unikat)
  - Login (auto-generowany z imienia i nazwiska, edytowalny)
  - Rola (lista: Serwisant, Serwisant-Admin)
  - Status (Aktywny, Nieaktywny)
  - Liczba telefoniczna (opcjonalne)
- Walidacja: email musi być poprawny, login min 4 znaki
- Po wypełnieniu formularz wyświetla przycisk "Dodaj"
- System generuje tymczasowe hasło
- Email do nowego serwisanta zawiera:
  - Login
  - Tymczasowe hasło
  - Link do logowania
  - Instrukcja zmiany hasła
  - Dane kontaktowe administratora
- Serwisant musi zmienić hasło przy pierwszym logowaniu
- Komunikat: "Serwisant dodany. Email z instrukcjami wysłany."
- Nowy serwisant pojawia się na liście ze statusem "Oczekuje zmiany hasła"

---

#### US-033: Edycja konta serwisanta

Tytuł: Modyfikacja danych serwisanta

Opis: Jako Serwisant-Admin chcę edytować dane serwisanta (imię, nazwisko, email, rolę, status).

Kryteria akceptacji:
- Kliknięcie "Edytuj" na serwisancie otwiera formularz z polami:
  - Imię
  - Nazwisko
  - Email
  - Login (read-only)
  - Rola
  - Status (Aktywny, Nieaktywny, Wstrzymany)
  - Liczba telefonu
  - Data dołączenia (read-only)
- Pola są preuzupełnione obecnymi wartościami
- Zmiana emaila wymaga potwierdzenia (wiadomość na stary email)
- Zmiana roli wymaga potwierdzenia
- Przycisk "Zapisz"
- Po zapisaniu wyświetla się komunikat
- Historia zmian jest w audycie

---

#### US-034: Resetowanie hasła serwisanta

Tytuł: Zmiana hasła pracownika przez administratora

Opis: Jako Serwisant-Admin chcę resetować/zmieniać hasło serwisanta w celu przywrócenia dostępu.

Kryteria akceptacji:
- Na stronie edycji serwisanta dostępny jest przycisk "Resetuj hasło"
- Kliknięcie otwiera dialog potwierdzenia: "Czy na pewno resetować hasło dla [imię serwisanta]?"
- Po potwierdzeniu system generuje nowe tymczasowe hasło
- Email do serwisanta zawiera:
  - Nowe tymczasowe hasło
  - Instrukcja zmiany
  - Link do logowania
- Serwisant musi zmienić hasło przy następnym logowaniu
- Komunikat administratora: "Hasło zresetowane. Email wysłany do [email serwisanta]"
- Historia resetowania jest w audycie

---

#### US-035: Zarządzanie uprawnieniami - nadawanie roli Admin

Tytuł: Przydzielanie uprawnień administratora innym serwisantom

Opis: Jako Serwisant-Admin chcę nadawać oraz odbierać uprawnienia "Admin" innym serwisantom, aby centralnie zarządzać dostępem do konfiguracji organizacji.

Kryteria akceptacji:
- Na stronie edycji serwisanta dostępny jest toggle "Uprawnienia Administratora"
- Toggle przełącza pomiędzy "Serwisant" a "Serwisant-Admin"
- Zmiana roli wymaga potwierdzenia z komunikatem:
  - Jeśli nadawanie: "Nadaj uprawnienia administratora? [Imię] będzie miał dostęp do sekcji Administracja."
  - Jeśli odbieranie: "Odebrać uprawnienia administratora? [Imię] nie będzie miał dostępu do sekcji Administracja."
- Po potwierdzeniu rola zmienia się natychmiast
- Email do serwisanta zawiera informację o zmian roli
- Komunikat administratora zmieniającego: "Uprawnienia zaktualizowane"
- Historia zmian uprawnień jest w audycie
- Brak możliwości usunięcia ostatniego administratora

---

#### US-036: Dezaktywacja serwisanta

Tytuł: Usunięcie dostępu dla serwisanta

Opis: Jako Serwisant-Admin chcę usunąć (dezaktywować) serwisanta, aby odebrać mu dostęp do systemu.

Kryteria akceptacji:
- Na stronie edycji serwisanta dostępny jest przycisk "Dezaktywuj"
- Kliknięcie otwiera dialog: "Czy dezaktywować serwisanta [imię]? Uzyska wnik będzie mieć status 'Nieaktywny' i nie będzie mógł się zalogować."
- Jeśli serwisant ma przypisane tickety, pojawia się pytanie: "Co zrobić z przypisanymi ticketami?"
  - Opcja 1: "Przypisz do innego serwisanta" (lista serwisantów)
  - Opcja 2: "Pozostaw nieprzypisane"
- Po potwierdzeniu serwisant zmienia status na "Nieaktywny"
- Konto pozostaje w systemie (dla historii/audytu)
- Serwisant nie może się zalogować
- Historia dezaktywacji jest w audycie
- Przycisk "Aktywuj" może reaktywować konto
- Email do serwisanta nie jest wysyłany (nie w MVP)

---

### 5.9. Dashboard - Portal Klienta

#### US-037: Wyświetlenie kafelków z statusami maszyn i zgłoszeń

Tytuł: Widok główny statusów organizacji

Opis: Jako Operator/Administrator chcę zobaczyć na głównym ekranie kafelki z liczbą maszyn w statusach (Active, Alarm, Maintenance), liczbą moich aktywnych ticketów i wszystkich zgłoszeń organizacji.

Kryteria akceptacji:
- Dashboard wyświetla się po zalogowaniu
- Kafelek "Maszyny Aktywne": liczba maszyn w statusie Active (zielony)
- Kafelek "Alarmy": liczba maszyn w statusie Alarm (czerwony)
- Kafelek "Konserwacja": liczba maszyn w statusie Maintenance (żółty)
- Kafelek "Moje tickety": liczba aktywnych ticketów przypisanych do zalogowanego użytkownika
- Kafelek "Wszystkie zgłoszenia": liczba wszystkich otwartych ticketów organizacji
- Każdy kafelek zawiera ikonę odpowiadającą kategorii
- Liczby są duże i czytelne
- Kliknięcie kafelka otwiera filtrowaną listę (np. kafelek "Alarmy" otwiera listę maszyn z alarmem)
- Kafelki aktualizują się co 1 minutę

---

#### US-038: Wyświetlenie kafelka z ostatnimi aktywnościami

Tytuł: Sekcja ostatnich zdarzeń w organizacji

Opis: Jako Operator/Administrator chcę mieć na dashboardzie kafelek z ostatnimi aktywnościami organizacji/ticketów, aby widzieć, co wydarzyło się ostatnio.

Kryteria akceptacji:
- Kafelek "Ostatnie aktywności" wyświetla listę 5-10 ostatnich zdarzeń
- Każde zdarzenie zawiera: typ (ikona), opis, czas
- Przykłady zdarzeń:
  - "Nowy ticket #TICK-001234 utworzony przez [imię]"
  - "Status ticketu #TICK-001234 zmieniony na 'W trakcie'"
  - "Komentarz serwisu do ticketu #TICK-001234"
  - "Maszyna [model] zgłosiła alarm"
  - "[Imię] zalogował się"
- Linki do ticketów/maszyn
- Czasy względne: "2 minuty temu", "1 godzinę temu"
- Przycisk "Pokaż wszystkie" otwiera pełną historię zdarzeń
- Lista aktualizuje się co 30 sekund

---

#### US-039: Szybki dostęp do administracji zespołu

Tytuł: Kafelek administracji dla administratora organizacji

Opis: Jako Administrator chcę mieć kafelek/przycisk prowadzący do zakładki "Zespół", żebym mógł szybko zarządzać operatorami.

Kryteria akceptacji:
- Dashboard wyświetla kafelek "Administracja" (tylko dla Administratorów)
- Kafelek zawiera przycisk "Zarządzaj zespołem"
- Kliknięcie otwiera sekcję Administracja (tab "Zespół")
- Kafelek zawiera podsumowanie:
  - Liczba operatorów w organizacji
  - Liczba oczekujących aktywacji
  - Ostatnie zmiany w zespole (data)
- Ikona + otwiera dialog dodawania nowego operatora

---

### 5.10. Lista Zgłoszeń - Portal Klienta

#### US-040: Wyświetlenie listy wszystkich zgłoszeń organizacji

Tytuł: Przeglądanie zgłoszeń w portalu klienta

Opis: Jako Operator/Administrator chcę widzieć listę wszystkich zgłoszeń organizacji z widocznym statusem, priorytetem i maszyną, abym mógł szybko przejrzeć problemy.

Kryteria akceptacji:
- Po kliknięciu "Zgłoszenia" w menu pojawia się lista zgłoszeń
- Lista wyświetla tabelę/karty ze zgłoszeniami:
  - Numer zgłoszenia (link)
  - Tytuł
  - Status (ikona + tekst)
  - Priorytet (ikona + tekst)
  - Maszyna (link)
  - Data utworzenia
  - Liczba komentarzy
  - Przypisany serwisant (opcjonalnie)
- Domyślnie sortowana po dacie DESC
- Wyświetla wszystkie zgłoszenia, które może widzieć użytkownik
- Dla Operatora: widoczne jego zgłoszenia + publicze zgłoszenia całej organizacji
- Dla Administratora: widoczne wszystkie zgłoszenia organizacji
- Liczba zgłoszeń w nagłówku: "Wyświetlanie 1-20 z 45 zgłoszeń"
- Tabela jest responsywna

---

#### US-041: Filtrowanie zgłoszeń po statusie, maszynie i właścicielu

Tytuł: Zaawansowane filtrowanie zgłoszeń w portalu klienta

Opis: Jako Operator/Administrator chcę filtrować zgłoszenia według statusu, maszyny, właściciela, żebym szybciej znalazł interesujące mnie zgłoszenie.

Kryteria akceptacji:
- Panel filtrowania nad listą zgłoszeń
- Filtry dostępne:
  - Status: lista (Nowy, Przyjęty, W trakcie, Rozwiązany, Wznowiony, Zamknięty)
  - Maszyna: lista dostępnych maszyn (dynamiczna)
  - Właściciel: lista (Ja, [imię innego operatora]) - dla Admina
  - Data od/do (datepicker)
- Dla Operatora: filtr "Właściciel" może zawierać tylko "Ja"
- Dla Administratora: dostępni wszyscy operatorzy organizacji
- Przycisk "Szukaj" lub auto-update
- Przycisk "Resetuj"
- Liczba wyników: "Znaleziono 12 zgłoszeń"
- Wybrane filtry są zapamiętywane w URL

---

#### US-042: Tworzenie nowego zgłoszenia

Tytuł: Inicjowanie nowego ticketu

Opis: Jako Operator/Administrator chcę mieć przycisk "Utwórz nowe zgłoszenie", abym mógł łatwo rozpocząć nowe zgłoszenie.

Kryteria akceptacji:
- Na stronie listy zgłoszeń dostępny jest przycisk "+ Utwórz nowe zgłoszenie"
- Kliknięcie otwiera formularz tworzenia z polami:
  - Maszyna (obowiązkowe, lista maszyn organizacji)
  - Tytuł (obowiązkowe, min 10 znaków)
  - Opis problemu (obowiązkowe, min 20 znaków)
  - Priorytet (obowiązkowe, lista: Niski, Średni, Wysoki, Krytyczny)
  - Załączniki (opcjonalne, max 3 pliki po 10 MB)
  - Checkbox "Zgadzam się na przetwarzanie danych"
- Walidacja wszystkich pól
- Przycisk "Utwórz"
- System tworzy zgłoszenie w statusie "Nowy"
- Zgłoszenie otrzymuje numer TICK-{timestamp}
- Email potwierdzenia wysyłany do zalogowanego użytkownika (zawiera numer i link)
- Użytkownik jest przekierowywany do szczegółów nowo utworzonego zgłoszenia
- Komunikat: "Zgłoszenie #TICK-001234 utworzone"
- Status można zmienić na "Wysłany" poprzez przycisk "Wyślij do serwisu"

---

#### US-043: Porzucanie zgłoszeń w wersji roboczej

Tytuł: Usuwanie niewymaganego zgłoszenia

Opis: Jako Operator/Administrator chcę mieć możliwość "porzucenia" zgłoszenia w statusie "Szkic" lub "Wysłany", żebym nie musiał wysyłać nieaktualnych zgłoszeń.

Kryteria akceptacji:
- Na stronie szczegółów zgłoszenia w statusie "Nowy" (Szkic) lub "Wysłany" dostępny jest przycisk "Porzuć"
- Kliknięcie otwiera dialog: "Czy na pewno chcesz porzucić to zgłoszenie? Nie będzie ono widoczne w liście."
- Po potwierdzeniu zgłoszenie zmienia status na "Porzucone" lub "Usunięte"
- Zgłoszenie pozostaje w systemie (dla audytu), ale nie jest widoczne w normalnej liście
- Komunikat: "Zgłoszenie porzucone"
- Po porzuceniu nie da się go wznowić (dla operatora)
- Admin serwisu może wciąż zobaczyć porzucone zgłoszenia (z filtrem)

---

### 5.11. Szczegóły Zgłoszenia - Portal Klienta

#### US-044: Przeglądanie szczegółów zgłoszenia w formie osi czasu

Tytuł: Widok szczegółów zgłoszenia z podziałem na strony

Opis: Jako Operator/Administrator chcę widzieć szczegóły zgłoszenia w formie osi czasu, z przejrzystym podziałem na działania organizacji (lewa strona), działania serwisu (prawa strona), oraz zmiany statusów (środek).

Kryteria akceptacji:
- Po kliknięciu zgłoszenia z listy otwiera się strona szczegółów
- Nagłówek zawiera:
  - Numer zgłoszenia
  - Tytuł
  - Status (ikona + tekst)
  - Priorytet (ikona + tekst)
  - Data utworzenia i ostatniej aktualizacji
- Oś czasu wyświetla się głównie:
  - Kolumna lewa: "Organizacja" - działania użytkownika (komentarze, załączniki, aktualizacje)
  - Kolumna środkowa: "Statusy" - zmiany statusu ze strzałkami
  - Kolumna prawa: "Serwis" - działania zespołu serwisu (zmiana statusu, komentarze, przypisanie)
- Każde zdarzenie zawiera: avatar, imię, typ, opis, czas
- Kolory: lewy kolor A, środkowy szary, prawy kolor B (różne od lewego)
- Timeline jest łatwo czytane nawet na urządzeniach mobilnych
- Możliwość scrollowania w pionie (dodatkowych zdarzeń)

---

#### US-045: Dodawanie komentarzy do zgłoszenia

Tytuł: Komunikacja z serwisem poprzez komentarze

Opis: Jako Operator chcę móc dodawać komentarz, załączniki lub aktualizować opis tylko w swoich ticketach, aby precyzyjnie komunikować się z serwisem.

Kryteria akceptacji:
- Na stronie szczegółów zgłoszenia znajduje się sekcja "Komentarze"
- Sekcja zawiera listę wszystkich komentarzy (od organizacji i serwisu)
- Poniżej jest pole do wpisania nowego komentarza
- Dla Operatora: może komentować tylko свои tickety
- Dla Administratora: może komentować wszystkie tickety organizacji
- Przycisk "Wyślij komentarz"
- Po wysłaniu komentarz pojawia się na timeline
- Komentarz zawiera: avatar, imię, treść, czas, możliwość edycji (przez autora, do 1h)
- Komentarze serwisu są wyróżnione (inny kolor tła)
- Komentarze zawierają formatowanie: bold, italic, linki

---

#### US-046: Dodawanie załączników do zgłoszenia

Tytuł: Dołączanie plików do komunikacji

Opis: Jako Operator/Administrator chcę w dowolnym momencie dodać do zgłoszenia nowy załącznik, żeby wzbogacić zgłoszenie o dodatkową dokumentację.

Kryteria akceptacji:
- Na stronie szczegółów zgłoszenia dostępny jest przycisk "Dodaj załącznik" lub drag-and-drop zone
- Kliknięcie otwiera dialog wyboru pliku
- Obsługiwane typy: JPG, PNG, PDF, TXT, DOC, DOCX, XLS, XLSX, ZIP, MP4
- Max rozmiar: 10 MB na plik
- Max liczba plików do dodania: 3 na raz
- Po wybraniu pliku wyświetla się progress bar
- Po załadowaniu plik pojawia się na timeline w sekcji "Załączniki"
- Załącznik zawiera: miniaturę, nazwę, rozmiar, opcję pobrania
- Dla Operatora: może dodawać załączniki tylko do swoich ticketów
- Dla Administratora: może dodawać załączniki do wszystkich ticketów
- Komunikat: "Załącznik dodany"

---

#### US-047: Wznowienie rozwiązanego zgłoszenia

Tytuł: Reaktywowanie ticketu w ciągu 14 dni od rozwiązania

Opis: Jako Administrator chcę widzieć przycisk "Wznów ticket" na zgłoszeniu, które jest Rozwiązane mniej niż 14 dni, aby ponownie uruchomić proces obsługi, jeśli problem nie został rozwiązany.

Kryteria akceptacji:
- Zgłoszenie w statusie "Rozwiązany" wyświetla przycisk "Wznów" (widoczny przez 14 dni)
- Przycisk jest widoczny w nagłówku lub na timeline
- Kliknięcie otwiera dialog: "Czy na pewno wznowić to zgłoszenie? Status zmieni się na 'Wznowiony'."
- Pole do wpisania przyczyny wznowienia (obowiązkowe)
- Po potwierdzeniu status zmienia się na "Wznowiony"
- Przyczyna wznowienia pojawia się na timeline
- Zespół serwisu otrzymuje notyfikację (w systemie)
- Po 14 dniach od rozwiązania przycisk "Wznów" znika
- Po wznowieniu ticketu przycisk "Wznów" znika

---

#### US-048: Aktualizacja opisu zgłoszenia

Tytuł: Edycja zawartości zgłoszenia

Opis: Jako Operator/Administrator chcę móc aktualizować opis zgłoszenia, aby dodawać nowe informacje w miarę ich pojawiania się.

Kryteria akceptacji:
- W sekcji szczegółów zgłoszenia (tytuł, opis) dostępny jest przycisk "Edytuj"
- Dla Operatora: może edytować tylko swoje zgłoszenia
- Dla Administratora: może edytować wszystkie zgłoszenia organizacji
- Kliknięcie otwiera formularz edycji:
  - Tytuł (edytowalny)
  - Opis (edytowalny, rich text editor)
  - Przycisk "Zapisz"
  - Przycisk "Anuluj"
- Po zapisaniu zmiany są natychmiast widoczne
- Historia edycji jest dostępna (kto, kiedy zmienił)
- Wiadomość w timeline: "[Imię] zaktualizował opis"
- Starszy opis pozostaje widoczny w historii

---

### 5.12. Zarządzanie Zespołem - Portal Klienta

#### US-049: Wyświetlenie listy operatorów organizacji

Tytuł: Przeglądanie zespołu w portalu klienta

Opis: Jako Administrator chcę widzieć listę wszystkich użytkowników w mojej organizacji z imieniem, emailem, ostatnią aktywnością i liczbą utworzonych ticketów, by mieć kontrolę nad zespołem.

Kryteria akceptacji:
- Na stronie "Administracja" > "Zespół" wyświetla się tabela z listą operatorów
- Każda pozycja zawiera:
  - Imię i nazwisko
  - Email
  - Rola (Operator, Administrator)
  - Status konta (Aktywny, Oczekuje aktywacji, Nieaktywny)
  - Ostatnia aktywność (data, czas lub "nigdy")
  - Liczba utworzonych ticketów
  - Przycisk "Edytuj"
  - Przycisk "Usuń"
- Użytkownicy oczekujący aktywacji są wyróżnieni (żółty)
- Nieaktywni użytkownicy są wyróżnieni (szary)
- Paginacja dla dużych zespołów
- Możliwość sortowania po kolumnach

---

#### US-050: Dodawanie nowego operatora przez email

Tytuł: Zaproszenie nowego pracownika do organizacji

Opis: Jako Administrator chcę mieć możliwość dodania nowego operatora przez e-mail oraz zobaczyć status aktywacji (oczekuje na aktywację/aktywny), żebym zwiększał skład organizacji.

Kryteria akceptacji:
- Na stronie "Zespół" dostępny jest przycisk "+ Dodaj operatora"
- Kliknięcie otwiera formularz:
  - Email (obowiązkowe, unikat)
  - Imię (obowiązkowe)
  - Nazwisko (obowiązkowe)
  - Rola (lista: Operator, Administrator)
- Po wypełnieniu przycisk "Wyślij zaproszenie"
- System wysyła email na podany adres zawierający:
  - Link aktywacyjny
  - Instrukcje logowania
  - Dane kontaktowe administratora organizacji
- Nowy operator pojawia się na liście ze statusem "Oczekuje aktywacji"
- Zaproszenie jest ważne 7 dni
- Komunikat: "Zaproszenie wysłane do [email]"
- Po kliknięciu linku aktywacyjnego operator ustawia hasło i активuje konto

---

#### US-051: Dezaktywacja operatora

Tytuł: Usunięcie dostępu operatora

Opis: Jako Administrator chcę móc usunąć (dezaktywować) operatora, by odebrać mu dostęp do systemu.

Kryteria akceptacji:
- Na stronie "Zespół" kliknięcie "Usuń" na operatorze otwiera dialog:
  - "Czy na pewno usunąć [imię] z organizacji?"
  - "Uzyska wnik będzie miał dostęp zablokowany do systemu."
  - Pole informacyjne: "Jego zgłoszenia pozostaną w systemie"
- Po potwierdzeniu operator zmienia status na "Nieaktywny"
- Operator nie może się zalogować
- Historia operatora pozostaje (dla audytu)
- Email do operatora nie jest wysyłany (nie w MVP)
- Komunikat administratora: "Operator usunięty z organizacji"

---

### 5.13. Uwierzytelnianie i Autoryzacja

#### US-052: Bezpieczna obsługa sesji użytkownika

Tytuł: Zarządzanie sesją zalogowanego użytkownika

Opis: Jako użytkownik systemu chcę mieć bezpieczną sesję z automatycznym timeout'iem, aby moje konto było chronione przed nieautoryzowanym dostępem.

Kryteria akceptacji:
- Sesja użytkownika jest utrzymywana na max 30 minut inaktywności (Portal Serwisu)
- Sesja użytkownika jest utrzymywana na max 60 minut inaktywności (Portal Klienta)
- Po timeout'ie system automatycznie wylogowuje użytkownika
- Komunikat: "Sesja wygasła. Zaloguj się ponownie."
- Przycisk "Zaloguj się" otwiera ekran logowania
- Dane sesji nie są tracone - jeśli to możliwe, formularz może być zapamiętany
- Możliwość manualnego wylogowania poprzez przycisk "Wyloguj"
- Cookie sesji zawierają flagę HttpOnly i Secure
- Przy logowaniu z innego urządzenia poprzednia sesja jest wylogowywana (opcjonalnie)

---

#### US-053: Kontrola dostępu na bazie ról

Tytuł: Implementacja RBAC - Role-Based Access Control

Opis: Jako administrator systemu chcę, aby każdy użytkownik miał dostęp tylko do funkcji przypisanych jego roli, aby zapewnić bezpieczeństwo i separację odpowiedzialności.

Kryteria akceptacji:
- System implementuje Role-Based Access Control (RBAC)
- Роле dostępne:
  - Serwisant: dostęp do Dashboard, Zgłoszeń, Organizacji
  - Serwisant-Admin: dostęp do Dashboard, Zgłoszeń, Organizacji, Administracji
  - Operator (klienta): dostęp do Dashboard, Zgłoszeń, własnych ticketów
  - Administrator (klienta): dostęp do Dashboard, Zgłoszeń, Administracji (Zespół)
- Menu nawigacyjne zmienia się w zależności od roli
- Niedostępne funkcje są ukryte (niewidoczne/wyszarzone)
- Próba bezpośredniego dostępu do niedostępnego URL zwraca błąd 403
- Każda akcja wymagająca uprawnień jest sprawdzana na backendu
- Role są przechowywane w JWT token (bezpiecznie)
- Historia zmian ról jest rejestrowana w audycie

---

#### US-054: Audyt bezpieczeństwa i logowanie akcji użytkownika

Tytuł: Rejestrowanie wszystkich działań użytkowników

Opis: Jako administrator serwisu chcę mieć pełny audyt wszystkich akcji użytkowników (logowanie, tworzenie, edycja, usuwanie), aby móc śledzić i badać bezpieczeństwo systemu.

Kryteria akceptacji:
- System rejestruje wszystkie akcje użytkowników w bazie danych
- Logio akcje obejmują:
  - Logowanie (udane i nieudane)
  - Wylogowanie
  - Tworzenie zgłoszenia
  - Edycja zgłoszenia
  - Zmiana statusu
  - Dodanie komentarza
  - Dodanie załącznika
  - Zmiana roli/uprawnień
  - Edycja organizacji
  - Edycja serwisanta
- Każdy log zawiera:
  - ID akcji (unique)
  - Użytkownik (ID, login, imię)
  - Typ akcji
  - Zasób (np. ticket ID, user ID)
  - Stara wartość (jeśli edycja)
  - Nowa wartość (jeśli edycja)
  - IP adres
  - User agent (browser)
  - Timestamp (dokładnie do sekund)
- Logi nie mogą być edytowane lub usuwane (append-only)
- Dostęp do dziennika audytu ma wyłącznie Serwisant-Admin
- Możliwość filtrowania logów po: użytkowniku, typie akcji, dacie, zasobie
- Eksport logów w formacie CSV/JSON

---

---

## 6. Metryki Sukcesu

### Szybkość

1. Czas utworzenia nowego zgłoszenia: max 2 minuty
   - Operator otwiera formularz, wpisuje dane, załącza plik, wysyła
   - Metrika: średni czas od kliknięcia "Utwórz" do kliknięcia "Wyślij"
   - Target: < 2 min, Acceptor: < 3 min

2. Czas znalezienia swoich zgłoszeń przez serwisanta: max 3 klikniecia
   - Zalogowanie → Dashboard → Kliknięcie "Moje tickety" → Lista
   - Metrika: liczba kliknięć do osiągnięcia filtrowanej listy
   - Target: ≤ 3 klikniecia

3. Zmiana statusu zgłoszenia widoczna natychmiast: < 1 sekunda
   - Po zatwierdzeniu zmiany statusu powinien być widoczny natychmiast na stronie i na timeline
   - Metrika: czas od kliknięcia potwierdzenia do zmian w UI
   - Target: < 1 s

4. Logi maszyn pojawia się w systemie: < 1 minuta od wysłania
   - Maszyna wysyła log → Log pojawia się w systemie → Widoczny na dashboardzie/szczegółach
   - Metrika: czas od wysłania logu przez maszynę do pojawienia się w UI
   - Target: < 1 min

### Bezpieczeństwo

1. Wszystkie dane użytkowników są zaszyfrowane:
   - Dane przechowywane w bazie: hasła haszowane (bcrypt+salt), dane osobowe szyfrowane
   - Dane w transporcie: HTTPS/TLS 1.2+
   - Metrika: brak niezaszyfrowanych haseł w bazie
   - Target: 100%

2. Każda zmiana w zgłoszeniu jest rejestrowana:
   - Wszystkie akcje (zmiana statusu, komentarz, załącznik, przypisanie) są logowane
   - Pełna historia audytu dostępna dla admina
   - Metrika: liczba zarejestrowanych akcji vs liczba wykonanych akcji
   - Target: 100% (każda akcja w audycie)

3. Dostęp zabezpieczony logowaniem i uprawnieniami:
   - Brak dostępu bez logowania
   - Role kontrolują dostęp do funkcji
   - Metrika: liczba niezalogowanych przejść na chronione strony
   - Target: 0

### Integracja Logów

1. Logi z maszyn automatycznie importowane:
   - Maszyna wysyła log z prawidłowym tokenem API
   - Log pojawia się w systemie
   - Metrika: procent poprawnie zaladowanych logów
   - Target: > 99%

2. Możliwość tworzenia zgłoszeń na bazie logów:
   - Administrator/serwisant widzi logi maszyn
   - Może wybrać log i stworzyć na jego bazie zgłoszenie
   - Metrika: dostępność opcji tworzenia zgłoszenia z logu
   - Target: dostępna dla 100% logów

### Jakość

1. Interfejs łatwy w obsłudze:
   - System Usability Scale (SUS) score > 75
   - Wytested z minimum 5 użytkownikami każdej roli
   - Metrika: wynik SUS, feedback użytkowników
   - Target: > 75 SUS score

2. Responsywny projekt na telefonach i komputerach:
   - Aplikacja działa na urządzeniach mobiles (320px - 768px)
   - Aplikacja działa na desktopach (1024px+)
   - Metrika: testy responsywności, brak błędów w UI
   - Target: brak kritycznych błędów na żadnej rozdzielczości

3. Wszystkie funkcje pracują bez błędów uniemożliwiających pracy:
   - Podczas load testing: 100 concurrent users
   - Szybkość response time < 2 sekund
   - Dostępność > 99.5%
   - Metrika: uptime, response time, error rate
   - Target: < 0.5% error rate, < 2s avg response, > 99.5% uptime

---

Koniec dokumentu wymagań produktu FLOWerTRACK
