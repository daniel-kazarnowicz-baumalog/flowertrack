## Struktura Funkcjonalna Serwisu

Poniższy plik Markdown przedstawia strukturę funkcjonalną serwisu, bazując na diagramie przepływu oraz szczegółowych opisach funkcjonalności i wymagań użytkownika (Use Cases) dla roli **Serwisanta** i **Serwisanta-Administratora**.

---

### 1. Moduł Logowania (Logowanie)

Moduł ten służy do uwierzytelniania użytkowników serwisu.

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Ekran Logowania** | Minimalistyczny ekran dostępowy. Użytkownik wprowadza login i hasło. Po weryfikacji następuje przekierowanie do Ekranu Głównego. | **Jako Serwisant** chcę zalogować się na dedykowany portal serwisowy, aby uzyskać dostęp do panelu głównego i moich zadań. |
| **Funkcje Dodatkowe** | Opcje: "Zapomniałem hasła" oraz wybór języka przed zalogowaniem. | (Brak dedykowanych Use Case'ów w tekście) |

---

### 2. Moduł Główny (Ekran główny / Dashboard)

Dynamiczny panel prezentujący kluczowe informacje, wskaźniki KPI oraz szybkie linki nawigacyjne.

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Wskaźniki KPI/Kafelki** | Liczba aktywnych zgłoszeń (podział na priorytety i statusy), historia zgłoszeń (rozwiązane, zamknięte), alerty i statusy organizacji. | **Jako Serwisant** chcę widzieć liczbę aktywnych zgłoszeń i alarmów krytycznych na kafelkach, aby natychmiast rozpoznać najpilniejsze zadania. |
| **Wykresy i Trendy** | Wykres trendów zgłoszeń, rozkład po priorytetach. | **Jako Serwisant** chcę mieć wykres trendów zgłoszeń i rozkład po priorytetach, aby monitorować obciążenie serwisu i strukturę problemów. |
| **Powiadomienia i Aktywność** | Lista ostatnich zdarzeń, powiadomienia push/mail o nowych zgłoszeniach, szybkie linki do "Moje tickety", "Organizacje z alarmami" i "Nadchodzące przeglądy". | **Jako Serwisant** chcę widzieć listę ostatnich zdarzeń oraz moich ticketów wymagających akcji, aby szybko przejść do działań operacyjnych. |
| **Nawigacja** | Menu nawigacyjne z zakładkami: Dashboard, Zgłoszenia, Organizacje, Administracja (widoczna tylko dla administratorów). Opcja zmiany języka i wylogowania. | **Jako Serwisant** chcę mieć menu z zakładkami nawigacyjnymi, opcję zmiany języka i przycisk Wyloguj, aby wygodnie przełączać się między sekcjami. |
| **Akcje Szybkie** | Szybkie przejście do list szczegółowych. | **Jako Serwisant** gdy kliknę kafelek „Moje tickety”, wtedy powinienem przejść do listy ticketów przefiltrowanej po moim loginie. |

---

### 3. Moduł Zgłoszeń (Zgłoszenia)

Centralne miejsce zarządzania wszystkimi ticketami serwisowymi.

#### 3.1. Widok Listy Zgłoszeń (Widok wszystkich zgłoszeń z filtrowaniem)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Tabela Zgłoszeń** | Tabela lub kafelki ze skróconym opisem, statusem, priorytetem i akcją przejścia do szczegółów. | **Jako Serwisant** chcę grupować tickety według statusu (Aktywne, Rozwiązane, Zamknięte), aby mieć logiczny podział spraw i workflow. |
| **Filtrowanie i Wyszukiwanie** | Możliwość filtrowania po statusie, priorytecie, organizacji, maszynie, przypisanym serwisancie, dacie utworzenia. Wyszukiwarka po tytule i numerze. | **Jako Serwisant** chcę filtrować tickety po organizacji, maszynie, statusie, priorytecie i sobie, aby sprawnie zarządzać zgłoszeniami. **Jako Serwisant** chcę szybciej odnajdywać zgłoszenia dzięki wyszukiwarce po tytule i numerze, aby nie tracić czasu na przeglądanie długiej listy. |
| **Masowe Akcje** | Przypisanie, zmiana statusu, archiwizacja. | (Brak dedykowanych Use Case'ów) |
| **Historia/Trendy** | Dodatkowy podgląd historii zgłoszeń z analizą trendów rozwiązania ticketów. | (Brak dedykowanych Use Case'ów) |

#### 3.2. Obsługa Pojedynczego Zgłoszenia (Widok pojedynczego zgłoszenia)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Kontekst Ticketu** | Prezentacja wszystkich istotnych danych o zgłoszeniu, maszynie i organizacji, w tym `raw data` z maszyny. | **Jako Serwisant** chcę zobaczyć pełne szczegóły ticketa – dane zgłoszenia, maszynę, załączniki, priorytet, organizację, status, aby mieć natychmiast potrzebny kontekst. |
| **Oś Czasu (Timeline)** | Pełna oś czasu zmian statusu, przypisań, historii chatów, załączników, logów. Pełny audyt wszystkich akcji. | **Jako Serwisant** chcę widzieć całą oś czasu zgłoszenia w formie łatwego do prześledzenia timeline (zdarzenia, statusy, wiadomości, załączniki), aby lepiej analizować przebieg obsługi. |
| **Workflow Statusów** | Pełny workflow statusów: Draft → Wysłany → Przyjęty → W trakcie → Rozwiązany → Wznowiony → Zamknięty. | **Jako Serwisant** chcę zmieniać status ticketa na: Przyjęty, W trakcie, Rozwiązany, Zamknięty, aby odzwierciedlić cykl serwisowy. |
| **Działania Serwisanta** | Zmiana statusu, dodanie notatki (wewnętrznej - niewidocznej dla klienta), przypisanie do siebie/innego serwisanta, komunikacja (chat), dodanie uzasadnienia do istotnych zmian. | **Jako Serwisant** chcę przeglądać i dodawać historię zmian statusów, przypisań oraz wewnętrzne notatki, aby transparentnie dokumentować postęp i decyzje. **Jako Serwisant** chcę przypisać ticket do siebie lub innego serwisanta, aby jasno określić odpowiedzialność. **Jako Serwisant** chcę komunikować się w tickecie przez chat z operatorem/klientem, aby wyjaśniać szczegóły i prosić o dodatkowe informacje. **Jako Serwisant** chcę dodać uzasadnienie do istotnych zmian (np. zamknięcia, odrzucenia wznowienia), aby każda operacja była udokumentowana. |
| **Eksport** | Możliwość eksportowania historii ticketu. | (Brak dedykowanych Use Case'ów) |

---

### 4. Moduł Organizacji (Organizacje)

Moduł do zarządzania danymi klientów, ich maszynami oraz procesem onboardingu.

#### 4.1. Zarządzanie Organizacjami (Zarządzanie organizacjami)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Lista Organizacji** | Lista klientów, zawierająca podstawowe dane kontaktowe, status umowy/serwisu, liczbę maszyn, stan alarmów i aktywnych ticketów. | **Jako Serwisant(-Admin)** chcę widzieć listę organizacji, liczbę aktywnych maszyn/ticketów, statusy umów/przeglądów, aby identyfikować “gorące” punkty wymagające reakcji. |
| **Onboarding** | Inicjowanie onboardingu nowych klientów poprzez wysyłkę zaproszenia na e-mail administratora klienta. | **Jako Serwisant-Admin** chcę tworzyć i zapraszać nowe organizacje (email do admina klienta), aby onboardingować nowych klientów. |
| **Edycja Danych** | Edycja danych kontaktowych organizacji (tylko dla administratora serwisu). | **Jako Serwisant-Admin** chcę edytować dane kontaktowe organizacji (nazwa, adres, telefon, osoba), aby móc aktualizować niezbędne informacje. |

#### 4.2. Szczegóły Organizacji (Szczegóły organizacji)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Zarządzanie Maszynami** | Przypisywanie/edycja maszyn, generowanie tokenu API. | **Jako Serwisant(-Admin)** chcę przypisywać i edytować maszyny dla organizacji, aby odwzorować stan faktyczny parku maszynowego klienta. |
| **Status Maszyn** | Przegląd maszyn, ich statusów, widok live danych i historycznych alarmów. | (Brak dedykowanych Use Case'ów) |
| **Przeglądy** | Przechowywanie dat ostatnich przeglądów i konfigurowalnych interwałów. | (Brak dedykowanych Use Case'ów) |

---

### 5. Moduł Administracji (Administracja)

Moduł dostępny **tylko dla Serwisantów-Administratorów** do zarządzania zespołem i uprawnieniami.

#### 5.1. Zarządzanie Serwisantami (Administracja)

| Element | Opis Funkcjonalności | Wymagania Użytkownika (Use Case) |
| :--- | :--- | :--- |
| **Lista Serwisantów** | Lista wszystkich serwisantów, ich rola, status konta oraz indykatory ostatniej aktywności. | **Jako Serwisant-Admin** chcę mieć listę wszystkich serwisantów wraz z indikatorami aktywności, aby w razie potrzeby móc przekierowywać sprawy i monitorować obciążenie zespołu. |
| **Zarządzanie Kontami** | Dodawanie nowego loginu, resetowanie/zmiana hasła, edycja konta. | **Jako Serwisant-Admin** chcę zarządzać loginami serwisantów – dodawać, edytować, resetować hasła, aby zapewnić bezpieczeństwo i płynność pracy zespołu. |
| **Uprawnienia** | Nadawanie/odbieranie uprawnień "administrator" dla dostępu do konfiguracji organizacji i sekcji Administracja. | **Jako Serwisant-Admin** chcę nadawać oraz odbierać uprawnienia “Admin” innym serwisantom, aby centralnie zarządzać dostępem do konfiguracji organizacji. |
| **Monitorowanie** | Monitorowanie obciążenia zespołu i centralne zarządzanie dostępem. (Opcjonalnie: statystyki efektywności serwisantów). | (Wymagania pokryte przez listę serwisantów) |

---