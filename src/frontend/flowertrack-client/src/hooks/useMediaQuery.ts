import { useState, useEffect, useCallback } from 'react';

/**
 * Hook do wykrywania media queries (responsywność)
 *
 * @param query - Media query string, np. '(max-width: 768px)'
 * @returns boolean - czy media query pasuje
 *
 * @example
 * const isMobile = useMediaQuery('(max-width: 768px)');
 * const isTablet = useMediaQuery('(min-width: 769px) and (max-width: 1024px)');
 * const prefersDarkMode = useMediaQuery('(prefers-color-scheme: dark)');
 */
export function useMediaQuery(query: string): boolean {
  const getMatches = useCallback((query: string): boolean => {
    // SSR safety
    if (typeof window !== 'undefined') {
      return window.matchMedia(query).matches;
    }
    return false;
  }, []);

  const [matches, setMatches] = useState<boolean>(() => getMatches(query));

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);

    // Update state if query matches changed
    const handleChange = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    // Set initial value
    setMatches(mediaQueryList.matches);

    // Modern browsers
    mediaQueryList.addEventListener('change', handleChange);

    return () => {
      mediaQueryList.removeEventListener('change', handleChange);
    };
  }, [query]);

  return matches;
}

/**
 * Predefiniowane breakpointy zgodne z design systemem
 */
export const breakpoints = {
  xs: '480px',
  sm: '640px',
  md: '768px',
  lg: '1024px',
  xl: '1280px',
  '2xl': '1536px',
} as const;

/**
 * Gotowe hooki dla typowych breakpointów
 */
export function useIsMobile(): boolean {
  return useMediaQuery(`(max-width: ${breakpoints.md})`);
}

export function useIsTablet(): boolean {
  return useMediaQuery(`(min-width: ${breakpoints.md}) and (max-width: ${breakpoints.lg})`);
}

export function useIsDesktop(): boolean {
  return useMediaQuery(`(min-width: ${breakpoints.lg})`);
}

export function useIsLargeDesktop(): boolean {
  return useMediaQuery(`(min-width: ${breakpoints.xl})`);
}

/**
 * Hook dla preferencji systemowych
 */
export function usePrefersColorScheme(): 'light' | 'dark' {
  const prefersDark = useMediaQuery('(prefers-color-scheme: dark)');
  return prefersDark ? 'dark' : 'light';
}

export function usePrefersReducedMotion(): boolean {
  return useMediaQuery('(prefers-reduced-motion: reduce)');
}

export function usePrefersContrast(): boolean {
  return useMediaQuery('(prefers-contrast: more)');
}

export default useMediaQuery;
