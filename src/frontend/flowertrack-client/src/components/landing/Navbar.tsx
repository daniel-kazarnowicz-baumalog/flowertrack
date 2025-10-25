import { Link } from 'react-router-dom';

interface NavbarProps {
  variant: 'service' | 'client';
}

export const Navbar = ({ variant }: NavbarProps) => {
  const isService = variant === 'service';

  return (
    <nav className="navbar">
      <div className="nav-container">
        <Link to="/" className="logo">
          <span className="logo-icon">⚙️</span>
          <span className="logo-text">
            FLOW<span className="highlight">er</span>TRACK
          </span>
        </Link>
        <div className="nav-links">
          <a href="#features">{isService ? 'Funkcje' : 'Możliwości'}</a>
          {!isService && <a href="#how-it-works">Jak działa</a>}
          <a href="#benefits">Korzyści</a>
          <a href={isService ? '/client' : '/service'}>
            {isService ? 'Dla Klientów' : 'Dla Serwisu'}
          </a>
          <a href="#contact">Kontakt</a>
          <Link to={isService ? '/client' : '/service'} className="btn-portal-switch">
            {isService ? 'Portal Klienta' : 'Portal Serwisu'}
          </Link>
          <button className="btn-login">Zaloguj się</button>
        </div>
      </div>
    </nav>
  );
};
