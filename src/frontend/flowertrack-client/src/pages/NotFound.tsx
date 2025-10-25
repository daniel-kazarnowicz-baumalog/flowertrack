import { Link } from 'react-router-dom';
import '../styles/landing.css';

const NotFound = () => {
  return (
    <div className="landing-page">
      <div
        style={{
          minHeight: '100vh',
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '2rem',
          textAlign: 'center',
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
        }}
      >
        <h1 style={{ fontSize: '6rem', fontWeight: '800', marginBottom: '1rem' }}>404</h1>
        <h2 style={{ fontSize: '2rem', marginBottom: '1rem' }}>Strona nie znaleziona</h2>
        <p style={{ fontSize: '1.25rem', marginBottom: '2rem', opacity: 0.9 }}>
          Przepraszamy, ale strona której szukasz nie istnieje.
        </p>
        <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap', justifyContent: 'center' }}>
          <Link to="/" className="btn-primary">
            <span>Strona główna (Portal Serwisu)</span>
          </Link>
          <Link to="/client" className="btn-secondary">
            <span>Portal Klienta</span>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default NotFound;
