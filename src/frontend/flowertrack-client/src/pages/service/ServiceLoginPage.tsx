import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Button, Input, Loader } from '../../components/ui';
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
    <div className="service-login">
      <div className="service-login__container">
        <div className="service-login__header">
          <div className="service-login__logo">
            <svg
              width="48"
              height="48"
              viewBox="0 0 48 48"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <rect width="48" height="48" rx="8" fill="#3b82f6" />
              <path
                d="M24 12L16 20H20V28H28V20H32L24 12Z"
                fill="white"
                stroke="white"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
          <h1 className="service-login__title">Portal Serwisowy</h1>
          <p className="service-login__subtitle">Zaloguj się do panelu zarządzania zgłoszeniami</p>
        </div>

        <form className="service-login__form" onSubmit={handleSubmit}>
          <Input
            type="email"
            label="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="twoj.email@example.com"
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

          <div className="service-login__actions">
            <Link to="/service/forgot-password" className="service-login__link">
              Zapomniałeś hasła?
            </Link>
          </div>

          <Button type="submit" variant="primary" size="lg" disabled={isLoading} fullWidth>
            {isLoading ? <Loader size="sm" /> : 'Zaloguj się'}
          </Button>
        </form>

        <div className="service-login__footer">
          <p className="service-login__footer-text">
            Jesteś klientem?{' '}
            <Link to="/client/login" className="service-login__link">
              Przejdź do portalu klienta
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ServiceLoginPage;
export { ServiceLoginPage };
