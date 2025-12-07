import { Link, useNavigate } from 'react-router-dom';
import { useAuth, type User } from '../../contexts/AuthContext';
import { Button } from '../ui';
import './Navbar.css';

export interface NavbarProps {
  portal: 'service' | 'client';
}

/**
 * Top navigation bar component
 * Adapts to Service or Client portal context
 */
export function Navbar({ portal }: NavbarProps) {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate(portal === 'service' ? '/service' : '/client');
  };

  // Don't show navbar if not authenticated
  if (!isAuthenticated || !user) {
    return null;
  }

  const isService = portal === 'service';

  // Get user display name
  const getUserName = (u: User): string => {
    if (u.fullName) return u.fullName;
    if (u.firstName && u.lastName) return `${u.firstName} ${u.lastName}`;
    if (u.name) return u.name;
    return u.email;
  };

  const userName = getUserName(user);
  const isAdmin = user.isAdmin || false;

  return (
    <nav className={`navbar navbar--${portal}`}>
      <div className="navbar__container">
        <div className="navbar__brand">
          <Link to={`/${portal}`} className="navbar__logo">
            <span className="navbar__logo-icon">🌸</span>
            <span className="navbar__logo-text">FLOWerTRACK</span>
          </Link>
        </div>

        <div className="navbar__menu">
          <Link to={`/${portal}/dashboard`} className="navbar__link">
            Dashboard
          </Link>
          <Link to={`/${portal}/tickets`} className="navbar__link">
            Tickets
          </Link>
          {isService && (
            <Link to={`/${portal}/organizations`} className="navbar__link">
              Organizations
            </Link>
          )}
          {!isService && isAdmin && (
            <Link to={`/${portal}/team`} className="navbar__link">
              Team
            </Link>
          )}
          {isService && isAdmin && (
            <Link to={`/${portal}/admin/users`} className="navbar__link">
              Admin
            </Link>
          )}
        </div>

        <div className="navbar__user">
          <span className="navbar__username">{userName}</span>
          {isAdmin && <span className="navbar__role-badge">Admin</span>}
          <Button variant="ghost" size="sm" onClick={handleLogout}>
            Logout
          </Button>
        </div>
      </div>
    </nav>
  );
}
