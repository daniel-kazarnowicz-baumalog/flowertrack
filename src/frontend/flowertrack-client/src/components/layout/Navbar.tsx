import { Link, useNavigate } from 'react-router-dom';
import { useAuth, isServiceUser } from '../../contexts/AuthContext';
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
    navigate(portal === 'service' ? '/service/login' : '/client/login');
  };

  // Don't show navbar if not authenticated
  if (!isAuthenticated || !user) {
    return null;
  }

  const isService = portal === 'service';
  const userName = isService
    ? (user as { fullName: string }).fullName
    : `${(user as { firstName: string; lastName: string }).firstName} ${(user as { firstName: string; lastName: string }).lastName}`;

  const isAdmin = isServiceUser(user) ? user.role === 'Admin' : user.isAdmin;

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
