interface FooterProps {
  variant: 'service' | 'client';
}

export const Footer = ({ variant }: FooterProps) => {
  const isService = variant === 'service';

  return (
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
          <a href="#features">{isService ? 'Funkcje' : 'Możliwości'}</a>
          <a href="#benefits">Korzyści</a>
          <a href="/demo">Demo</a>
          <a href={isService ? '/client' : '/service'}>
            {isService ? 'Portal Klienta' : 'Portal Serwisu'}
          </a>
        </div>
        <div className="footer-column">
          <h4>Firma</h4>
          <a href="/about">O nas</a>
          <a href="/blog">Blog</a>
          <a href="#contact">Kontakt</a>
          <a href="/career">Kariera</a>
        </div>
        <div className="footer-column">
          <h4>Wsparcie</h4>
          <a href="/docs">Dokumentacja</a>
          <a href="/faq">FAQ</a>
          <a href="/support">Pomoc techniczna</a>
        </div>
      </div>
      <div className="footer-bottom">
        <p>&copy; 2025 FLOWerTRACK by Baumalog. Wszystkie prawa zastrzeżone.</p>
      </div>
    </footer>
  );
};
