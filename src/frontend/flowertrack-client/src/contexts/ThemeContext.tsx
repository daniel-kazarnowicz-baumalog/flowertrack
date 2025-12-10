import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';

export type Theme = 'light' | 'dark' | 'system';

interface ThemeContextType {
  theme: Theme;
  setTheme: (theme: Theme) => void;
  // The actual resolved theme (light or dark)
  resolvedTheme: 'light' | 'dark';
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

export const ThemeProvider = ({ children }: { children: ReactNode }) => {
  const [theme, setThemeState] = useState<Theme>(
    () => (localStorage.getItem('theme') as Theme) || 'system'
  );

  const [resolvedTheme, setResolvedTheme] = useState<'light' | 'dark'>('light');

  useEffect(() => {
    localStorage.setItem('theme', theme);

    const root = window.document.documentElement;
    const systemQuery = window.matchMedia('(prefers-color-scheme: dark)');

    function applyTheme() {
      let targetTheme = theme;
      if (theme === 'system') {
        targetTheme = systemQuery.matches ? 'dark' : 'light';
      }

      // Remove both first
      root.classList.remove('light', 'dark');
      root.removeAttribute('data-theme');

      // Apply new
      root.classList.add(targetTheme);
      root.setAttribute('data-theme', targetTheme);
      setResolvedTheme(targetTheme as 'light' | 'dark');
    }

    applyTheme();

    if (theme === 'system') {
      systemQuery.addEventListener('change', applyTheme);
      return () => systemQuery.removeEventListener('change', applyTheme);
    }
  }, [theme]);

  const setTheme = (newTheme: Theme) => {
    setThemeState(newTheme);
  };

  const toggleTheme = () => {
    setThemeState((prev) => {
      if (prev === 'light') return 'dark';
      if (prev === 'dark') return 'light';
      // If system, switch to opposite of resolved
      return resolvedTheme === 'light' ? 'dark' : 'light';
    });
  };

  return (
    <ThemeContext.Provider value={{ theme, setTheme, resolvedTheme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};
