import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Navbar,
  HeroSection,
  FeatureCard,
  BenefitItem,
  HowItWorksStep,
  CTASection,
  Footer,
} from '../../components/landing';
import './ClientLanding.css';

export const ClientLanding: React.FC = () => {
  const navigate = useNavigate();

  const heroStats = [
    { value: '24/7', label: 'Dostęp do statusów' },
    { value: '2 min', label: 'Utworzenie zgłoszenia' },
    { value: '100%', label: 'Transparentność procesu' },
  ];

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

  const ctaButtons = [
    {
      text: 'Aktywuj konto',
      variant: 'primary' as const,
      onClick: () => navigate('/client/login'),
    },
    {
      text: 'Kontakt z serwisem',
      variant: 'outline' as const,
    },
  ];

  return (
    <div className="landing-page">
      <Navbar variant="client" />

      <HeroSection
        badge="🎯 Pełna kontrola w Twoich rękach"
        title="Śledź swoje maszyny i zgłoszenia w jednym miejscu"
        highlightedWord="jednym miejscu"
        subtitle="Dedykowany portal dla operatorów i administratorów organizacji. Twórz zgłoszenia, śledź statusy maszyn i zarządzaj zespołem bez wychodzenia z aplikacji."
        primaryCTA={{
          text: 'Aktywuj konto',
          icon: '→',
          onClick: () => navigate('/client/login'),
        }}
        secondaryCTA={{
          text: 'Dowiedz się więcej',
          icon: '📖',
        }}
        stats={heroStats}
        backgroundIcons={['🏭', '🖥️', '📱', '✉️']}
      />

      <section className="features-section" id="features">
        <div className="features-container">
          <div className="features-header">
            <div className="features-badge">Możliwości</div>
            <h2 className="features-title">Wszystko pod kontrolą</h2>
          </div>
          <div className="features-grid features-grid-client">
            {features.map((feature, index) => (
              <FeatureCard key={index} {...feature} />
            ))}
          </div>
        </div>
      </section>

      <section className="how-it-works-section" id="benefits">
        <div className="how-it-works-container">
          <div className="how-it-works-header">
            <h2 className="how-it-works-title">Jak to działa?</h2>
            <p className="how-it-works-subtitle">Zacznij w 3 prostych krokach</p>
          </div>
          <div className="how-it-works-steps">
            {steps.map((step) => (
              <HowItWorksStep key={step.number} {...step} />
            ))}
          </div>
        </div>
      </section>

      <section className="benefits-section">
        <div className="benefits-container">
          <div className="benefits-visual">
            <div className="timeline-preview">
              <div className="timeline-item">
                <div className="timeline-dot timeline-dot-org"></div>
                <div className="timeline-content">
                  <div className="timeline-label">[Organizacja]</div>
                  <div className="timeline-text">Dodano załącznik: zdjecie_problemu.jpg</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-dot timeline-dot-service"></div>
                <div className="timeline-content">
                  <div className="timeline-label">[Serwis]</div>
                  <div className="timeline-text">Status zmieniony: W trakcie</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-dot timeline-dot-org"></div>
                <div className="timeline-content">
                  <div className="timeline-label">[Organizacja]</div>
                  <div className="timeline-text">Dodano komentarz</div>
                </div>
              </div>
              <div className="timeline-item">
                <div className="timeline-dot timeline-dot-service"></div>
                <div className="timeline-content">
                  <div className="timeline-label">[Serwis]</div>
                  <div className="timeline-text">Status zmieniony: Rozwiązany</div>
                </div>
              </div>
            </div>
          </div>
          <div className="benefits-content">
            <h2 className="benefits-title">Dlaczego FLOWerTRACK dla Twojej Organizacji?</h2>
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
        buttons={ctaButtons}
      />

      <Footer />
    </div>
  );
};
