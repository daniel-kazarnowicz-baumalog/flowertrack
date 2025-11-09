import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import type { ReactNode } from 'react';
import type { ServiceUserDto, OrganizationUserDto } from '../types/api';

/**
 * Authentication Context for FLOWerTRACK
 * Manages authentication state for both Service and Client portals
 */

type UserType = 'service' | 'client';

interface AuthState {
  user: ServiceUserDto | OrganizationUserDto | null;
  userType: UserType | null;
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

interface AuthContextValue extends AuthState {
  login: (
    accessToken: string,
    refreshToken: string,
    expiresAt: string,
    user: ServiceUserDto | OrganizationUserDto,
    userType: UserType
  ) => void;
  logout: () => void;
  updateUser: (user: ServiceUserDto | OrganizationUserDto) => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

const STORAGE_KEYS = {
  ACCESS_TOKEN: 'accessToken',
  REFRESH_TOKEN: 'refreshToken',
  EXPIRES_AT: 'expiresAt',
  USER: 'user',
  USER_TYPE: 'userType',
} as const;

export function AuthProvider({ children }: AuthProviderProps) {
  const [state, setState] = useState<AuthState>({
    user: null,
    userType: null,
    accessToken: null,
    refreshToken: null,
    expiresAt: null,
    isAuthenticated: false,
    isLoading: true,
  });

  // Load auth state from localStorage on mount
  useEffect(() => {
    const loadAuthState = () => {
      try {
        const accessToken = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
        const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
        const expiresAt = localStorage.getItem(STORAGE_KEYS.EXPIRES_AT);
        const userStr = localStorage.getItem(STORAGE_KEYS.USER);
        const userType = localStorage.getItem(STORAGE_KEYS.USER_TYPE) as UserType | null;

        if (accessToken && userStr && userType) {
          const user = JSON.parse(userStr);

          // Check if token is expired
          if (expiresAt && new Date(expiresAt) > new Date()) {
            setState({
              user,
              userType,
              accessToken,
              refreshToken,
              expiresAt,
              isAuthenticated: true,
              isLoading: false,
            });
            return;
          }
        }

        // No valid auth state
        setState((prev) => ({ ...prev, isLoading: false }));
      } catch (error) {
        console.error('Failed to load auth state:', error);
        setState((prev) => ({ ...prev, isLoading: false }));
      }
    };

    loadAuthState();
  }, []);

  const login = useCallback(
    (
      accessToken: string,
      refreshToken: string,
      expiresAt: string,
      user: ServiceUserDto | OrganizationUserDto,
      userType: UserType
    ) => {
      // Save to localStorage
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, accessToken);
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
      localStorage.setItem(STORAGE_KEYS.EXPIRES_AT, expiresAt);
      localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
      localStorage.setItem(STORAGE_KEYS.USER_TYPE, userType);

      // Update state
      setState({
        user,
        userType,
        accessToken,
        refreshToken,
        expiresAt,
        isAuthenticated: true,
        isLoading: false,
      });
    },
    []
  );

  const logout = useCallback(() => {
    // Clear localStorage
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.EXPIRES_AT);
    localStorage.removeItem(STORAGE_KEYS.USER);
    localStorage.removeItem(STORAGE_KEYS.USER_TYPE);

    // Reset state
    setState({
      user: null,
      userType: null,
      accessToken: null,
      refreshToken: null,
      expiresAt: null,
      isAuthenticated: false,
      isLoading: false,
    });
  }, []);

  const updateUser = useCallback((user: ServiceUserDto | OrganizationUserDto) => {
    localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
    setState((prev) => ({ ...prev, user }));
  }, []);

  const value: AuthContextValue = {
    ...state,
    login,
    logout,
    updateUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

/**
 * Hook to access authentication context
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

/**
 * Type guard to check if user is a Service User
 */
export function isServiceUser(
  user: ServiceUserDto | OrganizationUserDto | null
): user is ServiceUserDto {
  return user !== null && 'role' in user;
}

/**
 * Type guard to check if user is an Organization User
 */
export function isOrganizationUser(
  user: ServiceUserDto | OrganizationUserDto | null
): user is OrganizationUserDto {
  return user !== null && 'organizationId' in user;
}
