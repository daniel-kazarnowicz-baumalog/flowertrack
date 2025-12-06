import { useState } from 'react';
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
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
        <div className="clientLayout__logo">
          <span className="clientLayout__logoIcon">🌸</span>
          <span className="clientLayout__logoText">FLOWerTRACK Client</span>
        </div>

        <nav className="clientLayout__nav">
          <NavLink to="/client/dashboard" className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}>
            <span className="icon">📊</span>
            <span className="text">Pulpit</span>
          </NavLink>
          <NavLink to="/client/tickets" className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}>
            <span className="icon">🎫</span>
            <span className="text">Zgłoszenia</span>
          </NavLink>
          <NavLink to="/client/team" className={({ isActive }) => `clientLayout__navLink ${isActive ? 'active' : ''}`}>
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
          <span className="clientLayout__welcome">Witaj, {user?.name}</span>
        </header>
        <div className="clientLayout__pageContainer">
          <Outlet />
        </div>
      </main>
    </div>
  );
};
