import type { ReactNode } from 'react';
import { NavLink, Link, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../../contexts/AuthContext';
import './Sidebar.css';

interface SidebarItem {
  to: string;
  icon: ReactNode;
  label: string;
}

interface SidebarSection {
  title?: string;
  items: SidebarItem[];
}

interface SidebarProps {
  sections: SidebarSection[];
  isOpen: boolean;
  onCloseMobile: () => void;
  logoLink?: string;
  logoText?: string;
  isCollapsed?: boolean;
  onToggleCollapse?: () => void;
}

export const Sidebar = ({
  sections,
  isOpen,
  onCloseMobile,
  logoLink = '/',
  logoText = 'FLOWerTRACK',
  isCollapsed = false,
  onToggleCollapse,
}: SidebarProps) => {
  const { user, logout } = useAuth();
  const { t } = useTranslation();
  const location = useLocation();

  const handleLogout = () => {
    logout();
  };

  return (
    <>
      {/* Mobile Overlay */}
      <div
        className={`sidebar-overlay ${isOpen ? 'open' : ''}`}
        onClick={onCloseMobile}
        aria-hidden="true"
      />

      <aside className={`sidebar ${isOpen ? 'open' : ''} ${isCollapsed ? 'collapsed' : ''}`}>
        <div className="sidebar__header">
          {!isCollapsed && (
            <Link to={logoLink} className="sidebar__logo">
              <span className="sidebar__logoIcon">🌸</span>
              <span className="sidebar__logoText">{logoText}</span>
            </Link>
          )}
          {isCollapsed && (
            <Link to={logoLink} className="sidebar__logo sidebar__logo--collapsed" title={logoText}>
              <span className="sidebar__logoIcon">🌸</span>
            </Link>
          )}

          <button
            className="sidebar__collapseBtn"
            onClick={onToggleCollapse}
            title={isCollapsed ? t('common.expand', 'Rozwiń') : t('common.collapse', 'Zwiń')}
          >
            {isCollapsed ? '»' : '«'}
          </button>
        </div>

        <div className="sidebar__content">
          <nav className="sidebar__nav">
            {sections.map((section, idx) => (
              <div key={idx} className="sidebar__section">
                {section.title && !isCollapsed && (
                  <h3 className="sidebar__sectionTitle">{section.title}</h3>
                )}
                {section.title && isCollapsed && <div className="sidebar__sectionDivider" />}
                <ul className="sidebar__list">
                  {section.items.map((item) => (
                    <li key={item.to}>
                      <NavLink
                        to={item.to}
                        className={({ isActive }) =>
                          `sidebar__link ${isActive || location.pathname.startsWith(item.to) ? 'active' : ''}`
                        }
                        onClick={onCloseMobile}
                        end={item.to === logoLink}
                        title={isCollapsed ? item.label : undefined}
                      >
                        <span className="sidebar__linkIcon">{item.icon}</span>
                        {!isCollapsed && <span className="sidebar__linkText">{item.label}</span>}
                      </NavLink>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </nav>
        </div>

        <div className="sidebar__footer">
          <div className="sidebar__user">
            <div className="sidebar__userAvatar">
              {(user?.fullName || user?.firstName || user?.name || user?.email || 'U')
                .charAt(0)
                .toUpperCase()}
            </div>
            {!isCollapsed && (
              <div className="sidebar__userDetails">
                <span className="sidebar__userName">
                  {user?.fullName || user?.name || user?.email || 'User'}
                </span>
                <span className="sidebar__userRole">
                  {user?.role === 'service'
                    ? user.isAdmin
                      ? t('roles.admin', 'Administrator')
                      : t('roles.technician', 'Technik')
                    : t('roles.client', 'Klient')}
                </span>
              </div>
            )}
          </div>

          {!isCollapsed && (
            <button
              onClick={handleLogout}
              className="sidebar__logoutBtn"
              title={t('auth.logout', 'Wyloguj')}
            >
              ↪
            </button>
          )}
        </div>
      </aside>
    </>
  );
};
