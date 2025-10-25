import './App.css';

function App() {
  return (
    <div className="landing-page">
      {/* Navigation */}
      <nav className="navbar">
        <div className="nav-container">
          <div className="logo">
            <span className="logo-icon">⚙️</span>
            <span className="logo-text">
              FLOW<span className="highlight">er</span>TRACK
            </span>
          </div>
          <div className="nav-links">
            <a href="#features">Funkcje</a>
            <a href="#benefits">Korzyści</a>
            <a href="#contact">Kontakt</a>
            <button className="btn-login">Zaloguj się</button>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <header className="hero">
        <div className="hero-content">
          <div className="hero-badge">
            <span className="badge-text">🚀 Nowa generacja zarządzania serwisem</span>
          </div>
          <h1 className="hero-title">
            Centralne zarządzanie
            <br />
            zgłoszeniami <span className="gradient-text">serwisowymi</span>
          </h1>
          <p className="hero-subtitle">
            Kompleksowa platforma dla firm zajmujących się serwisem urządzeń produkcyjnych. Skróć
            czas reakcji, zwiększ transparentność i zautomatyzuj workflow zgłoszeń.
          </p>
          <div className="hero-cta">
            <button className="btn-primary">
              <span>Rozpocznij bezpłatnie</span>
              <span className="arrow">→</span>
            </button>
            <button className="btn-secondary">
              <span className="play-icon">▶</span>
              Zobacz demo
            </button>
          </div>
          <div className="hero-stats">
            <div className="stat">
              <div className="stat-value">100+</div>
              <div className="stat-label">Obsłużonych zgłoszeń</div>
            </div>
            <div className="stat">
              <div className="stat-value">-60%</div>
              <div className="stat-label">Czasu reakcji</div>
            </div>
            <div className="stat">
              <div className="stat-value">99.9%</div>
              <div className="stat-label">Dostępności systemu</div>
            </div>
          </div>
        </div>

        {/* Animated background elements */}
        <div className="hero-background">
          <div className="floating-icon icon-1">🔧</div>
          <div className="floating-icon icon-2">⚙️</div>
          <div className="floating-icon icon-3">🔩</div>
          <div className="floating-icon icon-4">🛠️</div>
          <div className="grid-pattern"></div>
        </div>
      </header>

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
          <div className="feature-card">
            <div className="feature-icon-wrapper">
              <div className="feature-icon">🎯</div>
            </div>
            <h3>Portal Serwisu</h3>
            <p>
              Kompleksowy dashboard dla zespołu serwisowego z KPI, automatycznym przydzielaniem
              zadań i pełnym audytem zmian.
            </p>
            <ul className="feature-list">
              <li>Dashboard z kafelkami KPI</li>
              <li>Zarządzanie zgłoszeniami</li>
              <li>Workflow statusów</li>
            </ul>
          </div>

          <div className="feature-card featured">
            <div className="feature-badge">Najpopularniejsze</div>
            <div className="feature-icon-wrapper">
              <div className="feature-icon">👥</div>
            </div>
            <h3>Portal Klienta</h3>
            <p>
              Dedykowany portal dla operatorów i administratorów klientów z pełną transparentnością
              procesu serwisowego.
            </p>
            <ul className="feature-list">
              <li>Śledzenie statusu maszyn</li>
              <li>Tworzenie zgłoszeń</li>
              <li>Zarządzanie zespołem</li>
            </ul>
          </div>

          <div className="feature-card">
            <div className="feature-icon-wrapper">
              <div className="feature-icon">📊</div>
            </div>
            <h3>Integracja Logów</h3>
            <p>
              Automatyczne wysyłanie logów z maszyn produkcyjnych do systemu. Wszystkie informacje w
              jednym miejscu.
            </p>
            <ul className="feature-list">
              <li>Automatyczne zbieranie danych</li>
              <li>Historia problemów</li>
              <li>Analiza trendów</li>
            </ul>
          </div>

          <div className="feature-card">
            <div className="feature-icon-wrapper">
              <div className="feature-icon">�</div>
            </div>
            <h3>Scentralizowana Komunikacja</h3>
            <p>
              Komentarze, załączniki, oś czasu - cała komunikacja między serwisem a klientem w
              kontekście ticketu.
            </p>
            <ul className="feature-list">
              <li>Timeline zmian</li>
              <li>Załączniki i galeria</li>
              <li>Pełny audyt akcji</li>
            </ul>
          </div>

          <div className="feature-card">
            <div className="feature-icon-wrapper">
              <div className="feature-icon">🔒</div>
            </div>
            <h3>Bezpieczeństwo</h3>
            <p>
              Wielopoziomowy system uprawnień, tokeny API dla organizacji i bezpieczne sesje
              użytkowników.
            </p>
            <ul className="feature-list">
              <li>Role i uprawnienia</li>
              <li>Tokeny API</li>
              <li>Audyt zmian</li>
            </ul>
          </div>

          <div className="feature-card">
            <div className="feature-icon-wrapper">
              <div className="feature-icon">⚡</div>
            </div>
            <h3>Szybkość i Efektywność</h3>
            <p>
              Automatyzacja procesów, masowe akcje i szybkie filtry znacząco skracają czas obsługi
              zgłoszeń.
            </p>
            <ul className="feature-list">
              <li>Masowe akcje</li>
              <li>Zaawansowane filtry</li>
              <li>Szybkie przydzielanie</li>
            </ul>
          </div>
        </div>
      </section>

      {/* Benefits Section */}
      <section id="benefits" className="benefits">
        <div className="benefits-content">
          <div className="benefits-text">
            <span className="section-badge">Korzyści</span>
            <h2 className="section-title">Dlaczego FLOWerTRACK?</h2>
            <div className="benefits-list">
              <div className="benefit-item">
                <div className="benefit-icon">✅</div>
                <div>
                  <h4>Pełna transparentność</h4>
                  <p>
                    Klienci widzą status naprawy w czasie rzeczywistym, co zmniejsza liczbę zapytań.
                  </p>
                </div>
              </div>
              <div className="benefit-item">
                <div className="benefit-icon">✅</div>
                <div>
                  <h4>Centralizacja informacji</h4>
                  <p>
                    Wszystkie zgłoszenia, logi i komunikacja w jednym miejscu - koniec z chaosem.
                  </p>
                </div>
              </div>
              <div className="benefit-item">
                <div className="benefit-icon">✅</div>
                <div>
                  <h4>Szybsza reakcja</h4>
                  <p>
                    Automatyczne zbieranie danych z maszyn i natychmiastowe powiadamianie zespołu.
                  </p>
                </div>
              </div>
              <div className="benefit-item">
                <div className="benefit-icon">✅</div>
                <div>
                  <h4>Pełny audyt</h4>
                  <p>Historia wszystkich zmian z informacją kto, co i kiedy zmienił.</p>
                </div>
              </div>
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
                <div className="preview-card">� Aktywne zgłoszenia: 24</div>
                <div className="preview-card">⚠️ Krytyczne: 3</div>
                <div className="preview-card">✅ Rozwiązane: 156</div>
                <div className="preview-chart"></div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section id="contact" className="cta-section">
        <div className="cta-content">
          <h2>Gotowy na optymalizację serwisu?</h2>
          <p>Dołącz do firm, które już korzystają z FLOWerTRACK</p>
          <div className="cta-buttons">
            <button className="btn-primary-large">Rozpocznij za darmo</button>
            <button className="btn-outline">Skontaktuj się z nami</button>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="footer">
        <div className="footer-content">
          <div className="footer-column">
            <div className="footer-logo">
              <span className="logo-icon">⚙️</span>
              <span>FLOWerTRACK</span>
            </div>
            <p>
              Zaawansowany system zarządzania zgłoszeniami serwisowymi dla firm zajmujących się
              serwisem urządzeń produkcyjnych.
            </p>
          </div>
          <div className="footer-column">
            <h4>Produkt</h4>
            <a href="#features">Funkcje</a>
            <a href="#benefits">Korzyści</a>
            <a href="#">Demo</a>
          </div>
          <div className="footer-column">
            <h4>Firma</h4>
            <a href="#">O nas</a>
            <a href="#">Blog</a>
            <a href="#contact">Kontakt</a>
          </div>
          <div className="footer-column">
            <h4>Wsparcie</h4>
            <a href="#">Dokumentacja</a>
            <a href="#">FAQ</a>
            <a href="#">Pomoc</a>
          </div>
        </div>
        <div className="footer-bottom">
          <p>&copy; 2025 FLOWerTRACK by Baumalog. Wszystkie prawa zastrzeżone.</p>
        </div>
      </footer>
    </div>
  );
}

export default App;
