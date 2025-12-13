import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Button, Input } from '../../components/ui';
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
    <div className="login-card-wrapper">
      <div className="login-content-stack">
        <Link to="/" className="back-link">
          ← Wróć
        </Link>

        <div className="login-card client-card-theme">
          <div className="login-header">
            <div className="login-logo">
              <svg
                width="32"
                height="32"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <rect width="24" height="24" rx="4" fill="currentColor" fillOpacity="0.2" />
                <path
                  d="M12 7C9.24 7 7 9.24 7 12C7 14.76 9.24 17 12 17C14.76 17 17 14.76 17 12C17 9.24 14.76 7 12 7ZM12 10C12.83 10 13.5 10.67 13.5 11.5C13.5 12.33 12.83 13 12 13C11.17 13 10.5 12.33 10.5 11.5C10.5 10.67 11.17 10 12 10ZM12 15.6C10.75 15.6 9.645 14.96 9 13.99C9.015 12.995 11 12.45 12 12.45C13 12.45 14.985 12.995 15 13.99C14.355 14.96 13.25 15.6 12 15.6Z"
                  fill="currentColor"
                />
              </svg>
            </div>
            <h2 className="login-title">Witaj ponownie</h2>
            <p className="login-subtitle">Wprowadź swoje dane aby się zalogować</p>
          </div>

          <form className="login-form" onSubmit={handleSubmit}>
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
              Pracownik serwisu?{' '}
              <Link to="/service" className="form-link">
                Przejdź tutaj
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ClientLoginPage;
export { ClientLoginPage };
