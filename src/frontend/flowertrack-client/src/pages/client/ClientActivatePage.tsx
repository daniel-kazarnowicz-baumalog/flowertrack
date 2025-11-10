import { useState, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { apiClient } from '../../lib/apiClient';
import { useToast } from '../../hooks/useToast';
import { Button, Input } from '../../components/ui';
import './ClientActivatePage.css';

/**
 * ClientActivatePage
 * Allows new client users to activate their account and set password
 * US-005: Aktywacja konta operatora przez link z emaila
 */
export const ClientActivatePage = () => {
  const { token } = useParams<{ token: string }>();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isValidating, setIsValidating] = useState(true);
  const [isTokenValid, setIsTokenValid] = useState(false);
  const [tokenError, setTokenError] = useState<'expired' | 'invalid' | 'already-active' | null>(
    null
  );
  const [errors, setErrors] = useState<{
    password?: string;
    confirmPassword?: string;
  }>({});
  const { showToast } = useToast();

  // Validate token on mount and get email
  useEffect(() => {
    const validateToken = async () => {
      if (!token) {
        setIsTokenValid(false);
        setTokenError('invalid');
        setIsValidating(false);
        return;
      }

      try {
        // In real implementation, backend should validate token and return email
        // For now, we'll assume token is valid
        setIsTokenValid(true);
        setEmail('user@example.com'); // This would come from API
        setTokenError(null);
      } catch (error: unknown) {
        console.error('Token validation error:', error);
        const status = (error as { response?: { status?: number } }).response?.status;

        if (status === 410) {
          setTokenError('expired');
        } else if (status === 409) {
          setTokenError('already-active');
        } else {
          setTokenError('invalid');
        }
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
      await apiClient.post('/api/auth/client/activate', {
        token,
        password,
      });

      showToast('Konto aktywne! Możesz teraz się zalogować', 'success');

      // Redirect to login after 2 seconds
      setTimeout(() => {
        navigate('/client/login');
      }, 2000);
    } catch (error: unknown) {
      console.error('Account activation error:', error);
      const status = (error as { response?: { status?: number } }).response?.status;

      if (status === 410) {
        showToast('Link wygasł. Poproś administratora o ponowne wysłanie zaproszenia.', 'error');
      } else if (status === 409) {
        showToast('Konto już aktywne. Przejdź do logowania.', 'error');
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
      <div className="clientActivate">
        <div className="clientActivate__container">
          <div className="clientActivate__card">
            <div className="clientActivate__loading">
              <div className="clientActivate__spinner" />
              <p>Sprawdzanie zaproszenia...</p>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Invalid/expired token
  if (!isTokenValid || !token) {
    const getErrorMessage = () => {
      switch (tokenError) {
        case 'expired':
          return {
            title: 'Link wygasł',
            message:
              'Ten link aktywacyjny wygasł. Poproś administratora organizacji o ponowne wysłanie zaproszenia.',
          };
        case 'already-active':
          return {
            title: 'Konto już aktywne',
            message: 'To konto zostało już aktywowane. Możesz się zalogować.',
          };
        default:
          return {
            title: 'Nieprawidłowy link',
            message:
              'Ten link aktywacyjny jest nieprawidłowy. Sprawdź czy skopiowałeś pełny adres URL z emaila.',
          };
      }
    };

    const errorContent = getErrorMessage();

    return (
      <div className="clientActivate">
        <div className="clientActivate__container">
          <div className="clientActivate__card">
            <div className="clientActivate__error">
              <svg
                className="clientActivate__errorIcon"
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
              <h1 className="clientActivate__errorTitle">{errorContent.title}</h1>
              <p className="clientActivate__errorText">{errorContent.message}</p>
              <Link to="/client/login" className="clientActivate__errorLink">
                Przejdź do logowania
              </Link>
            </div>
          </div>
        </div>
      </div>
    );
  }

  const passwordStrength = getPasswordStrength(password);

  return (
    <div className="clientActivate">
      <div className="clientActivate__container">
        <div className="clientActivate__card">
          {/* Header */}
          <div className="clientActivate__header">
            <h1 className="clientActivate__title">Aktywuj konto</h1>
            <p className="clientActivate__subtitle">Ustaw hasło, aby dokończyć aktywację</p>
          </div>

          {/* Email Display */}
          <div className="clientActivate__emailInfo">
            <svg
              className="clientActivate__emailIcon"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
            <span className="clientActivate__email">{email}</span>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="clientActivate__form">
            <Input
              type="password"
              label="Hasło"
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
              <div className="clientActivate__strength">
                <span className="clientActivate__strengthLabel">Siła hasła:</span>
                <span
                  className={`clientActivate__strengthValue clientActivate__strengthValue--${passwordStrength.toLowerCase()}`}
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
            <div className="clientActivate__requirements">
              <p className="clientActivate__requirementsTitle">Wymagania hasła:</p>
              <ul className="clientActivate__requirementsList">
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
              Aktywuj konto
            </Button>
          </form>

          {/* Footer */}
          <div className="clientActivate__footer">
            <p className="clientActivate__footerText">
              Masz już konto?{' '}
              <Link to="/client/login" className="clientActivate__footerLink">
                Zaloguj się
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};
