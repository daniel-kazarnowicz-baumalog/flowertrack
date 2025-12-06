import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  useMemo,
  type ReactNode,
} from 'react';

/**
 * Typy dla motywów
 */
export type Theme = 'light' | 'dark' | 'system';
export type ResolvedTheme = 'light' | 'dark';

/**
 * Interfejs kontekstu
 */
interface ThemeContextValue {
  /** Aktualnie wybrany motyw (light/dark/system) */
  theme: Theme;
  /** Rzeczywisty motyw po rozwiązaniu 'system' */
  resolvedTheme: ResolvedTheme;
  /** Funkcja do zmiany motywu */
  setTheme: (theme: Theme) => void;
  /** Przełącznik między light/dark */
  toggleTheme: () => void;
  /** Czy motyw jest w trakcie ładowania */
  isLoading: boolean;
}

/**
 * Klucz do localStorage
 */
const STORAGE_KEY = 'flowertrack-theme';

/**
 * Context
 */
const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

/**
 * Helper do wykrywania preferencji systemowych
 */
function getSystemTheme(): ResolvedTheme {
  if (typeof window !== 'undefined') {
    return window.matchMedia('(prefers-color-scheme: dark)').matches
      ? 'dark'
      : 'light';
  }
  return 'light';
}

/**
 * Helper do odczytu z localStorage
 */
function getStoredTheme(): Theme | null {
  if (typeof window !== 'undefined') {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === 'light' || stored === 'dark' || stored === 'system') {
      return stored;
    }
  }
  return null;
}

/**
 * Helper do zapisu do localStorage
 */
function setStoredTheme(theme: Theme): void {
  if (typeof window !== 'undefined') {
    localStorage.setItem(STORAGE_KEY, theme);
  }
}

/**
 * Helper do aplikowania motywu na document
 */
function applyTheme(resolvedTheme: ResolvedTheme): void {
  if (typeof document !== 'undefined') {
    const root = document.documentElement;

    // Usuń poprzedni atrybut
    root.removeAttribute('data-theme');

    // Ustaw nowy
    root.setAttribute('data-theme', resolvedTheme);

    // Opcjonalnie: meta tag dla mobile browsers
    const metaThemeColor = document.querySelector('meta[name="theme-color"]');
    if (metaThemeColor) {
      metaThemeColor.setAttribute(
        'content',
        resolvedTheme === 'dark' ? '#0f172a' : '#ffffff'
      );
    }
  }
}

/**
 * Props dla ThemeProvider
 */
interface ThemeProviderProps {
  children: ReactNode;
  /** Domyślny motyw (jeśli nie ma w localStorage) */
  defaultTheme?: Theme;
  /** Wymuś konkretny motyw (ignoruje localStorage) */
  forcedTheme?: Theme;
}

/**
 * Provider dla motywu
 */
export function ThemeProvider({
  children,
  defaultTheme = 'system',
  forcedTheme,
}: ThemeProviderProps) {
  const [theme, setThemeState] = useState<Theme>(() => {
    // Jeśli wymuszony motyw, użyj go
    if (forcedTheme) return forcedTheme;

    // Sprawdź localStorage
    const stored = getStoredTheme();
    if (stored) return stored;

    // Użyj domyślnego
    return defaultTheme;
  });

  const [resolvedTheme, setResolvedTheme] = useState<ResolvedTheme>(() => {
    const currentTheme = forcedTheme || getStoredTheme() || defaultTheme;
    return currentTheme === 'system' ? getSystemTheme() : currentTheme;
  });

  const [isLoading, setIsLoading] = useState(true);

  /**
   * Rozwiąż 'system' do konkretnego motywu
   */
  const resolveTheme = useCallback((t: Theme): ResolvedTheme => {
    return t === 'system' ? getSystemTheme() : t;
  }, []);

  /**
   * Ustaw motyw
   */
  const setTheme = useCallback(
    (newTheme: Theme) => {
      if (forcedTheme) return; // Ignoruj jeśli wymuszony

      setThemeState(newTheme);
      setStoredTheme(newTheme);

      const resolved = resolveTheme(newTheme);
      setResolvedTheme(resolved);
      applyTheme(resolved);
    },
    [forcedTheme, resolveTheme]
  );

  /**
   * Przełącz między light/dark
   */
  const toggleTheme = useCallback(() => {
    const newTheme = resolvedTheme === 'dark' ? 'light' : 'dark';
    setTheme(newTheme);
  }, [resolvedTheme, setTheme]);

  /**
   * Nasłuchuj zmian preferencji systemowych
   */
  useEffect(() => {
    if (theme !== 'system') return;

    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

    const handleChange = (e: MediaQueryListEvent) => {
      const newResolved = e.matches ? 'dark' : 'light';
      setResolvedTheme(newResolved);
      applyTheme(newResolved);
    };

    mediaQuery.addEventListener('change', handleChange);

    return () => {
      mediaQuery.removeEventListener('change', handleChange);
    };
  }, [theme]);

  /**
   * Inicjalizacja - aplikuj motyw po montowaniu
   */
  useEffect(() => {
    const currentTheme = forcedTheme || theme;
    const resolved = resolveTheme(currentTheme);
    setResolvedTheme(resolved);
    applyTheme(resolved);
    setIsLoading(false);
  }, [forcedTheme, theme, resolveTheme]);

  /**
   * Wartość kontekstu
   */
  const value = useMemo<ThemeContextValue>(
    () => ({
      theme: forcedTheme || theme,
      resolvedTheme,
      setTheme,
      toggleTheme,
      isLoading,
    }),
    [theme, resolvedTheme, setTheme, toggleTheme, isLoading, forcedTheme]
  );

  return (
    <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
  );
}

/**
 * Hook do używania kontekstu motywu
 */
export function useTheme(): ThemeContextValue {
  const context = useContext(ThemeContext);

  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }

  return context;
}

export default ThemeContext;
