import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import '../styles/login.css';
import api from '../lib/api';
import { useAuth } from '../contexts/AuthContext';

const ServiceLogin = () => {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const response = await api.post('/auth/service/login', { email, password });
      const { accessToken, user } = response.data;
      
      // Check if user is admin based on role string from backend
      const isAdmin = user.role === 'ServiceAdministrator' || user.role === 'Admin';
      
      login(accessToken, {
        id: user.id,
        email: user.email,
        role: 'service',
        firstName: user.firstName,
        lastName: user.lastName,
        fullName: user.fullName || `${user.firstName || ''} ${user.lastName || ''}`.trim(),
        isAdmin,
      });
      navigate('/service/dashboard');
    } catch (err: any) {
      console.error('Login error:', err);
      setError(err.response?.data?.message || t('auth.loginError'));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container service-theme">
      <div className="login-card">
        <div className="login-header">
          <Link to="/" className="back-link">
            ← {t('common.back')}
          </Link>
          <div className="icon-circle">🔧</div>
          <h2>{t('auth.servicePortal')}</h2>
          <p>{t('auth.loginToTechnicalPanel')}</p>
        </div>

        <form onSubmit={handleSubmit} className="login-form">
          {error && (
            <div
              className="error-message"
              style={{ color: 'red', marginBottom: '1rem', textAlign: 'center' }}
            >
              {error}
            </div>
          )}
          <div className="form-group">
            <label htmlFor="email">{t('auth.workEmail')}</label>
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
        </form>

        <div className="login-footer">
          <a href="#">{t('auth.forgotPassword')}</a>
        </div>
      </div>
    </div>
  );
};

export default ServiceLogin;
