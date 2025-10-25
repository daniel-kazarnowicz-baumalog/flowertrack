import { Navbar } from '../components/landing/Navbar';
import { HeroSection } from '../components/landing/HeroSection';
import { FeatureCard } from '../components/landing/FeatureCard';
import { HowItWorksStep } from '../components/landing/HowItWorksStep';
import { BenefitItem } from '../components/landing/BenefitItem';
import { CTASection } from '../components/landing/CTASection';
import { Footer } from '../components/landing/Footer';
import '../styles/landing.css';

const ClientLanding = () => {
  const heroData = {
    badge: '🎯 Pełna kontrola w Twoich rękach',
    title: 'Śledź swoje maszyny i zgłoszenia w ',
    gradientWord: 'jednym miejscu',
    subtitle:
      'Dedykowany portal dla operatorów i administratorów organizacji. Twórz zgłoszenia, śledź statusy maszyn i zarządzaj zespołem bez wychodzenia z aplikacji.',
    primaryCTA: {
      text: 'Aktywuj konto',
      icon: '→',
    },
    secondaryCTA: {
      text: 'Dowiedz się więcej',
      icon: '📖',
    },
    stats: [
      { value: '24/7', label: 'Dostęp do statusów' },
      { value: '2 min', label: 'Utworzenie zgłoszenia' },
      { value: '100%', label: 'Transparentność procesu' },
    ],
    floatingIcons: ['🏭', '🖥️', '📱', '✉️'],
  };

  const features = [
    {
      icon: '📊',
      title: 'Dashboard Organizacji',
      description:
        'Przegląd statusów wszystkich Twoich maszyn, aktywnych zgłoszeń i ostatnich aktywności w jednym miejscu.',
      features: [
        'Kafelki statusów maszyn (Active, Alarm, Maintenance)',
        'Kafelki statusów zgłoszeń',
        'Ostatnie aktywności',
      ],
    },
    {
      icon: '✍️',
      title: 'Tworzenie Zgłoszeń',
      description:
        'Prosty formularz do zgłaszania problemów. Dodaj opis, załącz zdjęcia i śledź postęp naprawy.',
      features: ['Wybór maszyny z listy', 'Załączniki i zdjęcia', 'Status: Szkic → Wysłany'],
    },
    {
      icon: '👁️',
      title: 'Śledzenie Statusu',
      description:
        'Timeline z pełną historią zgłoszenia. Zobacz wszystkie zmiany statusu, komentarze serwisu i załączniki.',
      features: [
        'Oś czasu zmian (działania organizacji + serwisu)',
        'Komunikacja z serwisem',
        'Możliwość wznowienia (14 dni)',
      ],
      featured: true,
      badge: 'Najpopularniejsze',
    },
    {
      icon: '👥',
      title: 'Zarządzanie Zespołem',
      description:
        'Dodawaj operatorów, wysyłaj zaproszenia i monitoruj aktywność zespołu. Tylko dla Administratorów.',
      features: [
        'Wysyłanie zaproszeń emailem',
        'Status aktywacji operatorów',
        'Dezaktywacja dostępu',
      ],
    },
  ];

  const steps = [
    {
      number: 1,
      icon: '📧',
      title: 'Aktywuj konto',
      description:
        'Otrzymasz zaproszenie emailem od administratora serwisu. Kliknij link, ustaw hasło i gotowe!',
    },
    {
      number: 2,
      icon: '👥',
      title: 'Dodaj zespół',
      description:
        'Jeśli jesteś Administratorem, zaproś operatorów do współpracy. Wyślij im zaproszenia jednym kliknięciem.',
    },
    {
      number: 3,
      icon: '🎯',
      title: 'Twórz zgłoszenia',
      description:
        'Zgłaszaj problemy, dodawaj załączniki i śledź postęp naprawy w czasie rzeczywistym.',
    },
  ];

  const benefits = [
    {
      icon: '✅',
      title: 'Pełna transparentność',
      description:
        'Zobacz status naprawy w czasie rzeczywistym. Klienci wiedzą co się dzieje, co zmniejsza liczbę zapytań do serwisu.',
    },
    {
      icon: '✅',
      title: 'Łatwość obsługi',
      description:
        'Intuicyjny interfejs, który nie wymaga szkoleń. Każdy operator może zacząć korzystać od razu.',
    },
    {
      icon: '✅',
      title: 'Scentralizowana komunikacja',
      description:
        'Cała korespondencja z serwisem w kontekście zgłoszenia. Brak chaotycznych wątków emailowych.',
    },
    {
      icon: '✅',
      title: 'Autonomia zespołu',
      description:
        'Administratorzy mogą samodzielnie zarządzać dostępem swoich pracowników bez kontaktu z serwisem.',
    },
  ];

  return (
    <div className="landing-page">
      <Navbar variant="client" />

      <HeroSection {...heroData} />

      {/* Features Section */}
      <section id="features" className="features">
        <div className="section-header">
          <span className="section-badge">Możliwości</span>
          <h2 className="section-title">Wszystko pod kontrolą</h2>
        </div>
        <div className="features-grid">
          {features.map((feature, index) => (
            <FeatureCard key={index} {...feature} />
          ))}
        </div>
      </section>

      {/* How It Works Section */}
      <section id="how-it-works" className="how-it-works">
        <div className="section-header">
          <span className="section-badge">Jak to działa</span>
          <h2 className="section-title">Zacznij w 3 prostych krokach</h2>
        </div>
        <div className="how-it-works-steps">
          {steps.map((step, index) => (
            <HowItWorksStep key={index} {...step} />
          ))}
        </div>
      </section>

      {/* Benefits Section */}
      <section id="benefits" className="benefits">
        <div className="benefits-content">
          <div className="benefits-visual">
            <div className="timeline-preview">
              <h3 style={{ marginBottom: '1.5rem', color: '#1a202c' }}>
                Historia Zgłoszenia #TC-0042
              </h3>
              <div className="timeline-item">
                <div className="timeline-marker">📎</div>
                <div className="timeline-content">
                  <div className="timeline-label">Organizacja</div>
                  <div className="timeline-text">Dodano załącznik: zdjecie_problemu.jpg</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-marker">🔧</div>
                <div className="timeline-content">
                  <div className="timeline-label">Serwis</div>
                  <div className="timeline-text">Status zmieniony: W trakcie</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-marker">💬</div>
                <div className="timeline-content">
                  <div className="timeline-label">Organizacja</div>
                  <div className="timeline-text">Dodano komentarz do zgłoszenia</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-marker">✅</div>
                <div className="timeline-content">
                  <div className="timeline-label">Serwis</div>
                  <div className="timeline-text">Status zmieniony: Rozwiązany</div>
                </div>
              </div>
            </div>
          </div>
          <div className="benefits-text">
            <span className="section-badge">Korzyści</span>
            <h2 className="section-title">Dlaczego FLOWerTRACK dla Twojej Organizacji?</h2>
            <div className="benefits-list">
              {benefits.map((benefit, index) => (
                <BenefitItem key={index} {...benefit} />
              ))}
            </div>
          </div>
        </div>
      </section>

      <CTASection
        headline="Gotowy na lepszą komunikację z serwisem?"
        subheadline="Aktywuj swoje konto i zacznij korzystać już dziś"
        primaryButtonText="Aktywuj konto"
        secondaryButtonText="Kontakt z serwisem"
      />

      <Footer variant="client" />
    </div>
  );
};

export default ClientLanding;
