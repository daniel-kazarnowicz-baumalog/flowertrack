import { useTranslation } from 'react-i18next';
import { useTheme } from '../../contexts/ThemeContext';
import './ThemeToggle.css';

interface ThemeToggleProps {
  className?: string;
  variant?: 'icon' | 'buttons';
}

export const ThemeToggle = ({ className = '', variant = 'icon' }: ThemeToggleProps) => {
  const { theme, setTheme, resolvedTheme } = useTheme();
  const { t } = useTranslation();

  if (variant === 'buttons') {
    return (
      <div className={`themeToggle-buttons ${className}`}>
        <button
          className={`themeToggle-btn ${theme === 'light' ? 'active' : ''}`}
          onClick={() => setTheme('light')}
          title={t('common.themeLight', 'Jasny')}
        >
          ☀️
        </button>
        <button
          className={`themeToggle-btn ${theme === 'system' ? 'active' : ''}`}
          onClick={() => setTheme('system')}
          title={t('common.themeSystem', 'System')}
        >
          💻
        </button>
        <button
          className={`themeToggle-btn ${theme === 'dark' ? 'active' : ''}`}
          onClick={() => setTheme('dark')}
          title={t('common.themeDark', 'Ciemny')}
        >
          🌙
        </button>
      </div>
    );
  }

  // Icon variant - cycles through
  const toggleTheme = () => {
    if (theme === 'light') setTheme('dark');
    else setTheme('light');
  };

  return (
    <button
      className={`themeToggle ${className}`}
      onClick={toggleTheme}
      title={resolvedTheme === 'dark' ? 'Switch to Light Mode' : 'Switch to Dark Mode'}
    >
      {resolvedTheme === 'dark' ? '🌙' : '☀️'}
    </button>
  );
};
