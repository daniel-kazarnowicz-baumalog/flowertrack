import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

export interface LoginResponse {
  token: string;
  user: {
    id: string;
    email: string;
    name?: string;
    fullName?: string;
    firstName?: string;
    lastName?: string;
    organizationId?: string;
    organizationName?: string;
    isAdmin?: boolean;
  };
}

export interface AuthServiceType {
  loginService: (email: string, password: string) => Promise<LoginResponse>;
  loginClient: (email: string, password: string) => Promise<LoginResponse>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<string>;
  forgotPassword: (email: string) => Promise<void>;
  resetPassword: (token: string, newPassword: string) => Promise<void>;
}

/**
 * Check if we're in mock mode (no backend available)
 */
const isMockMode = (): boolean => {
  return import.meta.env.VITE_MOCK_API === 'true' || !import.meta.env.VITE_API_URL;
};

/**
 * Mock login for development without backend
 */
const mockServiceLogin = async (email: string, _password: string): Promise<LoginResponse> => {
  // Simulate network delay
  await new Promise(resolve => setTimeout(resolve, 500));
  
  // Simple validation
  if (!email.includes('@')) {
    throw new Error('Nieprawidłowy email');
  }
  
  return {
    token: `mock-service-token-${Date.now()}`,
    user: {
      id: `service-user-${Date.now()}`,
      email: email,
      fullName: 'Jan Kowalski',
      firstName: 'Jan',
      lastName: 'Kowalski',
      isAdmin: email.toLowerCase().includes('admin'),
    },
  };
};

const mockClientLogin = async (email: string, _password: string): Promise<LoginResponse> => {
  await new Promise(resolve => setTimeout(resolve, 500));
  
  if (!email.includes('@')) {
    throw new Error('Nieprawidłowy email');
  }
  
  return {
    token: `mock-client-token-${Date.now()}`,
    user: {
      id: `client-user-${Date.now()}`,
      email: email,
      fullName: 'Anna Nowak',
      firstName: 'Anna',
      lastName: 'Nowak',
      organizationId: 'org-1',
      organizationName: 'Demo Firma Sp. z o.o.',
      isAdmin: email.toLowerCase().includes('admin'),
    },
  };
};

export const authService: AuthServiceType = {
  /**
   * Login as service user (technician/admin)
   */
  loginService: async (email: string, password: string): Promise<LoginResponse> => {
    if (isMockMode()) {
      return mockServiceLogin(email, password);
    }
    
    try {
      const response = await axios.post<LoginResponse>(`${API_BASE_URL}/auth/service/login`, {
        email,
        password,
      });
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw new Error(error.response?.data?.message || 'Nieprawidłowy email lub hasło');
      }
      throw error;
    }
  },

  /**
   * Login as client user
   */
  loginClient: async (email: string, password: string): Promise<LoginResponse> => {
    if (isMockMode()) {
      return mockClientLogin(email, password);
    }
    
    try {
      const response = await axios.post<LoginResponse>(`${API_BASE_URL}/auth/client/login`, {
        email,
        password,
      });
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        throw new Error(error.response?.data?.message || 'Nieprawidłowy email lub hasło');
      }
      throw error;
    }
  },

  /**
   * Logout and invalidate token
   */
  logout: async (): Promise<void> => {
    if (isMockMode()) {
      return;
    }
    
    try {
      await axios.post(`${API_BASE_URL}/auth/logout`);
    } catch {
      // Ignore logout errors
    }
  },

  /**
   * Refresh authentication token
   */
  refreshToken: async (): Promise<string> => {
    if (isMockMode()) {
      return `mock-refreshed-token-${Date.now()}`;
    }
    
    const response = await axios.post<{ token: string }>(`${API_BASE_URL}/auth/refresh`);
    return response.data.token;
  },

  /**
   * Request password reset email
   */
  forgotPassword: async (email: string): Promise<void> => {
    if (isMockMode()) {
      await new Promise(resolve => setTimeout(resolve, 500));
      return;
    }
    
    await axios.post(`${API_BASE_URL}/auth/forgot-password`, { email });
  },

  /**
   * Reset password with token
   */
  resetPassword: async (token: string, newPassword: string): Promise<void> => {
    if (isMockMode()) {
      await new Promise(resolve => setTimeout(resolve, 500));
      return;
    }
    
    await axios.post(`${API_BASE_URL}/auth/reset-password`, {
      token,
      newPassword,
    });
  },
};

export default authService;
