import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Button, Input } from '../../components/ui';
import { useAuth } from '../../contexts/AuthContext';
import { useToast } from '../../hooks/useToast';
import './ServiceLoginPage.css';

const ServiceLoginPage = () => {
  const navigate = useNavigate();
  const { loginService } = useAuth();
  const { error: showError } = useToast();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});

  const validateForm = (): boolean => {
    const newErrors: { email?: string; password?: string } = {};

    if (!email) {
      newErrors.email = 'Email jest wymagany';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Nieprawidłowy format email';
    }

    if (!password) {
      newErrors.password = 'Hasło jest wymagane';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      await loginService(email, password);
      navigate('/service/dashboard');
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Nieprawidłowy email lub hasło';
      showError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-card-wrapper">
      <div className="login-content-stack">
        <Link to="/" className="back-link">
          ← Wróć
        </Link>

        <div className="login-card service-card-theme">
          <div className="login-header">
            <div className="login-logo">
              <svg
                width="32"
                height="32"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.488.488 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.484.484 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L3.19 9.17c-.46 1.05.02 1.93.12.61l2.03 1.58c-.05.3-.07.62-.07.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z"
                  fill="currentColor"
                />
              </svg>
            </div>
            <h2 className="login-title">Panel Technika</h2>
            <p className="login-subtitle">Zaloguj się aby rozpocząć pracę</p>
          </div>

          <form className="login-form" onSubmit={handleSubmit}>
            <Input
              type="email"
              label="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="technik@flowertrack.dev"
              error={errors.email}
              disabled={isLoading}
              autoComplete="email"
              required
            />

            <Input
              type="password"
              label="Hasło"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              error={errors.password}
              disabled={isLoading}
              autoComplete="current-password"
              required
            />

            <div className="login-actions">
              <Link to="/service/forgot-password" className="form-link">
                Zapomniałeś hasła?
              </Link>
            </div>

            <Button
              type="submit"
              variant="primary"
              size="lg"
              disabled={isLoading}
              fullWidth
              isLoading={isLoading}
            >
              Zaloguj się
            </Button>
          </form>

          <div className="form-footer">
            <p>
              Nie ten portal?{' '}
              <Link to="/client" className="form-link">
                Przejdź do Strefy Klienta
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ServiceLoginPage;
export { ServiceLoginPage };
