import { useTheme, type Theme } from '../../contexts/ThemeContext';
import { useTranslation } from 'react-i18next';
import './ThemeSwitcher.css';

export type ThemeSwitcherVariant = 'buttons' | 'dropdown' | 'toggle' | 'icon';

export interface ThemeSwitcherProps {
  /** Wariant wyświetlania */
  variant?: ThemeSwitcherVariant;
  /** Pokaż etykiety */
  showLabels?: boolean;
  /** Dodatkowa klasa CSS */
  className?: string;
}

/**
 * Komponent do przełączania motywu (light/dark/system)
 */
export function ThemeSwitcher({
  variant = 'buttons',
  showLabels = false,
  className = '',
}: ThemeSwitcherProps) {
  const { theme, setTheme, resolvedTheme, toggleTheme } = useTheme();
  const { t } = useTranslation();

  const themes: { value: Theme; icon: string; label: string }[] = [
    { value: 'light', icon: '☀️', label: t('theme.light', 'Jasny') },
    { value: 'dark', icon: '🌙', label: t('theme.dark', 'Ciemny') },
    { value: 'system', icon: '💻', label: t('theme.system', 'Systemowy') },
  ];

  // Wariant ikony (prosty przycisk)
  if (variant === 'icon') {
    return (
      <button
        type="button"
        className={`themeSwitcher themeSwitcher--icon ${className}`}
        onClick={toggleTheme}
        title={
          resolvedTheme === 'dark'
            ? t('theme.switchToLight', 'Przełącz na jasny')
            : t('theme.switchToDark', 'Przełącz na ciemny')
        }
        aria-label={
          resolvedTheme === 'dark'
            ? t('theme.switchToLight', 'Przełącz na jasny')
            : t('theme.switchToDark', 'Przełącz na ciemny')
        }
      >
        <span className="themeSwitcher__icon">
          {resolvedTheme === 'dark' ? '🌙' : '☀️'}
        </span>
      </button>
    );
  }

  // Wariant z przyciskami
  if (variant === 'buttons') {
    return (
      <div className={`themeSwitcher themeSwitcher--buttons ${className}`}>
        {themes.map(({ value, icon, label }) => (
          <button
            key={value}
            type="button"
            className={`themeSwitcher__btn ${theme === value ? 'themeSwitcher__btn--active' : ''}`}
            onClick={() => setTheme(value)}
            title={label}
            aria-label={label}
            aria-pressed={theme === value}
          >
            <span className="themeSwitcher__icon">{icon}</span>
            {showLabels && <span className="themeSwitcher__label">{label}</span>}
          </button>
        ))}
      </div>
    );
  }

  // Wariant toggle (tylko light/dark)
  if (variant === 'toggle') {
    return (
      <button
        type="button"
        className={`themeSwitcher themeSwitcher--toggle ${className}`}
        onClick={toggleTheme}
        title={
          resolvedTheme === 'dark'
            ? t('theme.switchToLight', 'Przełącz na jasny')
            : t('theme.switchToDark', 'Przełącz na ciemny')
        }
        aria-label={
          resolvedTheme === 'dark'
            ? t('theme.switchToLight', 'Przełącz na jasny')
            : t('theme.switchToDark', 'Przełącz na ciemny')
        }
      >
        <span className="themeSwitcher__track">
          <span
            className={`themeSwitcher__thumb ${resolvedTheme === 'dark' ? 'themeSwitcher__thumb--dark' : ''}`}
          >
            <span className="themeSwitcher__thumbIcon">
              {resolvedTheme === 'dark' ? '🌙' : '☀️'}
            </span>
          </span>
        </span>
        {showLabels && (
          <span className="themeSwitcher__label">
            {resolvedTheme === 'dark' ? t('theme.dark', 'Ciemny') : t('theme.light', 'Jasny')}
          </span>
        )}
      </button>
    );
  }

  // Wariant dropdown
  return (
    <div className={`themeSwitcher themeSwitcher--dropdown ${className}`}>
      <select
        value={theme}
        onChange={(e) => setTheme(e.target.value as Theme)}
        className="themeSwitcher__select"
        aria-label={t('theme.selectTheme', 'Wybierz motyw')}
      >
        {themes.map(({ value, icon, label }) => (
          <option key={value} value={value}>
            {icon} {label}
          </option>
        ))}
      </select>
    </div>
  );
}

export default ThemeSwitcher;
