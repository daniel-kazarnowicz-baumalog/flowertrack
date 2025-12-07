import { useState } from 'react';
import { Outlet, NavLink, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { ThemeSwitcher } from '../ui/ThemeSwitcher';
import './ClientLayout.css';

export const ClientLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/client');
  };

  return (
    <div className={`clientLayout ${isSidebarCollapsed ? 'clientLayout--collapsed' : ''}`}>
      <aside className="clientLayout__sidebar">
        <Link to="/client/dashboard" className="clientLayout__logo">
          <span className="clientLayout__logoIcon">🌸</span>
          <span className="clientLayout__logoText">FLOWerTRACK Client</span>
        </Link>

        <nav className="clientLayout__nav">
          <NavLink
            to="/client/dashboard"
            className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}
          >
            <span className="icon">📊</span>
            <span className="text">Pulpit</span>
          </NavLink>
          <NavLink
            to="/client/tickets"
            className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}
          >
            <span className="icon">🎫</span>
            <span className="text">Zgłoszenia</span>
          </NavLink>
          <NavLink
            to="/client/team"
            className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}
          >
            <span className="icon">👥</span>
            <span className="text">Mój Zespół</span>
          </NavLink>
        </nav>

        <div className="clientLayout__user">
          <div className="clientLayout__userInfo">
            <div className="clientLayout__userAvatar">{user?.name?.charAt(0) || 'C'}</div>
            <div className="clientLayout__userDetails">
              <span className="clientLayout__userName">{user?.name || 'Client User'}</span>
              <span className="clientLayout__userRole">Client</span>
            </div>
          </div>
          <button onClick={handleLogout} className="clientLayout__logoutBtn" title="Logout">
            ↪
          </button>
        </div>
      </aside>

      <main className="clientLayout__content">
        <header className="clientLayout__header">
          <button
            className="clientLayout__toggleBtn"
            onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
          >
            ☰
          </button>
          <div
            className="clientLayout__headerActions"
            style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}
          >
            <span className="clientLayout__welcome">Witaj, {user?.name}</span>
            <ThemeSwitcher variant="buttons" />
          </div>
        </header>
        <div className="clientLayout__pageContainer">
          <Outlet />
        </div>
      </main>
    </div>
  );
};
