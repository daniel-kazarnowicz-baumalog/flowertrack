import { Link } from 'react-router-dom';
import './QuickActions.css';

export const QuickActions = () => {
  return (
    <div className="quickActions">
      {/* "New Ticket" button removed as per request - service portal doesn't create tickets generally */}
      <Link to="/service/machines?action=register" className="quickActions__btn">
        <span className="quickActions__icon">🏭</span>
        Rejestruj Maszynę
      </Link>
      <Link to="/service/organizations?action=new" className="quickActions__btn">
        <span className="quickActions__icon">🏢</span>
        Dodaj Klienta
      </Link>
    </div>
  );
};
