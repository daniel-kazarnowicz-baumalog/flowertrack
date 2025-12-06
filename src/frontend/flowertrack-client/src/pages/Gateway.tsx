import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { LanguageSwitcher } from '../components/ui/LanguageSwitcher';
import { ThemeSwitcher } from '../components/ui/ThemeSwitcher';
import '../styles/gateway.css';

const Gateway = () => {
    const { t } = useTranslation();

    return (
        <div className="gateway-container">
            <div className="gateway-controls">
                <LanguageSwitcher />
                <ThemeSwitcher variant="toggle" />
            </div>
            <div className="gateway-content">
                <div className="gateway-header">
                    <span className="logo-icon">⚙️</span>
                    <h1>FLOW<span className="highlight">er</span>TRACK</h1>
                    <p className="gateway-subtitle">{t('gateway.subtitle')}</p>
                </div>

                <div className="gateway-options">
                    <Link to="/service" className="gateway-card service-card">
                        <div className="card-icon">🔧</div>
                        <h2>{t('auth.servicePortal')}</h2>
                        <p>{t('gateway.serviceDescription')}</p>
                        <div className="card-arrow">→</div>
                    </Link>

                    <Link to="/client" className="gateway-card client-card">
                        <div className="card-icon">🏢</div>
                        <h2>{t('auth.clientPortal')}</h2>
                        <p>{t('gateway.clientDescription')}</p>
                        <div className="card-arrow">→</div>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default Gateway;
