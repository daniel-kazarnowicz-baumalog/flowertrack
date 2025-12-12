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

    console.log('[AuthContext] Initializing...', {
      hasToken: !!storedToken,
      hasUser: !!storedUser,
    });

    if (storedToken && storedUser) {
      setToken(storedToken);
      try {
        const parsedUser = JSON.parse(storedUser);
        console.log('[AuthContext] Restored user:', parsedUser);
        setUser(parsedUser);
      } catch (e) {
        console.error('Failed to parse user data', e);
        localStorage.removeItem('user');
        localStorage.removeItem('accessToken');
      }
    } else {
      console.log('[AuthContext] No session found.');
    }
    setIsLoading(false);
  }, []);

  const login = (newToken: string, newUser: User) => {
    console.log('[AuthContext] Login called', { token: newToken, user: newUser });
    if (!newToken) console.error('[AuthContext] Token is missing/undefined!');

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
    const response = await authService.loginService(email, password);
    // Backend returns role as 'Admin' or 'Technician' in response.user.role
    // We map this to isAdmin and set the context role to 'service'
    const isAdmin = response.user.role === 'Admin';

    login(response.accessToken, {
      ...response.user,
      role: 'service',
      isAdmin,
    } as User);
  };

  const loginClient = async (email: string, password: string): Promise<void> => {
    const response = await authService.loginClient(email, password);
    login(response.accessToken, {
      ...response.user,
      role: 'client',
    } as User);
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
