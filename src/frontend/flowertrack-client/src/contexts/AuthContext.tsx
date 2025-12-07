import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { authService } from '../services/authService';

export interface User {
  id: string;
  email: string;
  role: 'service' | 'client';
  name?: string;
  fullName?: string;
  firstName?: string;
  lastName?: string;
  organizationId?: string;
  organizationName?: string;
  isAdmin?: boolean;
}

export interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (token: string, userData: User) => void;
  logout: () => void;
  loginService: (email: string, password: string) => Promise<void>;
  loginClient: (email: string, password: string) => Promise<void>;
  /**
   * @obsolete DEV ONLY - Mock login without backend authentication
   * TODO: Remove before production deployment
   */
  mockLogin: (role: 'client' | 'service') => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for persisted token on mount
    const storedToken = localStorage.getItem('accessToken');
    const storedUser = localStorage.getItem('user');

    if (storedToken && storedUser) {
      setToken(storedToken);
      try {
        setUser(JSON.parse(storedUser));
      } catch (e) {
        console.error('Failed to parse user data', e);
        localStorage.removeItem('user');
        localStorage.removeItem('accessToken');
      }
    }
    setIsLoading(false);
  }, []);

  const login = (newToken: string, newUser: User) => {
    localStorage.setItem('accessToken', newToken);
    localStorage.setItem('user', JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
  };

  const loginService = async (email: string, password: string): Promise<void> => {
    try {
      const response = await authService.loginService(email, password);
      login(response.token, {
        ...response.user,
        role: 'service',
      });
    } catch (error) {
      // Re-throw to let the component handle it
      throw error;
    }
  };

  const loginClient = async (email: string, password: string): Promise<void> => {
    try {
      const response = await authService.loginClient(email, password);
      login(response.token, {
        ...response.user,
        role: 'client',
      });
    } catch (error) {
      throw error;
    }
  };

  /**
   * @obsolete DEV ONLY - Mock login without backend authentication
   * TODO: Remove this function before production deployment
   */
  const mockLogin = (role: 'client' | 'service') => {
    const mockToken = 'mock-dev-token-' + Date.now();
    const mockUser: User = {
      id: 'mock-user-' + Date.now(),
      email: role === 'client' ? 'testclient@demo.pl' : 'testserwis@demo.pl',
      role,
      name: role === 'client' ? 'Test Klient' : 'Test Serwisant',
      fullName: role === 'client' ? 'Test Klient' : 'Test Serwisant',
      firstName: 'Test',
      lastName: role === 'client' ? 'Klient' : 'Serwisant',
      organizationId: role === 'client' ? 'mock-org-1' : undefined,
      organizationName: role === 'client' ? 'Demo Firma Sp. z o.o.' : undefined,
      isAdmin: true, // Grant admin rights to mock users for testing
    };
    login(mockToken, mockUser);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        isLoading,
        login,
        logout,
        loginService,
        loginClient,
        mockLogin,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
