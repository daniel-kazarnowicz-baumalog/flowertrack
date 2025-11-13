import { type ReactNode, useState } from 'react';
import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import './ServiceLayout.css';

interface ServiceLayoutProps {
  children: ReactNode;
}

/**
 * ServiceLayout component
 * Main layout for service portal with sidebar navigation
 */
export const ServiceLayout = ({ children }: ServiceLayoutProps) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/service/login');
  };

  const toggleSidebar = () => {
    setIsSidebarOpen(!isSidebarOpen);
  };

  const closeSidebar = () => {
    setIsSidebarOpen(false);
  };

  const isAdmin = user?.isAdmin ?? false;

  return (
    <div className="serviceLayout">
      {/* Sidebar */}
      <aside
        className={`serviceLayout__sidebar ${isSidebarOpen ? 'serviceLayout__sidebar--visible' : ''}`}
      >
        {/* Logo/Header */}
        <div className="serviceLayout__sidebarHeader">
          <Link to="/service/dashboard" className="serviceLayout__logo" onClick={closeSidebar}>
            FLOWerTRACK
          </Link>
          <div className="serviceLayout__logoSubtitle">Service Portal</div>
        </div>

        {/* Navigation */}
        <nav className="serviceLayout__nav">
          <ul className="serviceLayout__navList">
            <li className="serviceLayout__navItem">
              <NavLink
                to="/service/dashboard"
                className={({ isActive }) =>
                  `serviceLayout__navLink ${isActive ? 'serviceLayout__navLink--active' : ''}`
                }
                onClick={closeSidebar}
              >
                <svg
                  className="serviceLayout__navIcon"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"
                  />
                </svg>
                Dashboard
              </NavLink>
            </li>

            <li className="serviceLayout__navItem">
              <NavLink
                to="/service/tickets"
                className={({ isActive }) =>
                  `serviceLayout__navLink ${isActive ? 'serviceLayout__navLink--active' : ''}`
                }
                onClick={closeSidebar}
              >
                <svg
                  className="serviceLayout__navIcon"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
                  />
                </svg>
                Tickets
              </NavLink>
            </li>

            <li className="serviceLayout__navItem">
              <NavLink
                to="/service/organizations"
                className={({ isActive }) =>
                  `serviceLayout__navLink ${isActive ? 'serviceLayout__navLink--active' : ''}`
                }
                onClick={closeSidebar}
              >
                <svg
                  className="serviceLayout__navIcon"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"
                  />
                </svg>
                Organizations
              </NavLink>
            </li>

            <li className="serviceLayout__navItem">
              <NavLink
                to="/service/machines"
                className={({ isActive }) =>
                  `serviceLayout__navLink ${isActive ? 'serviceLayout__navLink--active' : ''}`
                }
                onClick={closeSidebar}
              >
                <svg
                  className="serviceLayout__navIcon"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                  />
                </svg>
                Machines
              </NavLink>
            </li>

            {isAdmin && (
              <li className="serviceLayout__navItem">
                <NavLink
                  to="/service/admin"
                  className={({ isActive }) =>
                    `serviceLayout__navLink ${isActive ? 'serviceLayout__navLink--active' : ''}`
                  }
                  onClick={closeSidebar}
                >
                  <svg
                    className="serviceLayout__navIcon"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
                    />
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                    />
                  </svg>
                  Admin
                </NavLink>
              </li>
            )}
          </ul>
        </nav>

        {/* User Section */}
        <div className="serviceLayout__user">
          <div className="serviceLayout__userName">
            {user?.firstName} {user?.lastName}
          </div>
          <div className="serviceLayout__userRole">{isAdmin ? 'Administrator' : 'Technician'}</div>
          <button className="serviceLayout__logoutBtn" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </aside>

      {/* Overlay for mobile */}
      <div
        className={`serviceLayout__overlay ${isSidebarOpen ? 'serviceLayout__overlay--visible' : ''}`}
        onClick={closeSidebar}
      />

      {/* Main Content */}
      <main
        className={`serviceLayout__main ${isSidebarOpen ? '' : 'serviceLayout__main--expanded'}`}
      >
        {/* Top Bar (mobile) */}
        <div className="serviceLayout__topBar">
          <button
            className="serviceLayout__menuBtn"
            onClick={toggleSidebar}
            aria-label="Toggle menu"
          >
            <svg
              className="serviceLayout__menuIcon"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M4 6h16M4 12h16M4 18h16"
              />
            </svg>
          </button>
          <div className="serviceLayout__topBarTitle">FLOWerTRACK</div>
          <div className="serviceLayout__topBarSpacer" />
        </div>

        {/* Page Content */}
        <div className="serviceLayout__content">{children}</div>
      </main>
    </div>
  );
};
