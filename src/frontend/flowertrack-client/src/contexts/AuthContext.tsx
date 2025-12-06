import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';

interface User {
  id: string;
  email: string;
  role: string;
  [key: string]: any;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (token: string, userData: User) => void;
  logout: () => void;
  loading: boolean;
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
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check for persisted token on mount
    const storedToken = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');

    if (storedToken && storedUser) {
      setToken(storedToken);
      try {
        setUser(JSON.parse(storedUser));
      } catch (e) {
        console.error('Failed to parse user data', e);
        localStorage.removeItem('user');
      }
    }
    setLoading(false);
  }, []);

  const login = (newToken: string, newUser: User) => {
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
    // Optional: Redirect to gateway
    window.location.href = '/';
  };

  /**
   * @obsolete DEV ONLY - Mock login without backend authentication
   * TODO: Remove this function before production deployment
   * This allows testing the frontend without a running backend
   */
  const mockLogin = (role: 'client' | 'service') => {
    const mockToken = 'mock-dev-token-' + Date.now();
    const mockUser: User = {
      id: 'mock-user-' + Date.now(),
      email: role === 'client' ? 'testclient@demo.pl' : 'testserwis@demo.pl',
      role: role,
      name: role === 'client' ? 'Test Klient' : 'Test Serwisant',
      organizationId: role === 'client' ? 'mock-org-1' : undefined,
      isAdmin: role === 'service', // Grant admin rights to mock service user
    };
    login(mockToken, mockUser);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        login,
        logout,
        loading,
        mockLogin, // @obsolete - DEV ONLY - Remove before production
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
