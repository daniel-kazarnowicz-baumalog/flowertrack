import { useTranslation } from 'react-i18next';
import { useAuth } from '../../contexts/AuthContext';

const ClientDashboard = () => {
    const { t } = useTranslation();
    const { user, logout } = useAuth();

    return (
        <div style={{ padding: '2rem' }}>
            <h1>{t('dashboard.title')}</h1>
            <p>{t('auth.welcome', 'Witaj')}, {user?.email}</p>
            <p>{t('users.role')}: {user?.role}</p>
            <button onClick={logout} style={{ padding: '0.5rem 1rem', marginTop: '1rem', cursor: 'pointer' }}>
                {t('auth.logout')}
            </button>
        </div>
    );
};

export default ClientDashboard;
