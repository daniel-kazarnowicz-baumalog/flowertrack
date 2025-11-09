import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Button, Input, Loader } from '../../components/ui';
import { useAuth } from '../../contexts/AuthContext';
import { useToast } from '../../hooks/useToast';
import './ClientLoginPage.css';

const ClientLoginPage = () => {
  const navigate = useNavigate();
  const { loginClient } = useAuth();
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
      await loginClient(email, password);
      navigate('/client/dashboard');
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Nieprawidłowy email lub hasło';
      showError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="client-login">
      <div className="client-login__container">
        <div className="client-login__header">
          <div className="client-login__logo">
            <svg
              width="48"
              height="48"
              viewBox="0 0 48 48"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <rect width="48" height="48" rx="8" fill="#10b981" />
              <path
                d="M24 14C18.48 14 14 18.48 14 24C14 29.52 18.48 34 24 34C29.52 34 34 29.52 34 24C34 18.48 29.52 14 24 14ZM24 20C25.66 20 27 21.34 27 23C27 24.66 25.66 26 24 26C22.34 26 21 24.66 21 23C21 21.34 22.34 20 24 20ZM24 31.2C21.5 31.2 19.29 29.92 18 27.98C18.03 25.99 22 24.9 24 24.9C25.99 24.9 29.97 25.99 30 27.98C28.71 29.92 26.5 31.2 24 31.2Z"
                fill="white"
              />
            </svg>
          </div>
          <h1 className="client-login__title">Portal Klienta</h1>
          <p className="client-login__subtitle">Zarządzaj swoimi zgłoszeniami i maszynami</p>
        </div>

        <form className="client-login__form" onSubmit={handleSubmit}>
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

          <Button type="submit" variant="primary" size="lg" disabled={isLoading} fullWidth>
            {isLoading ? <Loader size="sm" /> : 'Zaloguj się'}
          </Button>
        </form>

        <div className="client-login__footer">
          <p className="client-login__footer-text">
            Jesteś pracownikiem serwisu?{' '}
            <Link to="/service/login" className="client-login__link">
              Przejdź do portalu serwisowego
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ClientLoginPage;
