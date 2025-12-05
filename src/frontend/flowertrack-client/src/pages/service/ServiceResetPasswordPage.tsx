import { useState, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { apiClient } from '../../lib/apiClient';
import { useToast } from '../../hooks/useToast';
import { Button, Input } from '../../components/ui';
import './ServiceResetPasswordPage.css';

/**
 * ServiceResetPasswordPage
 * Allows service users to reset their password using a token from email
 * US-002: Resetowanie hasła przy użyciu opcji "Zapomniałem hasła"
 */
export const ServiceResetPasswordPage = () => {
  const { token } = useParams<{ token: string }>();
  const navigate = useNavigate();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isValidating, setIsValidating] = useState(true);
  const [isTokenValid, setIsTokenValid] = useState(false);
  const [errors, setErrors] = useState<{ password?: string; confirmPassword?: string }>({});
  const { showToast } = useToast();

  // Validate token on mount
  useEffect(() => {
    const validateToken = async () => {
      if (!token) {
        setIsTokenValid(false);
        setIsValidating(false);
        return;
      }

      try {
        // Try to validate token by making a request
        // Backend should have a validation endpoint or we check on submit
        setIsTokenValid(true);
      } catch (error) {
        console.error('Token validation error:', error);
        setIsTokenValid(false);
      } finally {
        setIsValidating(false);
      }
    };

    validateToken();
  }, [token]);

  const getPasswordStrength = (password: string): string => {
    if (password.length === 0) return '';
    if (password.length < 8) return 'Słabe';

    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;

    if (strength <= 2) return 'Słabe';
    if (strength <= 3) return 'Średnie';
    return 'Silne';
  };

  const validatePassword = (password: string, confirmPassword: string): boolean => {
    const newErrors: { password?: string; confirmPassword?: string } = {};

    if (!password.trim()) {
      newErrors.password = 'Hasło jest wymagane';
    } else if (password.length < 8) {
      newErrors.password = 'Hasło musi mieć minimum 8 znaków';
    } else if (!/[A-Z]/.test(password)) {
      newErrors.password = 'Hasło musi zawierać wielką literę';
    } else if (!/[0-9]/.test(password)) {
      newErrors.password = 'Hasło musi zawierać cyfrę';
    } else if (!/[^a-zA-Z0-9]/.test(password)) {
      newErrors.password = 'Hasło musi zawierać znak specjalny';
    }

    if (!confirmPassword.trim()) {
      newErrors.confirmPassword = 'Potwierdzenie hasła jest wymagane';
    } else if (password !== confirmPassword) {
      newErrors.confirmPassword = 'Hasła nie są identyczne';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validatePassword(password, confirmPassword)) {
      return;
    }

    setIsLoading(true);
    setErrors({});

    try {
      await apiClient.post('/api/auth/service/reset-password', {
        token,
        newPassword: password,
      });

      showToast('Hasło zostało zmienione pomyślnie', 'success');

      // Redirect to login after 2 seconds
      setTimeout(() => {
        navigate('/service/login');
      }, 2000);
    } catch (error: unknown) {
      console.error('Reset password error:', error);

      // Check if token expired
      if ((error as { response?: { status?: number } }).response?.status === 400) {
        showToast('Link wygasł. Spróbuj ponownie.', 'error');
      } else {
        showToast('Wystąpił błąd. Spróbuj ponownie.', 'error');
      }
    } finally {
      setIsLoading(false);
    }
  };

  // Loading state
  if (isValidating) {
    return (
      <div className="serviceResetPassword">
        <div className="serviceResetPassword__container">
          <div className="serviceResetPassword__card">
            <div className="serviceResetPassword__loading">
              <div className="serviceResetPassword__spinner" />
              <p>Sprawdzanie linku...</p>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Invalid/expired token
  if (!isTokenValid || !token) {
    return (
      <div className="serviceResetPassword">
        <div className="serviceResetPassword__container">
          <div className="serviceResetPassword__card">
            <div className="serviceResetPassword__error">
              <svg
                className="serviceResetPassword__errorIcon"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <h1 className="serviceResetPassword__errorTitle">Link wygasł</h1>
              <p className="serviceResetPassword__errorText">
                Ten link do resetowania hasła wygasł lub jest nieprawidłowy. Spróbuj ponownie.
              </p>
              <Link to="/service/forgot-password" className="serviceResetPassword__errorLink">
                Wyślij nowy link
              </Link>
            </div>
          </div>
        </div>
      </div>
    );
  }

  const passwordStrength = getPasswordStrength(password);

  return (
    <div className="serviceResetPassword">
      <div className="serviceResetPassword__container">
        <div className="serviceResetPassword__card">
          {/* Header */}
          <div className="serviceResetPassword__header">
            <h1 className="serviceResetPassword__title">Ustaw nowe hasło</h1>
            <p className="serviceResetPassword__subtitle">
              Wprowadź nowe hasło dla swojego konta serwisowego
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="serviceResetPassword__form">
            <Input
              type="password"
              label="Nowe hasło"
              placeholder="Minimum 8 znaków"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              error={errors.password}
              disabled={isLoading}
              required
              autoComplete="new-password"
              autoFocus
            />

            {/* Password Strength Indicator */}
            {password && (
              <div className="serviceResetPassword__strength">
                <span className="serviceResetPassword__strengthLabel">Siła hasła:</span>
                <span
                  className={`serviceResetPassword__strengthValue serviceResetPassword__strengthValue--${passwordStrength.toLowerCase()}`}
                >
                  {passwordStrength}
                </span>
              </div>
            )}

            <Input
              type="password"
              label="Potwierdź hasło"
              placeholder="Wpisz hasło ponownie"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              error={errors.confirmPassword}
              disabled={isLoading}
              required
              autoComplete="new-password"
            />

            {/* Password Requirements */}
            <div className="serviceResetPassword__requirements">
              <p className="serviceResetPassword__requirementsTitle">Wymagania hasła:</p>
              <ul className="serviceResetPassword__requirementsList">
                <li className={password.length >= 8 ? 'valid' : ''}>Minimum 8 znaków</li>
                <li className={/[A-Z]/.test(password) ? 'valid' : ''}>Wielka litera</li>
                <li className={/[0-9]/.test(password) ? 'valid' : ''}>Cyfra</li>
                <li className={/[^a-zA-Z0-9]/.test(password) ? 'valid' : ''}>Znak specjalny</li>
              </ul>
            </div>

            <Button
              type="submit"
              variant="primary"
              fullWidth
              disabled={isLoading}
              isLoading={isLoading}
            >
              Zmień hasło
            </Button>
          </form>

          {/* Back to Login */}
          <div className="serviceResetPassword__footer">
            <Link to="/service/login" className="serviceResetPassword__backLink">
              <svg
                className="serviceResetPassword__backIcon"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                />
              </svg>
              Powrót do logowania
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};
