import { Navbar } from '../components/landing/Navbar';
import { HeroSection } from '../components/landing/HeroSection';
import { FeatureCard } from '../components/landing/FeatureCard';
import { BenefitItem } from '../components/landing/BenefitItem';
import { CTASection } from '../components/landing/CTASection';
import { Footer } from '../components/landing/Footer';
import '../styles/landing.css';

const ServiceLanding = () => {
  const heroData = {
    badge: '🚀 Nowa generacja zarządzania serwisem',
    title: 'Centralne zarządzanie zgłoszeniami ',
    gradientWord: 'serwisowymi',
    subtitle:
      'Kompleksowa platforma dla zespołów serwisowych. Skróć czas reakcji, zwiększ transparentność i zautomatyzuj workflow zgłoszeń.',
    primaryCTA: {
      text: 'Rozpocznij bezpłatnie',
      icon: '→',
    },
    secondaryCTA: {
      text: 'Zobacz demo',
      icon: '▶',
    },
    stats: [
      { value: '100+', label: 'Obsłużonych zgłoszeń' },
      { value: '-60%', label: 'Czasu reakcji' },
      { value: '99.9%', label: 'Dostępności systemu' },
    ],
    floatingIcons: ['🔧', '⚙️', '🔩', '🛠️'],
  };

  const features = [
    {
      icon: '🎯',
      title: 'Dashboard KPI',
      description:
        'Kafelki ze statusami zgłoszeń, wykresy trendów i rozkład po priorytetach. Wszystkie kluczowe metryki w jednym miejscu.',
      features: [
        'Kafelki z liczbą zgłoszeń',
        'Wykresy trendów (30/90 dni)',
        'Filtrowanie po priorytetach',
      ],
    },
    {
      icon: '📋',
      title: 'Zarządzanie Zgłoszeniami',
      description:
        'Kompleksowy widok listy z zaawansowanym filtrowaniem, szczegóły ticketu z timeline i masowe akcje.',
      features: [
        'Tabela z sortowaniem i paginacją',
        'Timeline zmian statusu',
        'Masowe przypisywanie i zmiana statusu',
      ],
    },
    {
      icon: '👥',
      title: 'Przydzielanie Zadań',
      description:
        'Szybkie przypisywanie zgłoszeń do serwisantów, śledzenie obciążenia zespołu i workflow statusów.',
      features: [
        'Przypisanie do siebie lub innego',
        'Workflow: Draft → Wysłany → W trakcie → Rozwiązany',
        'Monitoring obciążenia zespołu',
      ],
    },
    {
      icon: '🏢',
      title: 'Moduł Organizacji',
      description:
        'Lista wszystkich klientów z danymi kontaktowymi, maszynami i możliwością onboardingu nowych organizacji.',
      features: [
        'Lista organizacji z statusami',
        'Zarządzanie maszynami klientów',
        'Generowanie tokenów API',
      ],
    },
    {
      icon: '⚙️',
      title: 'Administracja Zespołu',
      description: 'Zarządzanie serwisantami, nadawanie uprawnień i monitoring aktywności zespołu.',
      features: [
        'Dodawanie/edycja serwisantów',
        'Role i uprawnienia (Admin/Technician)',
        'Resetowanie haseł',
      ],
    },
    {
      icon: '📊',
      title: 'Integracja Logów',
      description:
        'Automatyczne zbieranie logów z maszyn produkcyjnych, historia problemów i tworzenie zgłoszeń z logów.',
      features: [
        'Automatyczne zbieranie danych',
        'Przeglądanie raw data',
        'Tworzenie ticketów z logów',
      ],
    },
  ];

  const benefits = [
    {
      icon: '✅',
      title: 'Pełna widoczność pracy zespołu',
      description:
        'Menedżerowie mają jasny wgląd w to, nad czym pracują serwisanci i jakie zadania czekają na przydzielenie.',
    },
    {
      icon: '✅',
      title: 'Centralizacja informacji',
      description:
        'Wszystkie zgłoszenia z różnych źródeł (email, telefon, HMI, logi maszyn) w jednym miejscu. Koniec z chaosem.',
    },
    {
      icon: '✅',
      title: 'Szybsza reakcja na problemy',
      description:
        'Automatyczne zbieranie logów z maszyn i natychmiastowe powiadamianie zespołu o nowych zgłoszeniach.',
    },
    {
      icon: '✅',
      title: 'Pełny audyt zmian',
      description:
        'Każda akcja (zmiana statusu, komentarz, przypisanie) jest logowana z informacją kto, co i kiedy zmienił.',
    },
  ];

  return (
    <div className="landing-page">
      <Navbar variant="service" />

      <HeroSection {...heroData} />

      {/* Features Section */}
      <section id="features" className="features">
        <div className="section-header">
          <span className="section-badge">Funkcjonalności</span>
          <h2 className="section-title">
            Wszystko czego potrzebujesz
            <br />w jednym miejscu
          </h2>
        </div>
        <div className="features-grid">
          {features.map((feature, index) => (
            <FeatureCard key={index} {...feature} />
          ))}
        </div>
      </section>

      {/* Benefits Section */}
      <section id="benefits" className="benefits">
        <div className="benefits-content">
          <div className="benefits-text">
            <span className="section-badge">Korzyści</span>
            <h2 className="section-title">Dlaczego FLOWerTRACK dla Twojego Serwisu?</h2>
            <div className="benefits-list">
              {benefits.map((benefit, index) => (
                <BenefitItem key={index} {...benefit} />
              ))}
            </div>
          </div>
          <div className="benefits-visual">
            <div className="dashboard-preview">
              <div className="preview-header">
                <div className="preview-dots">
                  <span></span>
                  <span></span>
                  <span></span>
                </div>
                <span className="preview-title">FLOWerTRACK Dashboard</span>
              </div>
              <div className="preview-content">
                <div className="preview-card">📊 Aktywne zgłoszenia: 24</div>
                <div className="preview-card">⚠️ Krytyczne: 3</div>
                <div className="preview-card">✅ Rozwiązane: 156</div>
                <div className="preview-chart"></div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <CTASection
        headline="Gotowy na optymalizację serwisu?"
        subheadline="Dołącz do firm, które już skracają czas reakcji z FLOWerTRACK"
        primaryButtonText="Rozpocznij za darmo"
        secondaryButtonText="Skontaktuj się z nami"
      />

      <Footer variant="service" />
    </div>
  );
};

export default ServiceLanding;
