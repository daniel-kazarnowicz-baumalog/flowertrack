import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Navbar,
  HeroSection,
  FeatureCard,
  BenefitItem,
  CTASection,
  Footer,
} from '../../components/landing';
import './ServiceLanding.css';

export const ServiceLanding: React.FC = () => {
  const navigate = useNavigate();

  const heroStats = [
    { value: '100+', label: 'Obsłużonych zgłoszeń' },
    { value: '-60%', label: 'Czasu reakcji' },
    { value: '99.9%', label: 'Dostępności systemu' },
  ];

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
      description:
        'Zarządzanie serwisantami, nadawanie uprawnień i monitoring aktywności zespołu.',
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

  const ctaButtons = [
    {
      text: 'Rozpocznij za darmo',
      variant: 'primary' as const,
      onClick: () => navigate('/service/login'),
    },
    {
      text: 'Skontaktuj się z nami',
      variant: 'outline' as const,
    },
  ];

  return (
    <div className="landing-page">
      <Navbar variant="service" />

      <HeroSection
        badge="🚀 Nowa generacja zarządzania serwisem"
        title="Centralne zarządzanie zgłoszeniami serwisowymi"
        highlightedWord="serwisowymi"
        subtitle="Kompleksowa platforma dla zespołów serwisowych. Skróć czas reakcji, zwiększ transparentność i zautomatyzuj workflow zgłoszeń."
        primaryCTA={{
          text: 'Rozpocznij bezpłatnie',
          icon: '→',
          onClick: () => navigate('/service/login'),
        }}
        secondaryCTA={{
          text: 'Zobacz demo',
          icon: '▶',
        }}
        stats={heroStats}
        backgroundIcons={['🔧', '⚙️', '🔩', '🛠️']}
      />

      <section className="features-section" id="features">
        <div className="features-container">
          <div className="features-header">
            <div className="features-badge">Funkcjonalności</div>
            <h2 className="features-title">
              Wszystko czego potrzebujesz w jednym miejscu
            </h2>
          </div>
          <div className="features-grid">
            {features.map((feature, index) => (
              <FeatureCard key={index} {...feature} />
            ))}
          </div>
        </div>
      </section>

      <section className="benefits-section" id="benefits">
        <div className="benefits-container">
          <div className="benefits-content">
            <h2 className="benefits-title">
              Dlaczego FLOWerTRACK dla Twojego Serwisu?
            </h2>
            <div className="benefits-list">
              {benefits.map((benefit, index) => (
                <BenefitItem key={index} {...benefit} />
              ))}
            </div>
          </div>
          <div className="benefits-visual">
            <div className="dashboard-preview">
              <div className="dashboard-card">
                <div className="dashboard-icon">📊</div>
                <div className="dashboard-label">Aktywne zgłoszenia</div>
                <div className="dashboard-value">24</div>
              </div>
              <div className="dashboard-card">
                <div className="dashboard-icon">⚠️</div>
                <div className="dashboard-label">Krytyczne</div>
                <div className="dashboard-value">3</div>
              </div>
              <div className="dashboard-card">
                <div className="dashboard-icon">✅</div>
                <div className="dashboard-label">Rozwiązane</div>
                <div className="dashboard-value">156</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <CTASection
        headline="Gotowy na optymalizację serwisu?"
        subheadline="Dołącz do firm, które już skracają czas reakcji z FLOWerTRACK"
        buttons={ctaButtons}
      />

      <Footer />
    </div>
  );
};
