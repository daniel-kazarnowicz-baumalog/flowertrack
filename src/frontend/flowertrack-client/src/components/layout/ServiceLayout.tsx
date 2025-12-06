import { useState } from 'react';
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import './ServiceLayout.css';

export const ServiceLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/service');
  };

  return (
    <div className={`serviceLayout ${isSidebarCollapsed ? 'serviceLayout--collapsed' : ''}`}>
      <aside className="serviceLayout__sidebar">
        <div className="serviceLayout__logo">
          <span className="serviceLayout__logoIcon">🌸</span>
          <span className="serviceLayout__logoText">FLOWerTRACK</span>
        </div>

        <nav className="serviceLayout__nav">
          <div className="serviceLayout__navSection">
            <h3 className="serviceLayout__navTitle">Main</h3>
            <NavLink to="/service/dashboard" className={({ isActive }) => `serviceLayout__navLink ${isActive ? 'active' : ''}`}>
              <span className="icon">📊</span>
              <span className="text">Dashboard</span>
            </NavLink>
            <NavLink to="/service/tickets" className={({ isActive }) => `serviceLayout__navLink ${isActive ? 'active' : ''}`}>
              <span className="icon">🎫</span>
              <span className="text">Zgłoszenia</span>
            </NavLink>
          </div>

          <div className="serviceLayout__navSection">
            <h3 className="serviceLayout__navTitle">Manage</h3>
            <NavLink to="/service/organizations" className={({ isActive }) => `serviceLayout__navLink ${isActive ? 'active' : ''}`}>
              <span className="icon">🏢</span>
              <span className="text">Klienci</span>
            </NavLink>
            <NavLink to="/service/machines" className={({ isActive }) => `serviceLayout__navLink ${isActive ? 'active' : ''}`}>
              <span className="icon">🏭</span>
              <span className="text">Maszyny</span>
            </NavLink>
            <NavLink to="/service/users" className={({ isActive }) => `serviceLayout__navLink ${isActive ? 'active' : ''}`}>
              <span className="icon">👥</span>
              <span className="text">Zespól</span>
            </NavLink>
          </div>
        </nav>

        <div className="serviceLayout__user">
          <div className="serviceLayout__userInfo">
            <div className="serviceLayout__userAvatar">{user?.name?.charAt(0) || 'S'}</div>
            <div className="serviceLayout__userDetails">
              <span className="serviceLayout__userName">{user?.name || 'Service User'}</span>
              <span className="serviceLayout__userRole">Technician</span>
            </div>
          </div>
          <button onClick={handleLogout} className="serviceLayout__logoutBtn" title="Logout">
            ↪
          </button>
        </div>
      </aside>

      <main className="serviceLayout__content">
        <header className="serviceLayout__header">
          <button
            className="serviceLayout__toggleBtn"
            onClick={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
          >
            ☰
          </button>
          <div className="serviceLayout__headerActions">
            <span className="serviceLayout__date">{new Date().toLocaleDateString()}</span>
          </div>
        </header>
        <div className="serviceLayout__pageContainer">
          <Outlet />
        </div>
      </main>
    </div>
  );
};
