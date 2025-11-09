import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

interface NavbarProps {
  variant: 'service' | 'client';
}

export const Navbar: React.FC<NavbarProps> = ({ variant }) => {
  const isService = variant === 'service';

  const scrollToSection = (id: string) => {
    const element = document.getElementById(id);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-logo">
          <span className="logo-icon">⚙️</span>
          <span className="logo-text">FLOWerTRACK</span>
        </Link>

        <div className="navbar-links">
          <button
            onClick={() => scrollToSection('features')}
            className="navbar-link"
          >
            {isService ? 'Funkcje' : 'Możliwości'}
          </button>
          <button
            onClick={() => scrollToSection('benefits')}
            className="navbar-link"
          >
            {isService ? 'Korzyści' : 'Jak działa'}
          </button>
          <Link
            to={isService ? '/client' : '/service'}
            className="navbar-link"
          >
            {isService ? 'Dla Klientów' : 'Dla Serwisu'}
          </Link>
          <button
            onClick={() => scrollToSection('contact')}
            className="navbar-link"
          >
            Kontakt
          </button>
        </div>

        <div className="navbar-actions">
          <Link
            to={isService ? '/client' : '/service'}
            className="navbar-btn navbar-btn-secondary"
          >
            {isService ? 'Portal Klienta' : 'Portal Serwisu'}
          </Link>
          <Link
            to={isService ? '/service/login' : '/client/login'}
            className="navbar-btn navbar-btn-primary"
          >
            Zaloguj się
          </Link>
        </div>
      </div>
    </nav>
  );
};
