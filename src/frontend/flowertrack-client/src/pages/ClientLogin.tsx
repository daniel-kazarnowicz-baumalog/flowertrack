import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import '../styles/login.css';
import api from '../lib/api';
import { useAuth } from '../contexts/AuthContext';

const ClientLogin = () => {
    const { t } = useTranslation();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const { login, mockLogin } = useAuth();
    const navigate = useNavigate();

    /**
     * @obsolete DEV ONLY - Handle mock login without backend
     * TODO: Remove before production deployment
     */
    const handleMockLogin = () => {
        mockLogin('client');
        navigate('/client/dashboard');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const response = await api.post('/auth/client/login', { email, password });
            const { token, user } = response.data;
            login(token, user || { email, role: 'client' });
            navigate('/client/dashboard');
        } catch (err: any) {
            console.error('Login error:', err);
            setError(err.response?.data?.message || t('auth.loginError'));
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container client-theme">
            <div className="login-card">
                <div className="login-header">
                    <Link to="/" className="back-link">← {t('common.back')}</Link>
                    <div className="icon-circle">🏢</div>
                    <h2>{t('auth.clientPortal')}</h2>
                    <p>{t('auth.loginToClientPanel')}</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    {error && <div className="error-message" style={{ color: 'red', marginBottom: '1rem', textAlign: 'center' }}>{error}</div>}
                    <div className="form-group">
                        <label htmlFor="email">{t('auth.email')}</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder={t('auth.emailPlaceholder')}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">{t('auth.password')}</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="••••••••"
                            required
                        />
                    </div>

                    <button type="submit" className="login-btn" disabled={isLoading}>
                        {isLoading ? t('auth.loggingIn') : t('auth.loginButton')}
                    </button>

                    {/* @obsolete DEV ONLY - Remove this button before production deployment */}
                    <button
                        type="button"
                        className="login-btn"
                        onClick={handleMockLogin}
                        style={{
                            marginTop: '0.5rem',
                            background: 'linear-gradient(135deg, #ff6b6b, #ee5a24)',
                            border: '2px dashed #fff'
                        }}
                    >
                        {t('auth.mockLoginDev')}
                    </button>
                </form>

                <div className="login-footer">
                    <a href="#">{t('auth.noAccount')}</a>
                </div>
            </div>
        </div>
    );
};

export default ClientLogin;
