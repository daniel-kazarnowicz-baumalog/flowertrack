import { useState } from 'react';
import { Link } from 'react-router-dom';
import { apiClient } from '../../lib/apiClient';
import { useToast } from '../../hooks/useToast';
import { Button, Input } from '../../components/ui';
import './ServiceForgotPasswordPage.css';

/**
 * ServiceForgotPasswordPage
 * Allows service users to request a password reset link
 * US-002: Resetowanie hasła przy użyciu opcji "Zapomniałem hasła"
 */
export const ServiceForgotPasswordPage = () => {
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [emailSent, setEmailSent] = useState(false);
  const [errors, setErrors] = useState<{ email?: string }>({});
  const { showToast } = useToast();

  const validateEmail = (email: string): boolean => {
    if (!email.trim()) {
      setErrors({ email: 'Email jest wymagany' });
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setErrors({ email: 'Nieprawidłowy format email' });
      return false;
    }

    setErrors({});
    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateEmail(email)) {
      return;
    }

    setIsLoading(true);
    setErrors({});

    try {
      await apiClient.post('/api/auth/service/forgot-password', { email });

      // Success - always show same message for security
      setEmailSent(true);
      showToast('Email z instrukcjami został wysłany', 'success');
    } catch (error: unknown) {
      console.error('Forgot password error:', error);

      // For security, we don't reveal if email exists or not
      // Always show success message
      setEmailSent(true);
      showToast('Email z instrukcjami został wysłany', 'success');
    } finally {
      setIsLoading(false);
    }
  };

  if (emailSent) {
    return (
      <div className="serviceForgotPassword">
        <div className="serviceForgotPassword__container">
          <div className="serviceForgotPassword__card">
            {/* Logo/Header */}
            <div className="serviceForgotPassword__header">
              <h1 className="serviceForgotPassword__title">Email wysłany</h1>
              <p className="serviceForgotPassword__subtitle">Sprawdź swoją skrzynkę pocztową</p>
            </div>

            {/* Success Message */}
            <div className="serviceForgotPassword__success">
              <svg
                className="serviceForgotPassword__successIcon"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <p className="serviceForgotPassword__successText">
                Jeśli podany adres email istnieje w systemie, wysłaliśmy na niego link do
                zresetowania hasła.
              </p>
              <p className="serviceForgotPassword__successNote">
                Link będzie ważny przez <strong>24 godziny</strong>.
              </p>
            </div>

            {/* Back to Login */}
            <div className="serviceForgotPassword__footer">
              <Link to="/service/login" className="serviceForgotPassword__backLink">
                <svg
                  className="serviceForgotPassword__backIcon"
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
  }

  return (
    <div className="serviceForgotPassword">
      <div className="serviceForgotPassword__container">
        <div className="serviceForgotPassword__card">
          {/* Logo/Header */}
          <div className="serviceForgotPassword__header">
            <h1 className="serviceForgotPassword__title">Zapomniałeś hasła?</h1>
            <p className="serviceForgotPassword__subtitle">
              Podaj swój adres email, a wyślemy Ci link do zresetowania hasła
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="serviceForgotPassword__form">
            <Input
              type="email"
              label="Email"
              placeholder="twoj.email@baumalog.pl"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              error={errors.email}
              disabled={isLoading}
              required
              autoComplete="email"
              autoFocus
            />

            <Button
              type="submit"
              variant="primary"
              fullWidth
              disabled={isLoading}
              isLoading={isLoading}
            >
              Wyślij link resetujący
            </Button>
          </form>

          {/* Back to Login */}
          <div className="serviceForgotPassword__footer">
            <Link to="/service/login" className="serviceForgotPassword__backLink">
              <svg
                className="serviceForgotPassword__backIcon"
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
