import React from 'react';
import { Link } from 'react-router-dom';
import './Footer.css';

export const Footer: React.FC = () => {
  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-grid">
          <div className="footer-column">
            <div className="footer-logo">
              <span className="footer-logo-icon">⚙️</span>
              <span className="footer-logo-text">FLOWerTRACK</span>
            </div>
            <p className="footer-description">
              Zaawansowany system zarządzania zgłoszeniami serwisowymi dla firm zajmujących się
              serwisem urządzeń produkcyjnych.
            </p>
          </div>

          <div className="footer-column">
            <h4 className="footer-title">Produkt</h4>
            <ul className="footer-links">
              <li>
                <button className="footer-link">Funkcje</button>
              </li>
              <li>
                <button className="footer-link">Korzyści</button>
              </li>
              <li>
                <button className="footer-link">Demo</button>
              </li>
              <li>
                <Link to="/client" className="footer-link">
                  Portal Klienta
                </Link>
              </li>
            </ul>
          </div>

          <div className="footer-column">
            <h4 className="footer-title">Firma</h4>
            <ul className="footer-links">
              <li>
                <button className="footer-link">O nas</button>
              </li>
              <li>
                <button className="footer-link">Blog</button>
              </li>
              <li>
                <button className="footer-link">Kontakt</button>
              </li>
              <li>
                <button className="footer-link">Kariera</button>
              </li>
            </ul>
          </div>

          <div className="footer-column">
            <h4 className="footer-title">Wsparcie</h4>
            <ul className="footer-links">
              <li>
                <button className="footer-link">Dokumentacja</button>
              </li>
              <li>
                <button className="footer-link">FAQ</button>
              </li>
              <li>
                <button className="footer-link">Pomoc techniczna</button>
              </li>
            </ul>
          </div>
        </div>

        <div className="footer-bottom">
          <p className="footer-copyright">
            © 2025 FLOWerTRACK by Baumalog. Wszystkie prawa zastrzeżone.
          </p>
        </div>
      </div>
    </footer>
  );
};
